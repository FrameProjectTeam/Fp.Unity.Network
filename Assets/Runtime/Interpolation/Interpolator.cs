using System;
using System.Runtime.CompilerServices;

using Fp.Collections;
using Fp.Utility;

namespace Fp.Network.Interpolation
{
	public sealed class Interpolator<TSnapshot, TLerpStrategy> : IInterpolator<TSnapshot>
		where TSnapshot : struct
		where TLerpStrategy : ILerpStrategy<TSnapshot>
	{
		/// <summary>
		///     Initial circular buffer size
		/// </summary>
		public const int InitCacheSize = 32;

		/// <summary>
		///     Default value for limiting snapshot history in the buffer by lifetime in seconds.
		/// </summary>
		public const float DefaultCacheTime = 5;

		/// <summary>
		///     Increases latency by a certain pct. To avoid noise in delay.
		/// </summary>
		public const float IncreaseLatencyPct = 3f;

		/// <summary>
		///     Calculated const. Do change <see cref="IncreaseLatencyPct" /> values for correcting this coefficient
		/// </summary>
		public const float IncreaseLatencyCoef = 1f + IncreaseLatencyPct / 100f;

		/// <summary>
		///     Determines ratio with which cached value change by estimated value.
		///     When estimated value is increasing.
		///     <example>
		///         Multiply by 1/4(25%) by modified value
		///         4 = 25%
		///         Multiply by 1/16(6.25%) by modified value
		///         16 = 6.25%
		///         32 = 3.125%
		///     </example>
		/// </summary>
		private const int IncreaseDivisor = 8;

		/// <summary>
		///     Determines ratio with which cached value change by estimated value.
		///     When estimated value is decreasing.
		///     <example>
		///         Multiply by 1/4(25%) by modified value
		///         4 = 25%
		///         Multiply by 1/16(6.25%) by modified value
		///         16 = 6.25%
		///         32 = 3.125%
		///     </example>
		/// </summary>
		private const int DecreaseDivisor = 32;

		/// <summary>
		///     Maximum possible increasing by pct. with current value.
		/// </summary>
		private const float IncreaseLimitPct = 2;

		/// <summary>
		///     Minimum possible decreasing by pct. with current value.
		/// </summary>
		private const float DecreaseLimitPct = 0.5f;

		// Waiting time for a delivering new snapshot.
		private float _latency;

		// Average interval between sending snapshots.
		private float _updateTime;

		// Cyclic buffer for saving snapshot history. 
		private readonly FpDeque<Snapshot> _history;

		// The strategy that defines the interpolation logic between two snapshots. 
		private readonly TLerpStrategy _interpolator;

		public Interpolator(TLerpStrategy interpolator, float cachedTime = DefaultCacheTime)
		{
			HistoryTimeLimit = cachedTime;

			_interpolator = interpolator;
			_history = new FpDeque<Snapshot>(InitCacheSize);
		}

#region IInterpolator Implementation

		public float HistoryTimeLimit { get; }

		/// <summary>
		///     Count of cached snapshot items.
		/// </summary>
		public int SnapshotCount => _history.Count;

		/// <summary>
		///     Average interval between sending snapshots.
		/// </summary>
		public float UpdateTime => _updateTime;

		/// <summary>
		///     Waiting time for a delivering new snapshot.
		/// </summary>
		public float Latency => _latency;

		/// <summary>
		///     Summary interpolation delay
		/// </summary>
		public float Delay => (UpdateTime + Latency) * IncreaseLatencyCoef;

#endregion

#region IInterpolator<TSnapshot> Implementation

		/// <summary>
		///     Adds new snapshot sample to interpolation history.
		/// </summary>
		/// <param name="remoteTime">Remote session time for passed snapshot.</param>
		/// <param name="localTime">Local session time.</param>
		/// <param name="snapshot">Target snapshot, will copy to history.</param>
		/// <returns></returns>
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

		/// <summary>
		///     Reset history and recalculate new timings by passed values.
		/// </summary>
		/// <param name="remoteTime">Remote session time for passed snapshot.</param>
		/// <param name="clientTime">Local session time.</param>
		/// <param name="snapshot">Target snapshot, will copy to history.</param>
		public void Reset(float remoteTime, float clientTime, in TSnapshot snapshot)
		{
			_history.Clear();
			_history.AddToRight(new Snapshot(remoteTime, snapshot));

			_latency = clientTime - remoteTime;
			_updateTime = _latency;
		}

		/// <summary>
		///     Calculate intermediate(interpolated) snapshot based on history and passed session time
		///     <see cref="interpolateTime" />
		/// </summary>
		/// <param name="interpolateTime">Session time to make snapshot.</param>
		/// <param name="snapshot">Intermediate(interpolated) snapshot.</param>
		/// <returns></returns>
		public float Interpolate(float interpolateTime, out TSnapshot snapshot)
		{
			switch (SnapshotCount)
			{
				case 0:
				{
					snapshot = default;
					return float.NaN;
				}
				case 1:
				{
					ref Snapshot first = ref _history.PeekRight();
					snapshot = first.Value;
					return interpolateTime - first.RemoteTime;
				}
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

#endregion

#region IInterpolatorCleanup Implementation

		public bool AutoCleanup { get; set; } = true;

		public void Cleanup()
		{
			if (!_history.TryPeekRight(out Snapshot value))
			{
				return;
			}

			InternalCleanup(value.RemoteTime);
		}

#endregion

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

		private bool EstimateTimings(float remoteTime, float clientTime)
		{
			//If it first time set this time as is.
			if (!_history.TryPeekRight(out Snapshot last))
			{
				_latency = clientTime - remoteTime;
				_updateTime = _latency;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EstimateValue(float currentValue, ref float bufferedValue)
		{
			if (currentValue > bufferedValue)
			{
				const float iDivisor = 1f / IncreaseDivisor;
				const int multiplier = IncreaseDivisor - 1;

				bufferedValue = Math.Min((bufferedValue * multiplier + currentValue) * iDivisor, bufferedValue * IncreaseLimitPct);
			}
			else
			{
				const float iDivisor = 1f / DecreaseDivisor;
				const int multiplier = DecreaseDivisor - 1;

				bufferedValue = Math.Max((bufferedValue * multiplier + currentValue) * iDivisor, bufferedValue * DecreaseLimitPct);
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