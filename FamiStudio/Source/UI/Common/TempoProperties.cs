﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FamiStudio
{
    class TempoProperties
    {
        private PropertyPage props;
        private Song song;

        private int patternIdx    = -1;
        private int minPatternIdx = -1;
        private int maxPatternIdx = -1;

        int firstPropIdx             = -1;
        int famitrackerTempoPropIdx  = -1;
        int famitrackerSpeedPropIdx  = -1;
        int notesPerBeatPropIdx      = -1;
        int notesPerPatternPropIdx   = -1;
        int bpmLabelPropIdx          = -1;
        int famistudioBpmPropIdx     = -1;
        int framesPerNotePropIdx     = -1;
        int groovePropIdx            = -1;
        int groovePadPropIdx         = -1;

        int originalNoteLength;
        int originalNotesPerBeat;

        TempoInfo[] tempoList;
        string[]    tempoStrings;
        string[]    grooveStrings;

        public TempoProperties(PropertyPage props, Song song, int patternIdx = -1, int minPatternIdx = -1, int maxPatternIdx = -1)
        {
            this.song = song;
            this.props = props;
            this.patternIdx = patternIdx;
            this.minPatternIdx = minPatternIdx;
            this.maxPatternIdx = maxPatternIdx;

            props.PropertyChanged += Props_PropertyChanged;
        }

        public void AddProperties()
        {
            firstPropIdx = props.PropertyCount;

            if (song.UsesFamiTrackerTempo)
            {
                if (patternIdx < 0)
                {
                    famitrackerTempoPropIdx = props.AddIntegerRange("Tempo :", song.FamitrackerTempo, 32, 255, CommonTooltips.Tempo); // 0
                    famitrackerSpeedPropIdx = props.AddIntegerRange("Speed :", song.FamitrackerSpeed, 1, 31, CommonTooltips.Speed); // 1
                }
                
                var notesPerBeat    = patternIdx < 0 ? song.BeatLength    : song.GetPatternBeatLength(patternIdx);
                var notesPerPattern = patternIdx < 0 ? song.PatternLength : song.GetPatternLength(patternIdx);
                var bpm = Song.ComputeFamiTrackerBPM(song.Project.PalMode, song.FamitrackerSpeed, song.FamitrackerTempo, notesPerBeat);

                notesPerBeatPropIdx    = props.AddIntegerRange("Notes per Beat :", notesPerBeat, 1, 256, CommonTooltips.NotesPerBeat); // 2
                notesPerPatternPropIdx = props.AddIntegerRange("Notes per Pattern :", notesPerPattern, 1, Pattern.MaxLength, CommonTooltips.NotesPerPattern); // 3
                bpmLabelPropIdx        = props.AddLabel("BPM :", bpm.ToString("n1"), false, CommonTooltips.BPM); // 4

                props.ShowWarnings = true;

                UpdateWarnings();
            }
            else
            {                                                              
                var noteLength      = (patternIdx < 0 ? song.NoteLength    : song.GetPatternNoteLength(patternIdx));
                var notesPerBeat    = (patternIdx < 0 ? song.BeatLength    : song.GetPatternBeatLength(patternIdx));
                var notesPerPattern = (patternIdx < 0 ? song.PatternLength : song.GetPatternLength(patternIdx));
                var groove          = (patternIdx < 0 ? song.Groove        : song.GetPatternGroove(patternIdx));

                tempoList = FamiStudioTempoUtils.GetAvailableTempos(song.Project.PalMode, notesPerBeat / noteLength);
                var tempoIndex = FamiStudioTempoUtils.FindTempoFromGroove(tempoList, groove);
                Debug.Assert(tempoIndex >= 0);
                tempoStrings = tempoList.Select(t => t.bpm.ToString("n1") + (t.groove.Length == 1 ? " *": "")).ToArray();

                var grooveList = FamiStudioTempoUtils.GetAvailableGrooves(tempoList[tempoIndex].groove);
                var grooveIndex = Array.FindIndex(grooveList, g => Utils.CompareArrays(g, groove) == 0);
                Debug.Assert(grooveIndex >= 0);
                grooveStrings = grooveList.Select(g => string.Join("-", g)).ToArray();

                famistudioBpmPropIdx   = props.AddDropDownList("BPM : ", tempoStrings, tempoStrings[tempoIndex], CommonTooltips.BPM); // 0
                notesPerBeatPropIdx    = props.AddIntegerRange("Notes per Beat : ", notesPerBeat / noteLength, 1, 256, CommonTooltips.NotesPerBeat); // 1
                notesPerPatternPropIdx = props.AddIntegerRange("Notes per Pattern : ", notesPerPattern / noteLength, 1, Pattern.MaxLength / noteLength, CommonTooltips.NotesPerPattern); // 2
                framesPerNotePropIdx   = props.AddLabel("Frames per Note :", noteLength.ToString(), false, CommonTooltips.FramesPerNote); // 3

                props.ShowWarnings = true;
                props.BeginAdvancedProperties();
                groovePropIdx    = props.AddDropDownList("Groove : ", grooveStrings, grooveStrings[grooveIndex], CommonTooltips.Groove); // 4
                groovePadPropIdx = props.AddDropDownList("Groove Padding : ", GroovePaddingType.Names, GroovePaddingType.Names[song.GroovePaddingMode], CommonTooltips.GroovePadding); // 5

                originalNoteLength      = noteLength;
                originalNotesPerBeat    = notesPerBeat;

                UpdateWarnings();
            }
        }

        private void Props_PropertyChanged(PropertyPage props, int idx, object value)
        {
            if (song.UsesFamiTrackerTempo)
            {
                var tempo = song.FamitrackerTempo;
                var speed = song.FamitrackerSpeed;

                if (idx == famitrackerTempoPropIdx ||
                    idx == famitrackerSpeedPropIdx)
                {
                    tempo = props.GetPropertyValue<int>(famitrackerTempoPropIdx);
                    speed = props.GetPropertyValue<int>(famitrackerSpeedPropIdx);
                }

                var beatLength = props.GetPropertyValue<int>(notesPerBeatPropIdx);

                props.SetLabelText(bpmLabelPropIdx, Song.ComputeFamiTrackerBPM(song.Project.PalMode, speed, tempo, beatLength).ToString("n1"));
            }
            else
            {
                var notesPerBeat = props.GetPropertyValue<int>(notesPerBeatPropIdx);

                // Changing the number of notes in a beat will affect the list of available BPMs.
                if (idx == notesPerBeatPropIdx)
                {
                    tempoList = FamiStudioTempoUtils.GetAvailableTempos(song.Project.PalMode, notesPerBeat);
                    tempoStrings = tempoList.Select(t => t.bpm.ToString("n1") + (t.groove.Length == 1 ? " *" : "")).ToArray();
                    props.UpdateDropDownListItems(famistudioBpmPropIdx, tempoStrings);
                }

                // Changing the BPM affects the grooves and note length.
                if (idx == famistudioBpmPropIdx ||
                    idx == notesPerBeatPropIdx)
                {
                    var tempoIndex    = Array.IndexOf(tempoStrings, props.GetPropertyValue<string>(famistudioBpmPropIdx));
                    var tempoInfo     = tempoList[tempoIndex];
                    var framesPerNote = Utils.Min(tempoInfo.groove);

                    props.UpdateIntegerRange(notesPerPatternPropIdx, 1, Pattern.MaxLength / framesPerNote);

                    var grooveList = FamiStudioTempoUtils.GetAvailableGrooves(tempoInfo.groove);
                    grooveStrings = grooveList.Select(g => string.Join("-", g)).ToArray();

                    props.UpdateDropDownListItems(groovePropIdx, grooveStrings);
                    props.SetLabelText(framesPerNotePropIdx, framesPerNote.ToString());
                }
            }

            UpdateWarnings();
        }

        private void UpdateWarnings()
        {
            var numFramesPerPattern = 0;

            if (song.UsesFamiStudioTempo)
            {
                var tempoIndex = Array.IndexOf(tempoStrings, props.GetPropertyValue<string>(famistudioBpmPropIdx));
                var tempoInfo = tempoList[tempoIndex];
                var notesPerBeat = props.GetPropertyValue<int>(notesPerBeatPropIdx);
                var notesPerPattern = props.GetPropertyValue<int>(notesPerPatternPropIdx);

                if (tempoInfo.groove.Length == 1)
                {
                    props.SetPropertyWarning(famistudioBpmPropIdx, CommentType.Good, "Ideal tempo : notes will be perfectly evenly divided.");
                }
                else if ((tempoInfo.groove.Length % notesPerBeat) == 0 ||
                         (notesPerBeat % tempoInfo.groove.Length) == 0)
                {
                    props.SetPropertyWarning(famistudioBpmPropIdx, CommentType.Warning, "Beat-aligned groove : notes will be slightly uneven, but well aligned with the beat.");
                }
                else
                {
                    props.SetPropertyWarning(famistudioBpmPropIdx, CommentType.Error, "Unaligned groove : notes will be slightly uneven and not aligned to the beat.");
                }

                if (notesPerBeat != 4)
                {
                    props.SetPropertyWarning(notesPerBeatPropIdx, CommentType.Error, "A value of 4 is strongly recommended as it gives the best range of available BPMs.");
                }
                else
                {
                    props.SetPropertyWarning(notesPerBeatPropIdx, CommentType.Good, "4 is the recommended value.");
                }

                var groovePadMode = GroovePaddingType.GetValueForName(props.GetPropertyValue<string>(groovePadPropIdx));
                numFramesPerPattern  = FamiStudioTempoUtils.ComputeNumberOfFrameForGroove(notesPerPattern * Utils.Min(tempoInfo.groove), tempoInfo.groove, groovePadMode);
            }
            else if (famitrackerSpeedPropIdx >= 0)
            {
                var speed = props.GetPropertyValue<int>(famitrackerSpeedPropIdx);
                var tempo = props.GetPropertyValue<int>(famitrackerTempoPropIdx);

                if (speed == 1)
                {
                    props.SetPropertyWarning(famitrackerSpeedPropIdx, CommentType.Warning, $"A speed of 1 will not produce the same BPM between platforms (PAL/NTSC).");
                }
                else
                {
                    props.SetPropertyWarning(famitrackerSpeedPropIdx, CommentType.Good, "");
                }

                if (tempo != 150)
                {
                    props.SetPropertyWarning(famitrackerTempoPropIdx, CommentType.Warning, "A tempo of 150 is strongly recommended as it produces even notes on all platforms (NTSC/PAL).");
                }
                else
                {
                    props.SetPropertyWarning(famitrackerTempoPropIdx, CommentType.Good, "150 is the recommended value.");
                }
            }

            if (patternIdx >= 0 && numFramesPerPattern > song.PatternLength)
            {
                props.SetPropertyWarning(notesPerPatternPropIdx, CommentType.Warning, $"Pattern is longer than the song pattern length and FamiTracker does not support this.\nIgnore this if you are not planning to export to FamiTracker.");
            }
            else if (numFramesPerPattern >= 256)
            {
                props.SetPropertyWarning(notesPerPatternPropIdx, CommentType.Warning, $"Pattern is longer than what FamiTracker supports.\nIgnore this if you are not planning to export to FamiTracker.");
            }
            else
            {
                props.SetPropertyWarning(notesPerPatternPropIdx, CommentType.Good, "");
            }
        }

        public void EnableProperties(bool enabled)
        {
            for (var i = firstPropIdx; i < props.PropertyCount; i++)
                props.SetPropertyEnabled(i, enabled);
        }

        private bool ShowConvertTempoDialog()
        {
            var messageDlg = new PropertyDialog(400, true, false);
            messageDlg.Properties.AddLabel(null, "You changed the BPM enough so that the number of frames in a note has changed.", true); // 0
            messageDlg.Properties.AddRadioButton(null, "Resize notes to reflect the new BPM. This is the most sensible option if you just want to change the tempo of the song.", true); // 1
            messageDlg.Properties.AddRadioButton(null, "Leave the notes exactly where they are, just move the grid lines around the notes. This option is useful if you want to change how the notes are grouped.", false); // 2
            messageDlg.Properties.Build();
            messageDlg.ShowDialog(null);

            return messageDlg.Properties.GetPropertyValue<bool>(1);
        }

        public void Apply(bool custom = false)
        {
            if (song.UsesFamiTrackerTempo)
            {
                if (patternIdx == -1)
                {
                    if (famitrackerTempoPropIdx >= 0)
                    {
                        song.FamitrackerTempo = props.GetPropertyValue<int>(famitrackerTempoPropIdx);
                        song.FamitrackerSpeed = props.GetPropertyValue<int>(famitrackerSpeedPropIdx);
                    }

                    song.SetBeatLength(props.GetPropertyValue<int>(notesPerBeatPropIdx));
                    song.SetDefaultPatternLength(props.GetPropertyValue<int>(notesPerPatternPropIdx));
                }
                else
                {
                    for (int i = minPatternIdx; i <= maxPatternIdx; i++)
                    {
                        var beatLength    = props.GetPropertyValue<int>(notesPerBeatPropIdx);
                        var patternLength = props.GetPropertyValue<int>(notesPerPatternPropIdx);

                        if (custom)
                            song.SetPatternCustomSettings(i, patternLength, beatLength);
                        else
                            song.ClearPatternCustomSettings(i);
                    }
                }
            }
            else
            {
                var tempoIndex    = Array.IndexOf(tempoStrings, props.GetPropertyValue<string>(famistudioBpmPropIdx));
                var tempoInfo     = tempoList[tempoIndex];

                var beatLength    = props.GetPropertyValue<int>(notesPerBeatPropIdx);
                var patternLength = props.GetPropertyValue<int>(notesPerPatternPropIdx);
                var noteLength    = Utils.Min(tempoInfo.groove);

                var grooveIndex   = Array.IndexOf(grooveStrings, props.GetPropertyValue<string>(groovePropIdx));
                var groovePadMode = GroovePaddingType.GetValueForName(props.GetPropertyValue<string>(groovePadPropIdx));
                var grooveList    = FamiStudioTempoUtils.GetAvailableGrooves(tempoInfo.groove);
                var groove        = grooveList[grooveIndex];

                props.UpdateIntegerRange(notesPerPatternPropIdx, 1, Pattern.MaxLength / noteLength);
                props.SetLabelText(framesPerNotePropIdx, noteLength.ToString());

                if (patternIdx == -1)
                {
                    var convertTempo = false;

                    if (noteLength != originalNoteLength)
                        convertTempo = ShowConvertTempoDialog();

                    song.ChangeFamiStudioTempoGroove(groove, convertTempo);
                    song.SetBeatLength(beatLength * song.NoteLength);
                    song.SetDefaultPatternLength(patternLength * song.NoteLength);
                    song.SetGroovePaddingMode(groovePadMode);
                }
                else
                {
                    var actualNoteLength    = song.NoteLength;
                    var actualPatternLength = song.PatternLength;
                    var actualBeatLength    = song.BeatLength;

                    if (custom)
                    {
                        actualNoteLength = noteLength;
                        actualBeatLength = beatLength * noteLength;
                        actualPatternLength = patternLength * noteLength;
                    }

                    var patternsToResize = new List<int>();
                    for (int i = minPatternIdx; i <= maxPatternIdx; i++)
                    {
                        if (actualNoteLength != song.GetPatternNoteLength(patternIdx))
                            patternsToResize.Add(i);
                    }

                    if (patternsToResize.Count > 0)
                    {
                        if (ShowConvertTempoDialog())
                        {
                            foreach (var p in patternsToResize)
                                song.ResizePatternNotes(p, actualNoteLength);
                        }
                    }

                    for (int i = minPatternIdx; i <= maxPatternIdx; i++)
                    {
                        if (custom)
                            song.SetPatternCustomSettings(i, actualPatternLength, actualBeatLength, groove, groovePadMode);
                        else
                            song.ClearPatternCustomSettings(i);
                    }
                }
            }

            song.DeleteNotesPastMaxInstanceLength();
            song.InvalidateCumulativePatternCache();
            song.Project.Validate();
        }
    }
}
