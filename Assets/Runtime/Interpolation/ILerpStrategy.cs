namespace Fp.Network.Interpolation
{
	/// <summary>
	/// Strategy interface that defines the interpolation logic between two snapshots. 
	/// </summary>
	/// <typeparam name="TState">Target interpolated type</typeparam>
	public interface ILerpStrategy<TState>
		where TState : struct
	{
		void Interpolate(in TState from, in TState to, float time, ref TState result);
	}
}