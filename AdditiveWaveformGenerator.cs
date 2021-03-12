using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class AdditiveWaveformGenerator : IWaveformProvider
    {
        public ISampleProvider[] Harmonics;

        private float[] fixedBuffer = new float[1];
        private float offset = 0F;

        public void ResetOffset()
        {
            offset = 0F;
        }

        public float GetSample(float Frequency, int SampleRate, int Offset, float Modulation)
        {
            //Console.WriteLine(Frequency);
            float SineSample = ((offset / Frequency) * Frequency * (((float)Math.PI * 2) / SampleRate)) + Modulation;
            float result = 0;
            for (int i = 0; i < Harmonics.Length; i++)
            {
                Harmonics[i].FillBuffer(fixedBuffer, SampleRate, Offset);
                if (fixedBuffer[0] > 0)
                {
                    result += (float)Math.Sin(SineSample * (i + 1)) * fixedBuffer[0];
                }
            }
            offset += Frequency;
            return result;
        }
    }
}
