using System;

using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Network.Compression
{
	public static class VectorCompression
	{
		private const uint SingMaskX_32 = (uint)1 << 31;
		private const uint SingMaskY_32 = 1 << 30;
		private const uint SingMaskZ_32 = 1 << 29;

		private const uint SignMask_32 = SingMaskX_32 | SingMaskY_32 | SingMaskZ_32;

		private const int Splitter_32 = 15;
		private const uint TopMask_32 = uint.MaxValue ^ (SignMask_32 | BottomMask_32);
		private const uint BottomMask_32 = (1 << Splitter_32) - 1;

		private const ushort SingMaskX_16 = 1 << 15;
		private const ushort SingMaskY_16 = 1 << 14;
		private const ushort SingMaskZ_16 = 1 << 13;

		private const ushort SignMask_16 = SingMaskX_16 | SingMaskY_16 | SingMaskZ_16;

		private const int Splitter_16 = 7;
		private const ushort TopMask_16 = ushort.MaxValue ^ (SignMask_16 | BottomMask_16);
		private const ushort BottomMask_16 = (1 << Splitter_16) - 1;

		public static uint PackDirectionToUInt32(Vector3 dir)
		{
			Assert.IsTrue(Mathf.Approximately(dir.magnitude, 1), "Vector must be unit length");

			uint res = 0;

			if (dir.x < 0)
			{
				res |= SingMaskX_32;
				dir.x = -dir.x;
			}

			if (dir.y < 0)
			{
				res |= SingMaskY_32;
				dir.y = -dir.y;
			}

			if (dir.z < 0)
			{
				res |= SingMaskZ_32;
				dir.z = -dir.z;
			}

			double w = 32766.0 / (dir.x + dir.y + dir.z);

			var xBits = (uint)(dir.x * w);
			var yBits = (uint)(dir.y * w);

			Assert.IsTrue(xBits < 32767);
			Assert.IsTrue(yBits < 32767);

			if (xBits >= 16384)
			{
				xBits = 32767 - xBits;
				yBits = 32767 - yBits;
			}

			res |= xBits << Splitter_32;
			res |= yBits;

			return res;
		}

		public static Vector3 UnpackDirection(uint compressed)
		{
			uint x = (compressed & TopMask_32) >> Splitter_32;
			uint y = compressed & BottomMask_32;

			if (x + y >= 32767)
			{
				x = 32767 - x;
				y = 32767 - y;
			}

			uint z = 32766 - x - y;

			double normalizer = 1.0 / Math.Sqrt(x * x + y * y + z * z);

			var vector = new Vector3((float)(x * normalizer), (float)(y * normalizer), (float)(z * normalizer));

			if ((compressed & SingMaskX_32) != 0)
			{
				vector.x = -vector.x;
			}

			if ((compressed & SingMaskY_32) != 0)
			{
				vector.y = -vector.y;
			}

			if ((compressed & SingMaskZ_32) != 0)
			{
				vector.z = -vector.z;
			}

			return vector;
		}

		public static ushort PackDirectionToUInt16(Vector3 dir)
		{
			Assert.IsTrue(Mathf.Approximately(dir.magnitude, 1), "Vector must be unit length");

			ushort res = 0;

			if (dir.x < 0)
			{
				res |= SingMaskX_16;
				dir.x = -dir.x;
			}

			if (dir.y < 0)
			{
				res |= SingMaskY_16;
				dir.y = -dir.y;
			}

			if (dir.z < 0)
			{
				res |= SingMaskZ_16;
				dir.z = -dir.z;
			}

			float w = 126f / (dir.x + dir.y + dir.z);

			var xBits = (ushort)(dir.x * w);
			var yBits = (ushort)(dir.y * w);

			Assert.IsTrue(xBits < 127);
			Assert.IsTrue(yBits < 127);

			if (xBits >= 64)
			{
				xBits = (ushort)(127 - xBits);
				yBits = (ushort)(127 - yBits);
			}

			res |= (ushort)(xBits << Splitter_16);
			res |= yBits;

			return res;
		}

		public static Vector3 UnpackDirection(ushort compressed)
		{
			var x = (ushort)((compressed & TopMask_16) >> Splitter_16);
			var y = (ushort)(compressed & BottomMask_16);

			if (x + y >= 127)
			{
				x = (ushort)(127 - x);
				y = (ushort)(127 - y);
			}

			int z = 126 - x - y;

			//TODO: May be used LuT.
			float normalizer = 1f / Mathf.Sqrt(x * x + y * y + z * z);

			var vector = new Vector3(x * normalizer, y * normalizer, z * normalizer);

			if ((compressed & SingMaskX_16) != 0)
			{
				vector.x = -vector.x;
			}

			if ((compressed & SingMaskY_16) != 0)
			{
				vector.y = -vector.y;
			}

			if ((compressed & SingMaskZ_16) != 0)
			{
				vector.z = -vector.z;
			}

			return vector;
		}
	}
}