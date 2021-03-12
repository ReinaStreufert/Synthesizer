using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class LowpassFilter : ISampleProcessor
    {
        public float CutoffFrequency;
        private ISampleProvider input;
        private float lastOutput = 0;
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

        public void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset)
        {
            input.FillBuffer(SampleBuffer, SampleRate, Offset);
            if (Offset == 0)
            {
                lastOutput = SampleBuffer[0];
            }
            float RC = 1F / (CutoffFrequency * 2 * (float)Math.PI);
            float dt = 1F / SampleRate;
            float alpha = dt / (RC + dt);
            for (int i = 0; i < SampleBuffer.Length; i++)
            {
                lastOutput = lastOutput + (alpha * (SampleBuffer[i] - lastOutput));
                SampleBuffer[i] = lastOutput;
            }
        }
    }
}
