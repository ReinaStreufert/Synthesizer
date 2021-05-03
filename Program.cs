using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synthesizer
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            //float[] buffer = new float[100];
            AdditiveWaveformGenerator wave = new AdditiveWaveformGenerator();
            AdditiveWaveformGenerator modWave = new AdditiveWaveformGenerator();
            AdditiveWaveformGenerator modWave2 = new AdditiveWaveformGenerator();
            wave.Harmonics = HarmonicPrimitives.GenerateSquareWaveHarmonics(64);
            modWave.Harmonics = HarmonicPrimitives.GenerateSawWaveHarmonics(64);
            modWave2.Harmonics = HarmonicPrimitives.GenerateSquareWaveHarmonics(32);

            ADSREnvelope pitchEnvelope = new ADSREnvelope();
            pitchEnvelope.InitialValue = 0;//NoteUtils.GetMidiNoteFrequency(46 + 36);
            pitchEnvelope.PeekValue = 0;//NoteUtils.GetMidiNoteFrequency(46 + 36);
            pitchEnvelope.AttackTime = 0F;
            pitchEnvelope.AttackType = CurveType.Linear;
            pitchEnvelope.AttackCurveHardness = 2F;
            pitchEnvelope.DecayTime = 0.095F;
            pitchEnvelope.DecayType = CurveType.FastStart;
            pitchEnvelope.DecayCurveHardness = 4F;
            pitchEnvelope.EndValue = 110F;
            pitchEnvelope.VoidValue = 110F;
            pitchEnvelope.ReleaseTime = 0F;
            pitchEnvelope.ReleaseType = CurveType.SlowStart;
            pitchEnvelope.ReleaseCurveHardness = 2F;
            pitchEnvelope.SustainValue = 0;//NoteUtils.GetMidiNoteFrequency(46);

            Oscillator osc3 = new Oscillator();
            osc3.PitchSource = pitchEnvelope;
            osc3.PitchModulators = new ISampleProvider[0];
            osc3.PitchMultiplier = 0.5F;
            osc3.Volume = 1F;
            osc3.AmplitudeSource = new ConstantSampleValue(1F);
            osc3.WaveformSource = modWave2;
            osc3.Carrier = false;

            Oscillator osc2 = new Oscillator();
            osc2.PitchSource = pitchEnvelope;
            osc2.PitchModulators = new ISampleProvider[0];
            osc2.PitchMultiplier = 0.5F;
            osc2.Volume = 1F;
            osc2.AmplitudeSource = new ConstantSampleValue(1F);
            osc2.WaveformSource = modWave;
            osc2.Carrier = false;

            ADSREnvelope volumeEnvelope = new ADSREnvelope();
            volumeEnvelope.InitialValue = 0F;
            volumeEnvelope.PeekValue = 1F;
            volumeEnvelope.AttackTime = 0F;
            volumeEnvelope.AttackType = CurveType.Linear;
            volumeEnvelope.AttackCurveHardness = 2F;
            volumeEnvelope.DecayTime = 1F;
            volumeEnvelope.DecayType = CurveType.FastStart;
            volumeEnvelope.DecayCurveHardness = 4F;
            volumeEnvelope.SustainValue = 0F;
            volumeEnvelope.EndValue = 0F;
            volumeEnvelope.VoidValue = 0F;
            volumeEnvelope.ReleaseTime = 0F;
            volumeEnvelope.ReleaseType = CurveType.SlowStart;
            volumeEnvelope.ReleaseCurveHardness = 2F;

            Oscillator osc = new Oscillator();
            int note = 57;
            osc.PitchSource = pitchEnvelope;
            osc.PitchModulators = new ISampleProvider[1] { osc2 };
            //osc.PitchModulators = new ISampleProvider[0];
            osc.PitchMultiplier = 1F;
            osc.Volume = 1F;
            osc.AmplitudeSource = volumeEnvelope;
            osc.WaveformSource = wave;

            Waveshaper shaper = new Waveshaper();
            shaper.Input = osc;
            shaper.Hardness = 5F;

            LowpassFilter filter = new LowpassFilter();
            filter.Input = shaper;
            filter.CutoffFrequency = 1770;

            StandardOutPlayer player = new StandardOutPlayer();
            player.Input = shaper;

            int samplesPerBufferSize = 2205;

            osc2.AllocateBuffers(samplesPerBufferSize);
            osc3.AllocateBuffers(samplesPerBufferSize);
            osc.AllocateBuffers(samplesPerBufferSize * 2);
            player.Initialize(samplesPerBufferSize);

            player.StartPlayback();

            Stream stdin = Console.OpenStandardInput();
            while (true)
            {
                byte midiNote = (byte)stdin.ReadByte();
                pitchEnvelope.InitialValue = NoteUtils.GetMidiNoteFrequency(midiNote + 36);
                pitchEnvelope.PeekValue = NoteUtils.GetMidiNoteFrequency(midiNote + 36);
                pitchEnvelope.SustainValue = NoteUtils.GetMidiNoteFrequency(midiNote);
                pitchEnvelope.EnvelopeFinished = false;
                player.ResetOffset();
                wave.ResetOffset();
                modWave.ResetOffset();
            }
        }
    }
}
