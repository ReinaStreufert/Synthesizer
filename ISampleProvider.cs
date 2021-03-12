using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    interface ISampleProvider
    {
        void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset);
    }
}
