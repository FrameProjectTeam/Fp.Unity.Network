namespace Fp.Network.Interpolation
{
	public static class InterpolatorUtility
	{
		/// <summary>
		///     Increases latency by a certain pct. To avoid noise in delay.
		/// </summary>
		public const float IncreaseLatencyPct = 5f; // %

		/// <summary>
		///     Calculated const. Do change <see cref="IncreaseLatencyPct" /> values for correcting this coefficient
		/// </summary>
		public const float IncreaseLatencyCoef = 1f + IncreaseLatencyPct / 100f;
		
		/// <summary>
		///		Convert current time to interpolate time with full delay(latency + interval + 5%).
		/// </summary>
		/// <param name="interpolator">Target synchronizer.</param>
		/// <param name="convertTime"></param>
		public static void ToInterpolationMaximizedTime(this IInterpolator interpolator, ref float convertTime)
		{
			convertTime -= interpolator.Delay * IncreaseLatencyCoef;
		}
		
		/// <summary>
		///		Convert current time to interpolate time with max of (latency, interval) + 5%.
		/// </summary>
		/// <param name="interpolator">Target synchronizer.</param>
		/// <param name="convertTime"></param>
		public static void ToInterpolationOptimizedTime(this IInterpolator interpolator, ref float convertTime)
		{
			convertTime -= Mathf.Max(interpolator.Latency, interpolator.UpdateTime) * IncreaseLatencyCoef;
		}
	}
}