using System;
using System.Runtime.CompilerServices;

using Fp.Collections;
using Fp.Utility;

namespace Fp.Network.Interpolation
{
    public interface IInterpolator<TSnapshot>
        where TSnapshot : struct
    {
        void Interpolate(in TSnapshot from, in TSnapshot to, float time, out TSnapshot result);
    }
    
    public sealed class Synchronizer<TSnapshot, TInterpolator> : ISynchronizer<TSnapshot>
        where TSnapshot : struct
        where TInterpolator : IInterpolator<TSnapshot>
    {
        public const int InitCacheSize = 32;
        public const float DefaultCacheTime = 5; // In seconds
        public const float IncreaseLatencyPct = 3f; //%
        public const float IncreaseLatencyCoef = 1f + IncreaseLatencyPct / 100f;

        //Divisors example:
        //multiply by 1/4(25%) by modified value
        //multiply by 1/16(6.25%) by modified value
        //4 = 25%
        //16 = 6.25%
        //32 = 3.125%
        private const int IncreaseDivisor = 8;
        private const int DecreaseDivisor = 32;
        private float _latency;
        private float _updateTime;

        private readonly FpDeque<Snapshot> _history;

        private readonly TInterpolator _interpolator;
        
        public Synchronizer(TInterpolator interpolator, float cachedTime = DefaultCacheTime)
        {
            _interpolator = interpolator;
            HistoryTimeLimit = cachedTime;
            _history = new FpDeque<Snapshot>(InitCacheSize);
        }

#region IInterpolator<TValue> Implementation

        public float HistoryTimeLimit { get; }

        public int SnapshotCount => _history.Count;

        public float UpdateTime => _updateTime;
        public float Latency => _latency;

        public float Delay => (UpdateTime + Latency) * IncreaseLatencyCoef;

        public bool AutoCleanup { get; set; } = true;

        public bool AddSample(float remoteTime, float localTime, in TSnapshot snapshot)
        {
            if (!EstimateTimings(remoteTime, localTime))
            {
                return false;
            }

            _history.AddToRight(new Snapshot(remoteTime, snapshot));
            if (AutoCleanup)
            {
                InternalCleanup(remoteTime);
            }
            
            return true;
        }

        public void Cleanup()
        {
            if (!_history.TryPeekRight(out Snapshot value))
            {
                return;
            }

            InternalCleanup(value.RemoteTime);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalCleanup(float lastRemoteTime)
        {
            float timeCap = lastRemoteTime - HistoryTimeLimit;
            int capElement;
            for (capElement = 0; capElement < _history.Count; capElement++)
            {
                ref Snapshot snap = ref _history.PeekItem(capElement);
                if (snap.RemoteTime > timeCap)
                {
                    break;
                }
            }

            if (capElement > 0)
            {
                _history.RemoveLeft(capElement);
            }
        }
        
        public void Reset(float remoteTime, float clientTime, in TSnapshot snapshot)
        {
            _history.Clear();
            _history.AddToRight(new Snapshot(remoteTime, snapshot));

            _updateTime = clientTime - remoteTime;
            _latency = _updateTime;
        }

        public float Interpolate(float interpolateTime, out TSnapshot snapshot)
        {
            if (SnapshotCount == 0)
            {
                snapshot = default;
                return float.NaN;
            }

            if (SnapshotCount == 1)
            {
                ref Snapshot first = ref _history.PeekRight();
                snapshot = first.Value;
                return interpolateTime - first.RemoteTime;
            }

            for (var i = 1; i < SnapshotCount; i++)
            {
                ref Snapshot to = ref _history.PeekRight(i - 1);
                ref Snapshot from = ref _history.PeekRight(i);

                //Target last
                if (interpolateTime >= to.RemoteTime)
                {
                    snapshot = to.Value;
                    return interpolateTime - to.RemoteTime;
                }

                //Target between
                if (interpolateTime > from.RemoteTime && interpolateTime < to.RemoteTime)
                {
                    float t = MathUtils.Map01(interpolateTime, from.RemoteTime, to.RemoteTime);

                    _interpolator.Interpolate(from.Value, to.Value, t, out snapshot);
                    return 0;
                }
            }

            ref Snapshot last = ref _history.PeekLeft();
            snapshot = last.Value;
            return interpolateTime - last.RemoteTime;
        }
        
        public void ToInterpolationTime(ref float convertTime)
        {
            float latency = UpdateTime + Latency;
            convertTime -= latency;
        }

#endregion

        private bool EstimateTimings(float remoteTime, float clientTime)
        {
            //If it first time set this time as is.
            if (!_history.TryPeekRight(out Snapshot last))
            {
                _updateTime = clientTime - remoteTime;
                _latency = _updateTime;
                return true;
            }

            //Skip if have newer
            if (remoteTime <= last.RemoteTime)
            {
                return false;
            }

            EstimateValue(clientTime - remoteTime, ref _latency);
            EstimateValue(remoteTime - last.RemoteTime, ref _updateTime);

            return true;
        }

        private static void EstimateValue(float currentValue, ref float bufferedValue)
        {
            if (currentValue > bufferedValue)
            {
                const float iDivisor = 1f / IncreaseDivisor;
                const int multiplier = IncreaseDivisor - 1;

                bufferedValue = (bufferedValue * multiplier + currentValue) * iDivisor;
            }
            else
            {
                const float iDivisor = 1f / DecreaseDivisor;
                const int multiplier = DecreaseDivisor - 1;

                bufferedValue = (bufferedValue * multiplier + currentValue) * iDivisor;
            }
        }

        private readonly struct Snapshot
        {
            public readonly float RemoteTime;
            public readonly TSnapshot Value;

            public Snapshot(float remoteTime, TSnapshot value)
            {
                Value = value;
                RemoteTime = remoteTime;
            }
        }
    }
}