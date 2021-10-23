namespace Fp.Network.Interpolation
{
    public interface ISynchronizer<TValue>
     where TValue : struct
    {
        float UpdateTime { get; }
        float Latency { get; }
        
        int SnapshotCount { get; }
        float HistoryTimeLimit { get; }
        float Delay { get; }

        bool AddSample(float remoteTime, float localTime, in TValue snapshot);
        void Reset(float remoteTime, float clientTime, in TValue snapshot);
        
        float Interpolate(float interpolateTime, out TValue snapshot);
        void ToInterpolationTime(ref float convertTime);
        void Cleanup();
    }
}