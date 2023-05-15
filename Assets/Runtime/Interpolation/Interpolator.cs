using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Fp.Collections;
using Fp.Utility;

using UnityEngine.Assertions;

namespace Fp.Network.Interpolation
{
	public sealed class Interpolator<TState, TLerpStrategy> : IInterpolator<TState>
		where TState : struct
		where TLerpStrategy : ILerpStrategy<TState>
	{
		/// <summary>
		///     Initial circular buffer size
		/// </summary>
		private const int InitCacheSize = 32;

		/// <summary>
		///     Default value for limiting snapshot history in the buffer by lifetime in seconds.
		/// </summary>
		private const float DefaultCacheTime = 5;

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
		///     Maximum possible increasing in seconds.
		/// </summary>
		private const float IncreaseTimeLimit = 0.25F;

		// Waiting time for a delivering new snapshot.
		private float _latency;

		// Average interval between sending snapshots.
		private float _interval;

		// Cyclic buffer for saving snapshot history. 
		private readonly Deque<Snapshot<TState>> _history;

		// The strategy that defines the interpolation logic between two snapshots. 
		private readonly TLerpStrategy _lerp;

		public Interpolator(TLerpStrategy lerp, float maxExtrapolationTime = float.PositiveInfinity, float cachedTime = DefaultCacheTime)
		{
			MaxExtrapolationTime = maxExtrapolationTime;
			HistoryTimeLimit = cachedTime;

			_lerp = lerp;
			_history = new Deque<Snapshot<TState>>(InitCacheSize);
		}

#region IEnumerable Implementation

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)_history).GetEnumerator();
		}

#endregion

#region IEnumerable<Snapshot<TState>> Implementation

		IEnumerator<Snapshot<TState>> IEnumerable<Snapshot<TState>>.GetEnumerator()
		{
			return _history.GetEnumerator();
		}

#endregion

#region IInterpolator Implementation

		public float MaxExtrapolationTime { get; }
		public float HistoryTimeLimit { get; }

		/// <inheritdoc />
		public int HistoryLength => _history.Count;

		/// <inheritdoc />
		public float Interval => _interval;

		/// <inheritdoc />
		public float Latency => _latency;

		public float ExtrapolationTime { get; private set; }

#endregion

#region IInterpolator<TState> Implementation

		/// <inheritdoc />
		public bool AddSample(float remoteTime, float localTime, in TState snapshot)
		{
			if(!EstimateTimings(remoteTime, localTime))
			{
				return false;
			}

			_history.AddToRight(new Snapshot<TState>(remoteTime, snapshot));
			if(AutoCleanup)
			{
				InternalCleanup(remoteTime);
			}

			return true;
		}

		/// <inheritdoc />
		public void Reset(float remoteTime, float clientTime, in TState state)
		{
			_history.Clear();
			_history.AddToRight(new Snapshot<TState>(remoteTime, state));

			_latency = clientTime - remoteTime;
			_interval = _latency;
		}

		/// <inheritdoc />
		public void Reset()
		{
			_history.Clear();

			_latency = 0;
			_interval = 0;
		}

		/// <inheritdoc />
		public float Interpolate(float interpolateTime, ref TState state)
		{
			//Interpolate between snapshot starting with a new ones ending with an old ones.
			for(var i = 1; i < HistoryLength; i++)
			{
				ref Snapshot<TState> toState = ref _history.PeekRight(i - 1);
				ref Snapshot<TState> fromState = ref _history.PeekRight(i);

				//Target between
				if(interpolateTime <= fromState.RemoteTime)
				{
					continue;
				}

				float t = MathUtils.Map01(interpolateTime, fromState.RemoteTime, toState.RemoteTime);

				if(t > 1)
				{
					return Extrapolation(interpolateTime, ref state, fromState, toState, t);
				}

				ExtrapolationTime = 0;
				_lerp.Interpolate(fromState.Value, toState.Value, t, ref state);

				return 0;
			}

			if(_history.IsEmpty)
			{
				//If have no history return default(invalid) value
				state = default;
				return float.NaN;
			}

			ref Snapshot<TState> first = ref _history.PeekItem(0);

			state = first.Value;
			return interpolateTime - first.RemoteTime;
		}

		private float Extrapolation(
			float interpolateTime,
			ref TState state,
			Snapshot<TState> fromState,
			Snapshot<TState> toState,
			float lerpTime)
		{ 
			//Extrapolation time limitation
			ExtrapolationTime = interpolateTime - toState.RemoteTime;
			if(ExtrapolationTime > MaxExtrapolationTime)
			{
				lerpTime = 1 + MaxExtrapolationTime / (toState.RemoteTime - fromState.RemoteTime);
				_lerp.Interpolate(fromState.Value, toState.Value, lerpTime, ref state);

				return MaxExtrapolationTime;
			}

			_lerp.Interpolate(fromState.Value, toState.Value, lerpTime, ref state);
			//Extrapolation time
			return ExtrapolationTime;
		}

#endregion

#region IInterpolatorCleanup Implementation

		public bool AutoCleanup { get; set; } = true;

		public void Cleanup()
		{
			if(!_history.TryPeekRight(out Snapshot<TState> value))
			{
				return;
			}

			InternalCleanup(value.RemoteTime);
		}

#endregion

#region IReadOnlyCollection<Snapshot<TState>> Implementation

		int IReadOnlyCollection<Snapshot<TState>>.Count => _history.Count;

#endregion

#region IReadOnlyList<Snapshot<TState>> Implementation

		public Snapshot<TState> this[int index] => _history[index];

#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InternalCleanup(float lastRemoteTime)
		{
			float timeCap = lastRemoteTime - HistoryTimeLimit;
			int capElement;
			for(capElement = 0; capElement < _history.Count; capElement++)
			{
				ref Snapshot<TState> snap = ref _history.PeekItem(capElement);
				if(snap.RemoteTime > timeCap)
				{
					break;
				}
			}

			if(capElement > 0)
			{
				_history.RemoveLeft(capElement);
			}
		}

		private bool EstimateTimings(float remoteTime, float clientTime)
		{
			float curLatency = Math.Max(clientTime - remoteTime, 0);
			
			if(!_history.TryPeekRight(out Snapshot<TState> last))
			{
				_latency = curLatency;
				_interval = _latency;
				return true;
			}

			//Skip if have newer
			if(remoteTime <= last.RemoteTime)
			{
				return false;
			}

			EstimateValue(curLatency, ref _latency);
			EstimateValue(remoteTime - last.RemoteTime, ref _interval);

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EstimateValue(float currentValue, ref float bufferedValue)
		{
			Assert.IsTrue(currentValue >= 0, $"currentValue >= 0, {currentValue}");

			if(currentValue > bufferedValue)
			{
				const float iDivisor = 1f / IncreaseDivisor;
				const int multiplier = IncreaseDivisor - 1;

				currentValue = Math.Min(currentValue, bufferedValue + IncreaseTimeLimit);
				bufferedValue = (bufferedValue * multiplier + currentValue) * iDivisor;
			}
			else
			{
				const float iDivisor = 1f / DecreaseDivisor;
				const int multiplier = DecreaseDivisor - 1;

				bufferedValue = (bufferedValue * multiplier + currentValue) * iDivisor;
			}
		}
	}
}