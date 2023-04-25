using System.Collections.Generic;

namespace Fp.Network.Interpolation
{
	public interface IInterpolator
		: IInterpolatorCleanup
	{
		/// <summary>
		///     Average interval between sending snapshots.
		/// </summary>
		float UpdateTime { get; }

		/// <summary>
		///     Waiting time for a delivering new snapshot.
		/// </summary>
		float Latency { get; }

		/// <summary>
		///     Summary interpolation delay
		/// </summary>
		float ComplexDelay { get; }

		/// <summary>
		///     Count of cached snapshot items.
		/// </summary>
		int HistoryLength { get; }

		/// <summary>
		///     Guaranteed time to save history.
		/// </summary>
		float HistoryTimeLimit { get; }

		float MaxExtrapolationTime { get; }
		float ExtrapolationTime { get; }
	}

	public interface IInterpolator<TState> : IInterpolator, IReadOnlyList<Snapshot<TState>>
		where TState : struct
	{
		/// <summary>
		///     Adds new snapshot sample to interpolation history.
		/// </summary>
		/// <param name="remoteTime">Remote session time for passed snapshot.</param>
		/// <param name="localTime">Local session time.</param>
		/// <param name="snapshot">Target snapshot, will copy to history.</param>
		/// <returns></returns>
		bool AddSample(float remoteTime, float localTime, in TState snapshot);

		/// <summary>
		///		Replace history by new state sample and recalculate new timings based on this value.
		/// </summary>
		/// <param name="remoteTime">Remote session time for passed snapshot.</param>
		/// <param name="clientTime">Local session time.</param>
		/// <param name="state">Target state, will copy to history.</param>
		void Reset(float remoteTime, float clientTime, in TState state);

		/// <summary>
		///		Clear history and reset timings.
		/// </summary>
		void Reset();
		
		/// <summary>
		///     Calculate intermediate(interpolated) snapshot based on history and passed session time
		///     <see cref="interpolateTime" />
		/// </summary>
		/// <param name="interpolateTime">Session time to make snapshot.</param>
		/// <param name="state">Intermediate(interpolated) snapshot.</param>
		/// <returns></returns>
		float Interpolate(float interpolateTime, out TState state);
	}
}