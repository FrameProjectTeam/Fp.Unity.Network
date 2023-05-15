namespace Fp.Network.Interpolation
{
	public static class InterpolatorUtility
	{
		/// <summary>
		///     Convert current time to interpolate time with full delay(latency + interval + 5%).
		/// </summary>
		/// <param name="interpolator">Target synchronizer.</param>
		/// <param name="convertTime">Time to convert</param>
		/// <param name="scale">Difference multiplier</param>
		public static void ToSafeTime(this IInterpolator interpolator, ref float convertTime, float scale = 1.05f)
		{
			convertTime -= (interpolator.Latency + interpolator.Interval) * scale;
		}
	}
}