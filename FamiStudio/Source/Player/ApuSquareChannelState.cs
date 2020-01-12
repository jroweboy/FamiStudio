﻿using System;

namespace FamiStudio
{
    public class ApuSquareChannelState : ChannelState
    {
        int regOffset = 0;
        int prevPeriodHi = 1000;

        public ApuSquareChannelState(int apuIdx, int channelType) : base(apuIdx, channelType)
        {
            regOffset = channelType * 4;
        }

        public override void UpdateAPU()
        {
            if (note.IsStop)
            {
                NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_VOL + regOffset, (duty << 6) | (0x30) | 0);
            }
            else if (note.IsValid)
            {
                var noteVal = Utils.Clamp(note.Value + envelopeValues[Envelope.Arpeggio], 0, noteTable.Length - 1);
                var period = Utils.Clamp(noteTable[noteVal] + GetSlidePitch() + envelopeValues[Envelope.Pitch], 0, maximumPeriod);
                var volume = MultiplyVolumes(note.Volume, envelopeValues[Envelope.Volume]);

                var periodHi = (period >> 8) & 0x07;
                var periodLo = period & 0xff;
                int deltaHi  = periodHi - prevPeriodHi;

                if (deltaHi != 0) // Avoid resetting the sequence.
                {
                    if (Settings.SquareSmoothVibrato && Math.Abs(deltaHi) == 1 && !IsSeeking)
                    {
                        // Blaarg's smooth vibrato technique using the sweep to avoid resetting the phase. Cool stuff.
                        // http://forums.nesdev.com/viewtopic.php?t=231

                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_FRAME_CNT, 0x40); // reset frame counter in case it was about to clock
                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_LO + regOffset, deltaHi < 0 ? 0x00 : 0xff); // be sure low 8 bits of timer period are $FF ($00 when negative)
                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_SWEEP + regOffset, deltaHi < 0 ? 0x8f : 0x87); // sweep enabled, shift = 7, set negative flag.
                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_FRAME_CNT, 0xc0); // clock sweep immediately
                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_SWEEP + regOffset, 0x08); // disable sweep
                    }
                    else
                    {
                        NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_HI + regOffset, periodHi);
                    }

                    prevPeriodHi = periodHi;
                }

                NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_LO + regOffset, periodLo);
                NesApu.NesApuWriteRegister(apuIdx, NesApu.APU_PL1_VOL + regOffset, (duty << 6) | (0x30) | volume);
            }
        }
    };
}
