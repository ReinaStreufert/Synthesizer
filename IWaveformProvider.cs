using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    interface IWaveformProvider
    {
        float GetSample(float Frequency, int SampleRate, int Offset, float Modulation);
    }
}
