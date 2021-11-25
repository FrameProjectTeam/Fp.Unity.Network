namespace Fp.Network.Interpolation
{
	public interface IInterpolatorCleanup
	{
		bool AutoCleanup { get; set; }
		void Cleanup();
	}
}