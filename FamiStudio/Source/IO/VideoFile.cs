﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

#if FAMISTUDIO_WINDOWS
    using RenderFont     = SharpDX.DirectWrite.TextFormat;
    using RenderBitmap   = SharpDX.Direct2D1.Bitmap;
    using RenderBrush    = SharpDX.Direct2D1.Brush;
    using RenderControl  = FamiStudio.Direct2DControl;
    using RenderGraphics = FamiStudio.Direct2DOffscreenGraphics;
    using RenderTheme    = FamiStudio.Direct2DTheme;
#else
    using RenderFont     = FamiStudio.GLFont;
    using RenderBitmap   = FamiStudio.GLBitmap;
    using RenderBrush    = FamiStudio.GLBrush;
    using RenderControl  = FamiStudio.GLControl;
    using RenderGraphics = FamiStudio.GLOffscreenGraphics;
    using RenderTheme    = FamiStudio.GLTheme;
#endif

namespace FamiStudio
{
    class VideoFile
    {
        const int   ChannelIconTextSpacing = 8;
        const int   ChannelIconPosY = 26;
        const int   SegmentTransitionNumFrames = 16;
        const int   SampleRate = 44100;
        const int   ThinNoteThreshold     = 288;
        const int   VeryThinNoteThreshold = 192;
        const float OscilloscopeWindowSize = 0.075f; // in sec.

        int videoResX = 1920;
        int videoResY = 1080;

        // Mostly from : https://github.com/kometbomb/oscilloscoper/blob/master/src/Oscilloscope.cpp
        private void GenerateOscilloscope(short[] wav, int position, int windowSize, int maxLookback, float scaleY, float minX, float minY, float maxX, float maxY, float[,] oscilloscope)
        {
            // Find a point where the waveform crosses the axis, looks nicer.
            int lookback = 0;
            int orig = wav[position];

            // If sample is negative, go back until positive.
            if (orig < 0)
            {
                while (lookback < maxLookback)
                {
                    if (position == 0 || wav[--position] > 0)
                        break;

                    lookback++;
                }

                orig = wav[position];
            }


            // Then look for a zero crossing.
            if (orig > 0)
            {
                while (lookback < maxLookback)
                {
                    if (position == wav.Length -1 || wav[++position] < 0)
                        break;

                    lookback++;
                }
            }

            int lastIdx = -1;
            int oscLen = oscilloscope.GetLength(0);

            for (int i = 0; i < oscLen; ++i)
            {
                var idx = Utils.Clamp(position - windowSize / 2 + i * windowSize / oscLen, 0, wav.Length - 1);
                var avg = (float)wav[idx];
                var cnt = 1;

                if (lastIdx >= 0)
                {
                    for (int j = lastIdx + 1; j < idx; j++, cnt++)
                        avg += wav[j];
                    avg /= cnt;
                }

                var sample = Utils.Clamp((int)(avg * scaleY), short.MinValue, short.MaxValue);

                var x = Utils.Lerp(minX, maxX, i / (float)(oscLen - 1));
                var y = Utils.Lerp(minY, maxY, (sample - short.MinValue) / (float)(ushort.MaxValue));

                oscilloscope[i, 0] = x;
                oscilloscope[i, 1] = y;

                lastIdx = idx;
            }
        }

        private void ComputeChannelsScroll(VideoFrameMetadata[] frames, int channelMask, int numVisibleNotes)
        {
            var numFrames = frames.Length;
            var numChannels = frames[0].channelNotes.Length;

            for (int c = 0; c < numChannels; c++)
            {
                if ((channelMask & (1 << c)) == 0)
                    continue;

                // Go through all the frames and split them in segments. 
                // A segment is a section of the song where all the notes fit in the view.
                var segments = new List<ScrollSegment>();
                var currentSegment = (ScrollSegment)null;
                var minOverallNote = int.MaxValue;
                var maxOverallNote = int.MinValue;

                for (int f = 0; f < numFrames; f++)
                {
                    var frame = frames[f];
                    var note  = frame.channelNotes[c];

                    if (frame.scroll == null)
                        frame.scroll = new float[numChannels];

                    if (note.IsMusical)
                    {
                        if (currentSegment == null)
                        {
                            currentSegment = new ScrollSegment();
                            segments.Add(currentSegment);
                        }

                        // If its the start of a new pattern and we've been not moving for ~10 sec, let's start a new segment.
                        bool forceNewSegment = frame.playNote == 0 && (f - currentSegment.startFrame) > 600;

                        var minNoteValue = note.Value - 1;
                        var maxNoteValue = note.Value + 1;

                        // Only consider slides if they arent too large.
                        if (note.IsSlideNote && Math.Abs(note.SlideNoteTarget - note.Value) < numVisibleNotes / 2)
                        {
                            minNoteValue = Math.Min(note.Value, note.SlideNoteTarget) - 1;
                            maxNoteValue = Math.Max(note.Value, note.SlideNoteTarget) + 1;
                        }

                        // Only consider arpeggios if they are not too big.
                        if (note.IsArpeggio && note.Arpeggio.GetChordMinMaxOffset(out var minArp, out var maxArp) && maxArp - minArp < numVisibleNotes / 2)
                        {
                            minNoteValue = note.Value + minArp;
                            maxNoteValue = note.Value + maxArp;
                        }

                        minOverallNote = Math.Min(minOverallNote, minNoteValue);
                        maxOverallNote = Math.Max(maxOverallNote, maxNoteValue);

                        var newMinNote = Math.Min(currentSegment.minNote, minNoteValue);
                        var newMaxNote = Math.Max(currentSegment.maxNote, maxNoteValue);

                        // If we cant fit the next note in the view, start a new segment.
                        if (forceNewSegment || newMaxNote - newMinNote + 1 > numVisibleNotes)
                        {
                            currentSegment.endFrame = f;
                            currentSegment = new ScrollSegment();
                            currentSegment.startFrame = f;
                            segments.Add(currentSegment);

                            currentSegment.minNote = minNoteValue;
                            currentSegment.maxNote = maxNoteValue;
                        }
                        else
                        {
                            currentSegment.minNote = newMinNote;
                            currentSegment.maxNote = newMaxNote;
                        }
                    }
                }

                // Not a single notes in this channel...
                if (currentSegment == null)
                {
                    currentSegment = new ScrollSegment();
                    currentSegment.minNote = Note.FromFriendlyName("C4");
                    currentSegment.maxNote = currentSegment.minNote;
                    segments.Add(currentSegment);
                }

                currentSegment.endFrame = numFrames;

                // Remove very small segments, these make the camera move too fast, looks bad.
                var shortestAllowedSegment = SegmentTransitionNumFrames * 2;

                bool removed = false;
                do
                {
                    var sortedSegment = new List<ScrollSegment>(segments);

                    sortedSegment.Sort((s1, s2) => s1.NumFrames.CompareTo(s2.NumFrames));

                    if (sortedSegment[0].NumFrames >= shortestAllowedSegment)
                        break;

                    for (int s = 0; s < sortedSegment.Count; s++)
                    {
                        var seg = sortedSegment[s];

                        if (seg.NumFrames >= shortestAllowedSegment)
                            break;

                        var thisSegmentIndex = segments.IndexOf(seg);

                        // Segment is too short, see if we can merge with previous/next one.
                        var mergeSegmentIndex  = -1;
                        var mergeSegmentLength = -1;
                        if (thisSegmentIndex > 0)
                        {
                            mergeSegmentIndex  = thisSegmentIndex - 1;
                            mergeSegmentLength = segments[thisSegmentIndex - 1].NumFrames;
                        }
                        if (thisSegmentIndex != segments.Count - 1 && segments[thisSegmentIndex + 1].NumFrames > mergeSegmentLength)
                        {
                            mergeSegmentIndex = thisSegmentIndex + 1;
                            mergeSegmentLength = segments[thisSegmentIndex + 1].NumFrames;
                        }
                        if (mergeSegmentIndex >= 0)
                        {
                            // Merge.
                            var mergeSeg = segments[mergeSegmentIndex];
                            mergeSeg.startFrame = Math.Min(mergeSeg.startFrame, seg.startFrame);
                            mergeSeg.endFrame   = Math.Max(mergeSeg.endFrame, seg.endFrame);
                            segments.RemoveAt(thisSegmentIndex);
                            removed = true;
                            break;
                        }
                    }
                }
                while (removed);

                // Build the actually scrolling data. 
                var minScroll = (float)Math.Ceiling(Note.MusicalNoteMin + numVisibleNotes * 0.5f);
                var maxScroll = (float)Math.Floor  (Note.MusicalNoteMax - numVisibleNotes * 0.5f);

                Debug.Assert(maxScroll >= minScroll);

                foreach (var segment in segments)
                {
                    segment.scroll = Utils.Clamp(segment.minNote + (segment.maxNote - segment.minNote) * 0.5f, minScroll, maxScroll);
                }

                for (var s = 0; s < segments.Count; s++)
                {
                    var segment0 = segments[s + 0];
                    var segment1 = s == segments.Count - 1 ? null : segments[s + 1];

                    for (int f = segment0.startFrame; f < segment0.endFrame - (segment1 == null ? 0 : SegmentTransitionNumFrames); f++)
                    {
                        frames[f].scroll[c] = segment0.scroll;
                    }

                    if (segment1 != null)
                    {
                        // Smooth transition to next segment.
                        for (int f = segment0.endFrame - SegmentTransitionNumFrames, a = 0; f < segment0.endFrame; f++, a++)
                        {
                            var lerp = a / (float)SegmentTransitionNumFrames;
                            frames[f].scroll[c] = Utils.Lerp(segment0.scroll, segment1.scroll, Utils.SmootherStep(lerp));
                        }
                    }
                }
            }
        }

        private void SmoothFamitrackerScrolling(VideoFrameMetadata[] frames)
        {
            var numFrames = frames.Length;

            for (int f = 0; f < numFrames; )
            {
                var thisFrame = frames[f];

                var currentPlayPattern = thisFrame.playPattern;
                var currentPlayNote    = thisFrame.playNote;

                // Keep moving forward until we see that we have advanced by 1 row.
                int nf = f + 1;
                for (; nf < numFrames; nf++)
                {
                    var nextFrame = frames[nf];
                    if (nextFrame.playPattern != thisFrame.playPattern ||
                        nextFrame.playNote    != thisFrame.playNote)
                    {
                        break;
                    }
                }

                var numFramesSameNote = nf - f;

                // Smooth out movement linearly.
                for (int i = 0; i < numFramesSameNote; i++)
                {
                    frames[f + i].playNote += i / (float)numFramesSameNote;
                }

                f = nf;
            }
        }

        private void SmoothFamiStudioScrolling(VideoFrameMetadata[] frames, Song song)
        {
            var patternIndices = new int[frames.Length];
            var absoluteNoteIndices = new float[frames.Length];

            // Keep copy of original pattern/notes.
            for (int i = 0; i < frames.Length; i++)
            {
                patternIndices[i] = frames[i].playPattern;
                absoluteNoteIndices[i] = song.GetPatternStartAbsoluteNoteIndex(frames[i].playPattern, (int)frames[i].playNote);
            }

            // Do moving average to smooth the movement.
            for (int i = 0; i < frames.Length; i++)
            {
                var averageSize = (Utils.Max(song.GetPatternGroove(patternIndices[i])) + 1) / 2;

                averageSize = Math.Min(averageSize, i);
                averageSize = Math.Min(averageSize, absoluteNoteIndices.Length - i - 1);

                var sum = 0.0f;
                var cnt = 0;
                for (int j = i - averageSize; j <= i + averageSize; j++)
                {
                    if (j >= 0 && j < absoluteNoteIndices.Length)
                    {
                        sum += absoluteNoteIndices[j];
                        cnt++;
                    }
                }
                sum /= cnt;

                frames[i].playPattern = song.PatternIndexFromAbsoluteNoteIndex((int)sum);
                frames[i].playNote = sum - song.GetPatternStartAbsoluteNoteIndex(frames[i].playPattern);
            }
        }

        private Process LaunchFFmpeg(string ffmpegExecutable, string commandLine, bool redirectStdIn, bool redirectStdOut)
        {
            var psi = new ProcessStartInfo(ffmpegExecutable, commandLine);

            psi.UseShellExecute = false;
            psi.WorkingDirectory = Path.GetDirectoryName(ffmpegExecutable);
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            if (redirectStdIn)
            {
                psi.RedirectStandardInput = true;
            }

            if (redirectStdOut)
            {
                psi.RedirectStandardOutput = true;
            }

            return Process.Start(psi);
        }

        bool DetectFFmpeg(string ffmpegExecutable)
        {
            try
            {
                var process = LaunchFFmpeg(ffmpegExecutable, $"-version", false, true);
                var output = process.StandardOutput.ReadToEnd();

                var ret = true;
                if (!output.Contains("--enable-libx264"))
                {
                    Log.LogMessage(LogSeverity.Error, "ffmpeg does not seem to be compiled with x264 support. Make sure you have the GPL version.");
                    ret = false;
                }

                process.WaitForExit();
                process.Dispose();

                return ret;
            }
            catch
            {
                Log.LogMessage(LogSeverity.Error, "Error launching ffmpeg. Make sure the path is correct.");
                return false;
            }
        }

        private void ExtendSongForLooping(Song song, int loopCount)
        {
            // For looping, we simply extend the song by copying pattern instances.
            if (loopCount > 1 && song.LoopPoint >= 0 && song.LoopPoint < song.Length)
            {
                var originalLength = song.Length;
                var loopSectionLength = originalLength - song.LoopPoint;

                song.SetLength(Math.Min(Song.MaxLength, originalLength + loopSectionLength * (loopCount - 1)));

                var srcPatIdx = song.LoopPoint;

                for (var i = originalLength; i < song.Length; i++)
                {
                    foreach (var c in song.Channels)
                        c.PatternInstances[i] = c.PatternInstances[srcPatIdx];

                    if (song.PatternHasCustomSettings(srcPatIdx))
                    {
                        var customSettings = song.GetPatternCustomSettings(srcPatIdx);
                        song.SetPatternCustomSettings(i, customSettings.patternLength, customSettings.beatLength, customSettings.groove, customSettings.groovePaddingMode);
                    }

                    if (++srcPatIdx >= originalLength)
                        srcPatIdx = song.LoopPoint;
                }
            }
        }

        // Not tested in a while, probably wrong channel order.
        private unsafe void DumpDebugImage(byte[] image, int sizeX, int sizeY, int frameIndex)
        {
#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
            var pb = new Gdk.Pixbuf(image, true, 8, sizeX, sizeY, sizeX * 4);
            pb.Save($"/home/mat/Downloads/frame_{frameIndex:D4}.png", "png");
#else
            fixed (byte* vp = &image[0])
            {
                var b = new Bitmap(sizeX, sizeY, sizeX * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, new IntPtr(vp));
                b.Save($"d:\\dump\\pr\\frame_{frameIndex:D4}.png");
            }
#endif
        }

        class VideoChannelState
        {
            public int videoChannelIndex;
            public int songChannelIndex;
            public int patternIndex;
            public string channelText;
            public int volume;
            public Note note;
            public Channel channel;
            public RenderBitmap bmpIcon;
            public RenderGraphics graphics;
            public RenderBitmap bitmap;
            public short[] wav;
        };

        public unsafe bool Save(Project originalProject, int songId, int loopCount, string ffmpegExecutable, string filename, int resX, int resY, bool halfFrameRate, int channelMask, int audioBitRate, int videoBitRate, int pianoRollZoom)
        {
            if (channelMask == 0 || loopCount < 1)
                return false;

            Log.LogMessage(LogSeverity.Info, "Detecting FFmpeg...");

            if (!DetectFFmpeg(ffmpegExecutable))
                return false;
            
            videoResX = resX;
            videoResY = resY;

            var project = originalProject.DeepClone();
            var song = project.GetSong(songId);

            ExtendSongForLooping(song, loopCount);

            Log.LogMessage(LogSeverity.Info, "Initializing channels...");

            var frameRateNumerator = song.Project.PalMode ? 5000773 : 6009883;
            if (halfFrameRate)
                frameRateNumerator /= 2;
            var frameRate = frameRateNumerator.ToString() + "/100000";

            var numChannels = Utils.NumberOfSetBits(channelMask);
            var channelResXFloat = videoResX / (float)numChannels;
            var channelResX = videoResY;
            var channelResY = (int)channelResXFloat;
            var longestChannelName = 0.0f;

            var videoGraphics = RenderGraphics.Create(videoResX, videoResY, true);

            if (videoGraphics == null)
            {
                Log.LogMessage(LogSeverity.Error, "Error initializing off-screen graphics, aborting.");
                return false;
            }
            
            var theme = RenderTheme.CreateResourcesForGraphics(videoGraphics);
            var bmpWatermark = videoGraphics.CreateBitmapFromResource("VideoWatermark");

            // Generate WAV data for each individual channel for the oscilloscope.
            var channelStates = new List<VideoChannelState>();

            List<short[]> channelsWavData  = new List<short[]>();
            var maxAbsSample = 0;

            for (int i = 0, channelIndex = 0; i < song.Channels.Length; i++)
            {
                if ((channelMask & (1 << i)) == 0)
                    continue;

                var pattern = song.Channels[i].PatternInstances[0];
                var state = new VideoChannelState();

                state.videoChannelIndex = channelIndex;
                state.songChannelIndex = i;
                state.channel = song.Channels[i];
                state.patternIndex = 0;
                state.channelText = state.channel.Name + (state.channel.IsExpansionChannel ? $" ({song.Project.ExpansionAudioShortName})" : "");
                state.wav = new WavPlayer(SampleRate, 1, 1 << i).GetSongSamples(song, song.Project.PalMode, -1);
                state.graphics = RenderGraphics.Create(channelResX, channelResY, false);
                state.bitmap = videoGraphics.CreateBitmapFromOffscreenGraphics(state.graphics);

                channelStates.Add(state);
                channelIndex++;

                // Find maximum absolute value to rescale the waveform.
                foreach (short s in state.wav)
                    maxAbsSample = Math.Max(maxAbsSample, Math.Abs(s));

                // Measure the longest text.
                longestChannelName = Math.Max(longestChannelName, state.graphics.MeasureString(state.channelText, ThemeBase.FontBigUnscaled));
            }

            // Tweak some cosmetic stuff that depends on resolution.
            var smallChannelText = longestChannelName + 32 + ChannelIconTextSpacing > channelResY * 0.8f;
            var bmpSuffix = smallChannelText ? "" : "@2x";
            var font = smallChannelText ? ThemeBase.FontMediumUnscaled : ThemeBase.FontBigUnscaled;
            var textOffsetY = smallChannelText ? 1 : 4;
            var pianoRollScaleX = Utils.Clamp(resY / 1080.0f, 0.6f, 0.9f);
            var pianoRollScaleY = channelResY < VeryThinNoteThreshold ? 0.5f : (channelResY < ThinNoteThreshold ? 0.667f : 1.0f);
            var channelLineWidth = channelResY < ThinNoteThreshold ? 3 : 5;
            var gradientSizeY = 256 * (videoResY / 1080.0f);
            var gradientBrush = videoGraphics.CreateVerticalGradientBrush(0, gradientSizeY, Color.Black, Color.FromArgb(0, Color.Black));

            foreach (var s in channelStates)
                s.bmpIcon = videoGraphics.CreateBitmapFromResource(ChannelType.Icons[s.channel.Type] + bmpSuffix);

            // Generate the metadata for the video so we know what's happening at every frame
            var metadata = new VideoMetadataPlayer(SampleRate, 1).GetVideoMetadata(song, song.Project.PalMode, -1);

            var oscScale = maxAbsSample != 0 ? short.MaxValue / (float)maxAbsSample : 1.0f;
            var oscLookback = (metadata[1].wavOffset - metadata[0].wavOffset) / 2;

#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
            var dummyControl = new DummyGLControl();
            dummyControl.Move(0, 0, videoResX, videoResY);
#endif

            // Setup piano roll and images.
            var pianoRoll = new PianoRoll();
#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
            pianoRoll.Move(0, 0, channelResX, channelResY);
#else
            pianoRoll.Width  = channelResX;
            pianoRoll.Height = channelResY;
#endif

            pianoRoll.StartVideoRecording(channelStates[0].graphics, song, pianoRollZoom, pianoRollScaleX, pianoRollScaleY, out var noteSizeY);

            // Build the scrolling data.
            var numVisibleNotes = (int)Math.Floor(channelResY / (float)noteSizeY);
            ComputeChannelsScroll(metadata, channelMask, numVisibleNotes);

            if (song.UsesFamiTrackerTempo)
                SmoothFamitrackerScrolling(metadata);
            else
                SmoothFamiStudioScrolling(metadata, song);

            var videoImage   = new byte[videoResY * videoResX * 4];
            var oscilloscope = new float[channelResY, 2];

            // Start ffmpeg with pipe input.
            var tempFolder = Utils.GetTemporaryDiretory();
            var tempAudioFile = Path.Combine(tempFolder, "temp.wav");

#if !DEBUG
            try
#endif
            {
                Log.LogMessage(LogSeverity.Info, "Exporting audio...");

                // Save audio to temporary file.
                WavMp3ExportUtils.Save(song, tempAudioFile, SampleRate, 1, -1, channelMask, false, false, (samples, fn) => { WaveFile.Save(samples, fn, SampleRate); });

                var process = LaunchFFmpeg(ffmpegExecutable, $"-y -f rawvideo -pix_fmt argb -s {videoResX}x{videoResY} -r {frameRate} -i - -i \"{tempAudioFile}\" -c:v h264 -pix_fmt yuv420p -b:v {videoBitRate}K -c:a aac -b:a {audioBitRate}k \"{filename}\"", true, false);

                // Generate each of the video frames.
                using (var stream = new BinaryWriter(process.StandardInput.BaseStream))
                {
                    for (int f = 0; f < metadata.Length; f++)
                    {
                        if (Log.ShouldAbortOperation)
                            break;

                        if ((f % 100) == 0)
                            Log.LogMessage(LogSeverity.Info, $"Rendering frame {f} / {metadata.Length}");

                        Log.ReportProgress(f / (float)(metadata.Length - 1));

                        if (halfFrameRate && (f & 1) != 0)
                            continue;

                        var frame = metadata[f];

                        // Render the piano rolls for each channels.
                        foreach (var s in channelStates)
                        {
                            s.volume = frame.channelVolumes[s.songChannelIndex];
                            s.note = frame.channelNotes[s.songChannelIndex];

                            var color = Color.Transparent;

                            if (s.note.IsMusical)
                            {
                                if (s.channel.Type == ChannelType.Dpcm)
                                {
                                    var mapping = project.GetDPCMMapping(s.note.Value);
                                    if (mapping != null && mapping.Sample != null)
                                        color = mapping.Sample.Color;
                                }
                                else
                                {
                                    color = Color.FromArgb(128 + s.volume * 127 / 15, s.note.Instrument != null ? s.note.Instrument.Color : ThemeBase.DarkGreyFillColor2);
                                }
                            }

#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
                            s.graphics.BeginDraw(pianoRoll, channelResY);
#else
                            s.graphics.BeginDraw();
#endif
                            pianoRoll.RenderVideoFrame(s.graphics, Channel.ChannelTypeToIndex(s.channel.Type), frame.playPattern, frame.playNote, frame.scroll[s.songChannelIndex], s.note.Value, color);
                            s.graphics.EndDraw();
                        }

                        // Render the full screen overlay.
#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
                        videoGraphics.BeginDraw(dummyControl, videoResY);
#else
                        videoGraphics.BeginDraw();
#endif
                        videoGraphics.Clear(Color.Black);

                        // Composite the channel renders.
                        foreach (var s in channelStates)
                        {
                            int channelPosX1 = (int)Math.Round((s.videoChannelIndex + 1) * channelResXFloat);
                            videoGraphics.DrawRotatedFlippedBitmap(s.bitmap, channelPosX1, videoResY, s.bitmap.Size.Width, s.bitmap.Size.Height);
                        }

                        // Gradient
                        videoGraphics.FillRectangle(0, 0, videoResX, gradientSizeY, gradientBrush);

                        // Channel names + oscilloscope
                        foreach (var s in channelStates)
                        {
                            int channelPosX0 = (int)Math.Round((s.videoChannelIndex + 0) * channelResXFloat);
                            int channelPosX1 = (int)Math.Round((s.videoChannelIndex + 1) * channelResXFloat);

                            var channelNameSizeX = videoGraphics.MeasureString(s.channelText, font);
                            var channelIconPosX = channelPosX0 + channelResY / 2 - (channelNameSizeX + s.bmpIcon.Size.Width + ChannelIconTextSpacing) / 2;

                            videoGraphics.FillRectangle(channelIconPosX, ChannelIconPosY, channelIconPosX + s.bmpIcon.Size.Width, ChannelIconPosY + s.bmpIcon.Size.Height, theme.DarkGreyLineBrush2);
                            videoGraphics.DrawBitmap(s.bmpIcon, channelIconPosX, ChannelIconPosY);
                            videoGraphics.DrawText(s.channelText, font, channelIconPosX + s.bmpIcon.Size.Width + ChannelIconTextSpacing, ChannelIconPosY + textOffsetY, theme.LightGreyFillBrush1);

                            if (s.videoChannelIndex > 0)
                                videoGraphics.DrawLine(channelPosX0, 0, channelPosX0, videoResY, theme.BlackBrush, channelLineWidth);

                            var oscMinY = (int)(ChannelIconPosY + s.bmpIcon.Size.Height + 10);
                            var oscMaxY = (int)(oscMinY + 100.0f * (resY / 1080.0f));

                            GenerateOscilloscope(s.wav, frame.wavOffset, (int)Math.Round(SampleRate * OscilloscopeWindowSize), oscLookback, oscScale, channelPosX0 + 10, oscMinY, channelPosX1 - 10, oscMaxY, oscilloscope);

                            videoGraphics.AntiAliasing = true;
                            videoGraphics.DrawLine(oscilloscope, theme.LightGreyFillBrush1);
                            videoGraphics.AntiAliasing = false;
                        }

                        // Watermark.
                        videoGraphics.DrawBitmap(bmpWatermark, videoResX - bmpWatermark.Size.Width, videoResY - bmpWatermark.Size.Height);
                        videoGraphics.EndDraw();

                        // Readback + send to ffmpeg.
                        videoGraphics.GetBitmap(videoImage);
                        stream.Write(videoImage);

                        // Dump debug images.
                        // DumpDebugImage(videoImage, videoResX, videoResY, f);
                    }
                }

                process.WaitForExit();
                process.Dispose();
                process = null;

                File.Delete(tempAudioFile);
            }
#if !DEBUG
            catch (Exception e)
            {
                Log.LogMessage(LogSeverity.Error, "Error exporting video.");
                Log.LogMessage(LogSeverity.Error, e.Message);
            }
            finally
#endif
            {
                pianoRoll.EndVideoRecording();
                foreach (var c in channelStates)
                {
                    c.bmpIcon.Dispose();
                    c.bitmap.Dispose();
                    c.graphics.Dispose();
                }
                theme.Terminate();
                bmpWatermark.Dispose();
                gradientBrush.Dispose();
                videoGraphics.Dispose();
            }

            return true;
        }
    }

    class ScrollSegment
    {
        public int startFrame;
        public int endFrame;
        public int minNote = int.MaxValue;
        public int maxNote = int.MinValue;
        public int NumFrames => endFrame - startFrame;
        public float scroll;
    }

    class VideoFrameMetadata
    {
        public int     playPattern;
        public float   playNote;
        public int     wavOffset;
        public Note[]  channelNotes;
        public int[]   channelVolumes;
        public float[] scroll;
    };

    class VideoMetadataPlayer : BasePlayer
    {
        int numSamples = 0;
        int prevNumSamples = 0;
        List<VideoFrameMetadata> metadata;

        public VideoMetadataPlayer(int sampleRate, int maxLoop) : base(NesApu.APU_WAV_EXPORT, sampleRate)
        {
            maxLoopCount = maxLoop;
            metadata = new List<VideoFrameMetadata>();
        }

        private void WriteMetadata(List<VideoFrameMetadata> metadata)
        {
            var meta = new VideoFrameMetadata();

            meta.playPattern = playLocation.PatternIndex;
            meta.playNote = playLocation.NoteIndex;
            meta.wavOffset = prevNumSamples;
            meta.channelNotes = new Note[song.Channels.Length];
            meta.channelVolumes = new int[song.Channels.Length];

            for (int i = 0; i < channelStates.Length; i++)
            {
                meta.channelNotes[i] = channelStates[i].CurrentNote;
                meta.channelVolumes[i] = channelStates[i].CurrentVolume;
            }

            metadata.Add(meta);

            prevNumSamples = numSamples;
        }

        public VideoFrameMetadata[] GetVideoMetadata(Song song, bool pal, int duration)
        {
            int maxSample = int.MaxValue;

            if (duration > 0)
                maxSample = duration * sampleRate;

            if (BeginPlaySong(song, pal, 0))
            {
                WriteMetadata(metadata);

                while (PlaySongFrame() && numSamples < maxSample)
                {
                    WriteMetadata(metadata);
                }
            }

            return metadata.ToArray();
        }

        protected override short[] EndFrame()
        {
            numSamples += base.EndFrame().Length;
            return null;
        }
    }

    static class VideoResolution
    {
        public static readonly string[] Names =
        {
            "1080p",
            "720p",
            "576p",
            "480p"
        };

        public static readonly int[] ResolutionY =
        {
            1080,
            720,
            576,
            480
        };

        public static readonly int[] ResolutionX =
        {
            1920,
            1280,
            1024,
            854
        };

        public static int GetIndexForName(string str)
        {
            return Array.IndexOf(Names, str);
        }
    }

#if FAMISTUDIO_LINUX || FAMISTUDIO_MACOS
    class DummyGLControl : GLControl
    {
    };
#endif
}
