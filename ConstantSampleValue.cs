using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class ConstantSampleValue : ISampleProvider
    {
        public float ConstantValue;
        public ConstantSampleValue(float ConstantValue)
        {
            this.ConstantValue = ConstantValue;
        }
        public void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset)
        {
            for (int i = 0; i < SampleBuffer.Length; i++)
            {
                SampleBuffer[i] = ConstantValue;
            }
        }
    }
}
