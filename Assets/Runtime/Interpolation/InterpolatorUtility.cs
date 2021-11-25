namespace Fp.Network.Interpolation
{
	public static class InterpolatorUtility
	{
		/// <summary>
		///		Convert current time to interpolate time with delay.
		/// </summary>
		/// <param name="interpolator">Target synchronizer.</param>
		/// <param name="convertTime"></param>
		public static void ToInterpolationTime(this IInterpolator interpolator, ref float convertTime)
		{
			convertTime -= interpolator.Delay;
		}
	}
}