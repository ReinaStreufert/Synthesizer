using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class Oscillator : ISampleProvider
    {
        public ISampleProvider AmplitudeSource;
        public ISampleProvider PitchSource;
        public ISampleProvider[] PitchModulators;
        public IWaveformProvider WaveformSource;
        public float PitchMultiplier;
        public float Volume = 1F;
        public float LVolume = 1F; // Only valid for carrier oscillators
        public float RVolume = 1F; // Only valid for carrier oscillators
        public bool Carrier = true;

        private int bufferSize = 0;

        private float[] amplitudeBuffer;
        private float[] pitchBuffer;
        private float[][] pitchModulatorBuffers;

        public void AllocateBuffers(int BufferSize)
        {
            bufferSize = BufferSize;
            if (Carrier)
            {
                BufferSize /= 2;
            }
            amplitudeBuffer = new float[BufferSize];
            pitchBuffer = new float[BufferSize];
            pitchModulatorBuffers = new float[PitchModulators.Length][];
            for (int i = 0; i < PitchModulators.Length; i++)
            {
                pitchModulatorBuffers[i] = new float[BufferSize];
            }
        }
        public void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset)
        {
            if (SampleBuffer.Length != bufferSize)
            {
                throw new InvalidOperationException("Unexpected buffer size.");
            }
            if (Carrier)
            {
                AmplitudeSource.FillBuffer(amplitudeBuffer, SampleRate, Offset / 2);
                PitchSource.FillBuffer(pitchBuffer, SampleRate, Offset / 2);
                for (int ii = 0; ii < PitchModulators.Length; ii++)
                {
                    PitchModulators[ii].FillBuffer(pitchModulatorBuffers[ii], SampleRate, Offset / 2);
                }
                for (int i = 0; i < SampleBuffer.Length; i += 2)
                {
                    float frequency = pitchBuffer[i / 2] * PitchMultiplier;
                    //Console.WriteLine(frequency);
                    /*if (frequency < 110)
                    {
                        Console.WriteLine("bruh");
                    }*/
                    float modulation = 0F;
                    for (int ii = 0; ii < PitchModulators.Length; ii++)
                    {
                        modulation += pitchModulatorBuffers[ii][i / 2];
                    }

                    float sample = WaveformSource.GetSample(frequency, SampleRate, (Offset / 2) + (i / 2), modulation) * amplitudeBuffer[i / 2] * Volume;
                    SampleBuffer[i] = sample * LVolume;
                    SampleBuffer[i + 1] = sample * RVolume;
                }
            } else
            {
                AmplitudeSource.FillBuffer(amplitudeBuffer, SampleRate, Offset);
                PitchSource.FillBuffer(pitchBuffer, SampleRate, Offset);
                for (int ii = 0; ii < PitchModulators.Length; ii++)
                {
                    PitchModulators[ii].FillBuffer(pitchModulatorBuffers[ii], SampleRate, Offset);
                }
                for (int i = 0; i < SampleBuffer.Length; i++)
                {
                    float frequency = pitchBuffer[i] * PitchMultiplier;
                    float modulation = 0F;
                    for (int ii = 0; ii < PitchModulators.Length; ii++)
                    {
                        modulation += pitchModulatorBuffers[ii][i];
                    }

                    float sample = WaveformSource.GetSample(frequency, SampleRate, Offset + i, modulation) * amplitudeBuffer[i] * Volume;
                    SampleBuffer[i] = sample;
                }
            }
        }
    }
}
