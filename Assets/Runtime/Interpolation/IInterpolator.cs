namespace Fp.Network.Interpolation
{
	public interface IInterpolator
		: IInterpolatorCleanup
	{
		float UpdateTime { get; }
		float Latency { get; }
		int SnapshotCount { get; }
		float HistoryTimeLimit { get; }
		float Delay { get; }
	}

	public interface IInterpolator<TValue> : IInterpolator
		where TValue : struct
    {
	    bool AddSample(float remoteTime, float localTime, in TValue snapshot);
        void Reset(float remoteTime, float clientTime, in TValue snapshot);
        
        float Interpolate(float interpolateTime, out TValue snapshot);
    }
}