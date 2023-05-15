using System.Collections.Generic;

using Fp.Network.Interpolation;

using UnityEngine;

public class InterpolatorExample : MonoBehaviour
{
	[SerializeField]
	private Transform _localTransform;

	[SerializeField]
	private Transform _remoteTransform;

	[SerializeField]
	private float _pingTime = 0.05f;

	[SerializeField]
	private float _pingNoise = 0.01f;

	[SerializeField]
	private float _dropRate = 0.1f;

	[SerializeField]
	private float _sendInterval = 0.1f;

	[SerializeField]
	private float _movementSpeed = 5f;

	[SerializeField]
	private float _rotationSpeed = 30f;

	[SerializeField]
	private Vector3 _remoteOffset = Vector3.zero;

	private Interpolator<TransformState, TransformStateLerpStrategy> _remoteInterpolator;

	[SerializeField]
	[Tooltip(
		"Extrapolation time in seconds, edit it to see how it affects the interpolation. (0.1f is default)\nDo not edit it in playmode, it's not be affected."
	)]
	private float _extrapolationTime = 0.1f;

	private readonly Queue<NetworkSnapshot> _snapshotsQueue = new();

	private float _lastSendTime = float.NegativeInfinity;

	private TransformState _tempState;

	private void Update()
	{
		if(Random.value > 0.5f)
		{
			LocalUpdate();
			RemoveUpdate();
		}
		else
		{
			RemoveUpdate();
			LocalUpdate();
		}
	}

	private void Awake()
	{
		_remoteInterpolator = new Interpolator<TransformState, TransformStateLerpStrategy>(new TransformStateLerpStrategy(), _extrapolationTime);
	}

	private void LocalUpdate()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		var movementDirection = new Vector3(horizontal, 0, vertical);
		_localTransform.position += new Vector3(horizontal, 0, vertical) * Time.deltaTime * _movementSpeed;

		if(movementDirection.sqrMagnitude > 0.01f)
		{
			_localTransform.rotation = Quaternion.Lerp(
				_localTransform.rotation, Quaternion.LookRotation(movementDirection.normalized), Time.deltaTime * _rotationSpeed
			);
		}

		SendSnapshot();
	}

	private void SendSnapshot()
	{
		if(Time.time - _lastSendTime < _sendInterval)
		{
			return;
		}

		_snapshotsQueue.Enqueue(
			new NetworkSnapshot
			{
				Time = Time.time,
				TransformState = new TransformState
				{
					Position = _localTransform.position,
					Rotation = _localTransform.rotation
				}
			}
		);

		_lastSendTime = Time.time;
	}

	private void RemoveUpdate()
	{
		HandleMessageQueue();

		float time = Time.time;

		_remoteInterpolator.ToSafeTime(ref time, 1);
		_remoteInterpolator.Interpolate(time, ref _tempState);

		_remoteTransform.SetPositionAndRotation(_remoteOffset + _tempState.Position, _tempState.Rotation);
	}

	private void HandleMessageQueue()
	{
		if(_snapshotsQueue.TryPeek(out NetworkSnapshot networkSnapshot))
		{
			float noise = Random.value * _pingNoise;
			if(Time.time - networkSnapshot.Time > _pingTime + noise)
			{
				_snapshotsQueue.Dequeue();
			}
			else
			{
				return;
			}

			if(Random.value < _dropRate)
			{
				return;
			}

			_remoteInterpolator.AddSample(networkSnapshot.Time, Time.time, networkSnapshot.TransformState);
		}
	}

	public struct NetworkSnapshot
	{
		public float Time { get; set; }
		public TransformState TransformState { get; set; }
	}

	public struct TransformState
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
	}

	public readonly struct TransformStateLerpStrategy : ILerpStrategy<TransformState>
	{
#region ILerpStrategy<TransformState> Implementation

		public void Interpolate(in TransformState from, in TransformState to, float time, ref TransformState result)
		{
			result.Position = Vector3.LerpUnclamped(from.Position, to.Position, time);
			result.Rotation = Quaternion.SlerpUnclamped(from.Rotation, to.Rotation, time);
		}

#endregion
	}
}