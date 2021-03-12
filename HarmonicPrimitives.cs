using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    static class HarmonicPrimitives
    {
        public static ISampleProvider[] GenerateSineWaveHarmonics()
        {
            ConstantSampleValue[] harmonics = new ConstantSampleValue[1];
            harmonics[0] = new ConstantSampleValue(1F);
            return harmonics;
        }
        public static ISampleProvider[] GenerateSawWaveHarmonics(int MaxHarmonics)
        {
            ConstantSampleValue[] harmonics = new ConstantSampleValue[MaxHarmonics];
            for (int i = 0; i < harmonics.Length; i++)
            {
                harmonics[i] = new ConstantSampleValue(1F / (i + 1));
            }
            return harmonics;
        }
        public static ISampleProvider[] GenerateSquareWaveHarmonics(int MaxHarmonics)
        {
            ConstantSampleValue[] harmonics = new ConstantSampleValue[MaxHarmonics];
            for (int i = 0; i < harmonics.Length; i++)
            {
                if ((i + 1) % 2 == 1)
                {
                    harmonics[i] = new ConstantSampleValue(1F / (i + 1));
                } else
                {
                    harmonics[i] = new ConstantSampleValue(0);
                }
            }
            return harmonics;
        }
        public static ISampleProvider[] GenerateTriangleWaveHarmonics(int MaxHarmonics)
        {
            ConstantSampleValue[] harmonics = new ConstantSampleValue[MaxHarmonics];
            for (int i = 0; i < harmonics.Length; i++)
            {
                if ((i + 1) % 2 == 1)
                {
                    harmonics[i] = new ConstantSampleValue((float)(1F / (Math.Pow(i + 1, 2))));
                    if ((i + 3) % 4 == 1)
                    {
                        harmonics[i].ConstantValue = -harmonics[i].ConstantValue;
                    }
                }
                else
                {
                    harmonics[i] = new ConstantSampleValue(0);
                }
            }
            return harmonics;
        }
    }
}
