namespace Fp.Network.Interpolation
{
	/// <summary>
	/// Strategy interface that defines the interpolation logic between two snapshots. 
	/// </summary>
	/// <typeparam name="TSnapshot">Target interpolated type</typeparam>
	public interface ILerpStrategy<TSnapshot>
		where TSnapshot : struct
	{
		void Interpolate(in TSnapshot from, in TSnapshot to, float time, out TSnapshot result);
	}
}