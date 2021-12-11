using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Fp.Network.Compression
{
	public static class CompressionUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Magnitude(this Quaternion quaternion)
		{
			return Math.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MagnitudeF(this Quaternion quaternion)
		{
			return (float) quaternion.Magnitude();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNormalized(this Quaternion quaternion)
		{
			return Mathf.Approximately((float)quaternion.Magnitude(), 1f);
		}
	}
}