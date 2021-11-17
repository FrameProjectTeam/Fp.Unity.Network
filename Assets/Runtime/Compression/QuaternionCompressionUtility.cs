using System;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Network.Compression
{
    /// <summary>
    ///     Compress quaternion (4byte * 4 = 16byte = 128bit) to uint(4 byte = 32bit)
    /// </summary>
    public static class QuaternionCompressionUtility
    {
        private const double SqrtTwo = 1.414213562373;
        private const double Maximum = 1 / SqrtTwo;
        private const double Minimum = -Maximum;
        private const double Range = Maximum - Minimum;

        private const int BitComponentSize = 10;

        private const float Scale = (1 << BitComponentSize) - 1;
        private const float InverseScale = 1f / Scale;
        private const double InverseScaledRange = Range * InverseScale;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Unpack(uint compressed)
        {
            uint largest = compressed >> 30; //last two bit
            uint intA = (compressed >> 20) & 0x3FF;
            uint intB = (compressed >> 10) & 0x3FF;
            uint intC = compressed & 0x3FF;

            double a = intA * InverseScaledRange + Minimum;
            double b = intB * InverseScaledRange + Minimum;
            double c = intC * InverseScaledRange + Minimum;

            float x, y, z, w;

            switch (largest)
            {
                case 0:
                {
                    x = (float) Math.Sqrt(1 - a * a - b * b - c * c);
                    y = (float) a;
                    z = (float) b;
                    w = (float) c;

                    break;
                }
                case 1:
                {
                    x = (float) a;
                    y = (float) Math.Sqrt(1 - a * a - b * b - c * c);
                    z = (float) b;
                    w = (float) c;

                    break;
                }
                case 2:
                {
                    x = (float) a;
                    y = (float) b;
                    z = (float) Math.Sqrt(1 - a * a - b * b - c * c);
                    w = (float) c;

                    break;
                }
                case 3:
                {
                    x = (float) a;
                    y = (float) b;
                    z = (float) c;
                    w = (float) Math.Sqrt(1 - a * a - b * b - c * c);
                    break;
                }
                default:
                {
                    Assert.IsTrue(false);
                    x = 0;
                    y = 0;
                    z = 0;
                    w = 1;

                    break;
                }
            }

            return new Quaternion(x, y, z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PackToUint32(Quaternion quaternion)
        {
            return PackToUint32(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PackToUint32(float x, float y, float z, float w)
        {
            float absX = Math.Abs(x);
            float absY = Math.Abs(y);
            float absZ = Math.Abs(z);
            float absW = Math.Abs(w);

            uint largest = 0;
            float largestValue = absX;

            if (absY > largestValue)
            {
                largest = 1;
                largestValue = absY;
            }

            if (absZ > largestValue)
            {
                largest = 2;
                largestValue = absZ;
            }

            if (absW > largestValue)
            {
                largest = 3;
                //largestValue = absW;
            }

            float a = 0;
            float b = 0;
            float c = 0;

            switch (largest)
            {
                case 0:
                    if (x >= 0)
                    {
                        a = y;
                        b = z;
                        c = w;
                    }
                    else
                    {
                        a = -y;
                        b = -z;
                        c = -w;
                    }

                    break;

                case 1:
                    if (y >= 0)
                    {
                        a = x;
                        b = z;
                        c = w;
                    }
                    else
                    {
                        a = -x;
                        b = -z;
                        c = -w;
                    }

                    break;

                case 2:
                    if (z >= 0)
                    {
                        a = x;
                        b = y;
                        c = w;
                    }
                    else
                    {
                        a = -x;
                        b = -y;
                        c = -w;
                    }

                    break;

                case 3:
                    if (w >= 0)
                    {
                        a = x;
                        b = y;
                        c = z;
                    }
                    else
                    {
                        a = -x;
                        b = -y;
                        c = -z;
                    }

                    break;
                default:
                    Assert.IsTrue(false);
                    break;
            }

            double normalA = (a - Minimum) / (Maximum - Minimum);
            double normalB = (b - Minimum) / (Maximum - Minimum);
            double normalC = (c - Minimum) / (Maximum - Minimum);

            var integerA = (uint) Math.Floor(normalA * Scale + 0.5f);
            var integerB = (uint) Math.Floor(normalB * Scale + 0.5f);
            var integerC = (uint) Math.Floor(normalC * Scale + 0.5f);

            return (largest << 30) | (integerA << 20) | (integerB << 10) | integerC;
        }
    }
}