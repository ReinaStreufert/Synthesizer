using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Diagnostics;

namespace Synthesizer
{
    class NAudioFloatWaveProvider : NAudio.Wave.IWaveProvider
    {
        private int position = 0;
        private float[] buf;

        public void Initialize(int SamplesPerBuffer)
        {
            buf = new float[SamplesPerBuffer * 2];
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            }
        }
        public ISampleProvider Provider;

        public int Read(byte[] buffer, int offset, int count)
        {
            Provider.FillBuffer(buf, 44100, position);
            Buffer.BlockCopy(buf, 0, buffer, offset, count);

            position += count / 4;
            return count;
        }
    }
}
