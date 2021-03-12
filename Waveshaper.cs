using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class Waveshaper : ISampleProcessor
    {
        private ISampleProvider input;
        public ISampleProvider Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
            }
        }
        public float Hardness = 3F; // Values below 1F are unpredictable.
        public float Multiplier = 1F; // Multiply input before shaping.
        public void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset)
        {
            input.FillBuffer(SampleBuffer, SampleRate, Offset);
            for (int i = 0; i < SampleBuffer.Length; i++)
            {
                float sample = SampleBuffer[i] * Multiplier;
                if (sample <= 0)
                {
                    SampleBuffer[i] = (float)-Math.Sqrt(1 - Math.Pow(sample + 1, Hardness));
                } else
                {
                    SampleBuffer[i] = (float)Math.Sqrt(1 - Math.Pow(1 - sample, Hardness));
                }
            }
        }
    }
}
