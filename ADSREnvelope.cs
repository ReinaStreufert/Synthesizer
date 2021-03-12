using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    class ADSREnvelope : ISampleProvider
    {
        public float InitialValue = 1F;
        public float PeekValue = 1F;
        public float SustainValue = 1F;
        public float EndValue = 0F;
        public float VoidValue = 0F; // After the note has been released and the release time has elapsed, this is the value the rest of the buffer will be filled with.

        public float AttackTime = 0F;
        public float DecayTime = 0F;
        public float ReleaseTime = 0F;

        public CurveType AttackType = CurveType.Linear;
        public float AttackCurveHardness = 0F; // Value of 2F is the most balanced curve (it's actually an arc), the higher the value the harder the curve. Values below 2F are uneven and below 1F are unpredictable.
        public CurveType DecayType = CurveType.FastStart;
        public float DecayCurveHardness = 0F;
        public CurveType ReleaseType = CurveType.SlowStart;
        public float ReleaseCurveHardness = 0F;

        public bool EnvelopeFinished = false;

        private int releaseOffset = -1;

        public void FillBuffer(float[] SampleBuffer, int SampleRate, int Offset)
        {
            int attackStartOffset = 0;
            int attackLength = (int)(AttackTime * SampleRate);
            int decayStartOffset = attackStartOffset + attackLength;
            int decayLength = (int)(DecayTime * SampleRate);
            int sustainStartOffset = decayStartOffset + decayLength;
            int releaseLength = (int)(ReleaseTime * SampleRate);

            int i = 0;
            while (i < SampleBuffer.Length)
            {
                int offsetI = Offset + i;
                int bufferLeft = SampleBuffer.Length - i;
                if (releaseOffset > -1)
                {
                    int voidStart = releaseOffset + (int)(ReleaseTime * SampleRate);
                    if (offsetI >= voidStart)
                    {
                        while (i < SampleBuffer.Length)
                        {
                            SampleBuffer[i] = VoidValue;
                            i++;
                        }
                    } else if (offsetI >= releaseOffset)
                    {
                        int interpProgress = offsetI - releaseOffset;
                        int interpLeft = (releaseOffset + releaseLength) - offsetI;
                        if (ReleaseType == CurveType.Linear)
                        {
                            for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                            {
                                SampleBuffer[i] = clamp(linearFall(releaseLength, interpProgress + ii), EndValue, SustainValue);
                                i++;
                            }
                        } else if (ReleaseType == CurveType.FastStart)
                        {
                            for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                            {
                                SampleBuffer[i] = clamp(fastCurveFall(releaseLength, interpProgress + ii, ReleaseCurveHardness), EndValue, SustainValue);
                                i++;
                            }
                        }
                        else if (ReleaseType == CurveType.SlowStart)
                        {
                            for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                            {
                                SampleBuffer[i] = clamp(slowCurveFall(releaseLength, interpProgress + ii, ReleaseCurveHardness), EndValue, SustainValue);
                                i++;
                            }
                        }
                    }
                } else if (offsetI >= sustainStartOffset)
                {
                    int interpProgress = offsetI - sustainStartOffset;
                    if (releaseOffset > -1)
                    {
                        int interpLeft = (releaseOffset) - offsetI;
                        while (i < Math.Min(SampleBuffer.Length, offsetI + interpLeft + 1))
                        {
                            SampleBuffer[i] = SustainValue;
                            i++;
                        }
                    } else
                    {
                        while (i < SampleBuffer.Length)
                        {
                            SampleBuffer[i] = SustainValue;
                            i++;
                        }
                    }
                } else if (offsetI >= decayStartOffset)
                {
                    int interpProgress = offsetI - decayStartOffset;
                    int interpLeft = (decayStartOffset + decayLength) - offsetI;
                    if (DecayType == CurveType.Linear)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(linearFall(decayLength, interpProgress + ii), SustainValue, PeekValue);
                            i++;
                        }
                    }
                    else if (DecayType == CurveType.FastStart)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(fastCurveFall(decayLength, interpProgress + ii, DecayCurveHardness), SustainValue, PeekValue);
                            i++;
                        }
                    }
                    else if (DecayType == CurveType.SlowStart)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(slowCurveFall(decayLength, interpProgress + ii, DecayCurveHardness), SustainValue, PeekValue);
                            i++;
                        }
                    }
                } else if (offsetI >= attackStartOffset)
                {
                    int interpProgress = offsetI - attackStartOffset;
                    int interpLeft = (attackStartOffset + attackLength) - offsetI;
                    if (AttackType == CurveType.Linear)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(linearRise(attackLength, interpProgress + ii), InitialValue, PeekValue);
                            i++;
                        }
                    }
                    else if (AttackType == CurveType.FastStart)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(fastCurveRise(attackLength, interpProgress + ii, AttackCurveHardness), InitialValue, PeekValue);
                            i++;
                        }
                    }
                    else if (AttackType == CurveType.SlowStart)
                    {
                        for (int ii = 0; ii < Math.Min(interpLeft, bufferLeft); ii++)
                        {
                            SampleBuffer[i] = clamp(slowCurveRise(attackLength, interpProgress + ii, AttackCurveHardness), InitialValue, PeekValue);
                            i++;
                        }
                    }
                }
            }
        }

        public void Release(int Offset)
        {
            releaseOffset = Offset;
        }

        private float fastCurveRise(float curveLength, float time, float hardness)
        {
            return (float)Math.Sqrt(1 - Math.Pow((curveLength - time) / curveLength, hardness));
        }
        private float slowCurveRise(float curveLength, float time, float hardness)
        {
            return -(float)Math.Sqrt(1 - Math.Pow((time) / curveLength, hardness)) + 1;
        }
        private float slowCurveFall(float curveLength, float time, float hardness)
        {
            return (float)Math.Sqrt(1 - Math.Pow((time) / curveLength, hardness));
        }
        private float fastCurveFall(float curveLength, float time, float hardness)
        {
            return -(float)Math.Sqrt(1 - Math.Pow((curveLength - time) / curveLength, hardness)) + 1;
        }
        private float linearRise(float length, float time)
        {
            return time / length;
        }
        private float linearFall(float length, float time)
        {
            return 1 - (time / length);
        }
        private float clamp(float input, float min, float max)
        {
            //input /= 2;
            return (input * (max - min)) + min;
        }
    }
    enum CurveType : byte
    {
        Linear = 0, // Not curved
        SlowStart = 1, // The speed of fall of or rise starts at 0 and ends at a speed of infinity
        FastStart = 2 // The speed of fall of or rise starts at infinity and ends at a speed of 0
    }
}
