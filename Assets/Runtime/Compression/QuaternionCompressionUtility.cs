using System;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Network.Compression
{
	public static class QuaternionCompressionUtility
	{
		private const double SqrtTwo = 1.4142135623731;
		private const double Maximum = 1 / SqrtTwo;
		private const double Minimum = -Maximum;
		private const double Range = Maximum - Minimum;

		private const int BitComponentSize_32 = 10;

		private const int Scale_32 = (1 << BitComponentSize_32) - 1;
		private const float InverseScale_32 = 1f / Scale_32;
		private const double InverseScaledRange_32 = Range * InverseScale_32;

		private const int BitComponentSize_64 = 20;
		private const int Scale_64 = (1 << BitComponentSize_64) - 1;
		private const float InverseScale_64 = 1f / Scale_64;
		private const double InverseScaledRange_64 = Range * InverseScale_64;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Unpack(uint compressed)
		{
			uint largest = compressed >> 30; //last two bit
			uint intA = (compressed >> 20) & Scale_32;
			uint intB = (compressed >> 10) & Scale_32;
			uint intC = compressed & Scale_32;

			double a = intA * InverseScaledRange_32 + Minimum;
			double b = intB * InverseScaledRange_32 + Minimum;
			double c = intC * InverseScaledRange_32 + Minimum;

			float x = 0;
			float y = 0;
			float z = 0;
			float w = 0;

			switch (largest)
			{
				case 0:
				{
					x = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					y = (float)a;
					z = (float)b;
					w = (float)c;

					break;
				}
				case 1:
				{
					x = (float)a;
					y = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					z = (float)b;
					w = (float)c;

					break;
				}
				case 2:
				{
					x = (float)a;
					y = (float)b;
					z = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					w = (float)c;

					break;
				}
				case 3:
				{
					x = (float)a;
					y = (float)b;
					z = (float)c;
					w = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					break;
				}
			}

			return new Quaternion(x, y, z, w);
		}

		/// <summary>
		///     Compress quaternion (4byte * 4 = 16byte = 128bit) to uint(4 byte = 32bit)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint PackToUint32(Quaternion quaternion)
		{
			return PackToUint32(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		/// <summary>
		///     Compress quaternion (4byte * 4 = 16byte = 128bit) to uint(4 byte = 32bit)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint PackToUint32(float x, float y, float z, float w)
		{
			Assert.IsTrue(Mathf.Approximately(x + y + z + w, 1f));

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
			}

			float a = 0;
			float b = 0;
			float c = 0;

			switch (largest)
			{
				case 0:
				{
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
				}
				case 1:
				{
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
				}
				case 2:
				{
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
				}
				case 3:
				{
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
				}
			}

			double normalA = (a - Minimum) / (Maximum - Minimum);
			double normalB = (b - Minimum) / (Maximum - Minimum);
			double normalC = (c - Minimum) / (Maximum - Minimum);

			var integerA = (uint)Math.Floor(normalA * Scale_32 + 0.5f);
			var integerB = (uint)Math.Floor(normalB * Scale_32 + 0.5f);
			var integerC = (uint)Math.Floor(normalC * Scale_32 + 0.5f);

			return (largest << 30) | (integerA << 20) | (integerB << 10) | integerC;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Unpack(ulong compressed)
		{
			ulong largest = compressed >> 60; //last four bit, two's is empty
			ulong intA = (compressed >> 40) & Scale_64;
			ulong intB = (compressed >> 20) & Scale_64;
			ulong intC = compressed & Scale_64;

			double a = intA * InverseScaledRange_64 + Minimum;
			double b = intB * InverseScaledRange_64 + Minimum;
			double c = intC * InverseScaledRange_64 + Minimum;

			float x = 0;
			float y = 0;
			float z = 0;
			float w = 0;

			switch (largest)
			{
				case 0:
				{
					x = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					y = (float)a;
					z = (float)b;
					w = (float)c;

					break;
				}
				case 1:
				{
					x = (float)a;
					y = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					z = (float)b;
					w = (float)c;

					break;
				}
				case 2:
				{
					x = (float)a;
					y = (float)b;
					z = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					w = (float)c;

					break;
				}
				case 3:
				{
					x = (float)a;
					y = (float)b;
					z = (float)c;
					w = (float)Math.Sqrt(1 - a * a - b * b - c * c);
					break;
				}
			}

			return new Quaternion(x, y, z, w);
		}

		/// <summary>
		///     Compress quaternion (4byte * 4 = 16byte = 128bit) to uint(4 byte = 64bit)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong PackToUint64(Quaternion quaternion)
		{
			return PackToUint64(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		/// <summary>
		///     Compress quaternion (4byte * 4 = 16byte = 128bit) to uint(4 byte = 64bit)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong PackToUint64(float x, float y, float z, float w)
		{
			Assert.IsTrue(Mathf.Approximately(x + y + z + w, 1f));

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
				{
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
				}
				case 1:
				{
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
				}
				case 2:
				{
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
				}

				case 3:
				{
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
				}
			}

			double normalA = (a - Minimum) / (Maximum - Minimum);
			double normalB = (b - Minimum) / (Maximum - Minimum);
			double normalC = (c - Minimum) / (Maximum - Minimum);

			var integerA = (ulong)Math.Floor(normalA * Scale_64 + 0.5f);
			var integerB = (ulong)Math.Floor(normalB * Scale_64 + 0.5f);
			var integerC = (ulong)Math.Floor(normalC * Scale_64 + 0.5f);

			//TODO: Ignored 2bit, should use for two values but ignore third ?
			return (largest << 60) | (integerA << 40) | (integerB << 20) | integerC;
		}
	}
}