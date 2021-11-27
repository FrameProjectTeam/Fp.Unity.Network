namespace Fp.Network.Interpolation
{
	public readonly struct Snapshot<TState>
	{
		public readonly float RemoteTime;
		public readonly TState Value;

		public Snapshot(float remoteTime, TState value)
		{
			Value = value;
			RemoteTime = remoteTime;
		}
	}
}