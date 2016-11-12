using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour
{
	private List<Vector3> startingPosition = new List<Vector3>();
	private List<Quaternion> startingOrientation = new List<Quaternion>();
	private CameraPosition currentPosition = CameraPosition.PRIMARY;
	public float amountToSlide = 1.0f;
	public float cameraLerpSpeed = 1.0f;

	public enum CameraPosition
	{
		PRIMARY,
		SECONDARY,
		CENTER
	}

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
	}

	void OnDestroy()
	{
		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnPhaseStart -= OnPhaseStart;
		}
	}

	void Start ()
	{
		startingPosition.Add(transform.position);
		startingOrientation.Add(transform.rotation);

		transform.RotateAround(Vector3.zero, Vector3.left, (90.0f - transform.rotation.eulerAngles.x) * -2.0f);
		transform.Translate(0.0f, 0.0f, amountToSlide, Space.World);

		startingPosition.Add(transform.position);
		startingOrientation.Add(transform.rotation);

		transform.forward = Vector3.down;
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
		
		startingPosition.Add(transform.position);
		startingOrientation.Add(transform.rotation);

		// set back to starting position
		transform.position = startingPosition[2];
		transform.rotation = startingOrientation[2];

		MoveToPosition(CameraPosition.PRIMARY);
	}

	public void MoveToPosition(CameraPosition _position)
	{
		StartCoroutine(LerpToPosition(_position));
	}

	IEnumerator LerpToPosition(CameraPosition _position)
	{
		float lerpProgress = 0.0f;
		Vector3 startPos = transform.position;
		Quaternion startRot = transform.rotation;
		Vector3 targetPos = startingPosition[(int)_position];
		Quaternion targetRot = startingOrientation[(int)_position];

		while (true)
		{
			lerpProgress += Time.deltaTime * cameraLerpSpeed;
			lerpProgress = Mathf.Min(1.0f, lerpProgress);
			float easedLerpProgress = EaseInAndOut(lerpProgress);

			transform.position = Vector3.Lerp(startPos, targetPos, easedLerpProgress);
			transform.rotation = Quaternion.Slerp(startRot, targetRot, easedLerpProgress);

			if (lerpProgress >= 1.0f)
			{
				break;
			}
			yield return null;
		}
	}

	float EaseInAndOut(float value)
	{
		return (Mathf.Sin((value * Mathf.PI) + (1.5f * Mathf.PI)) * 0.5f) + 0.5f;
	}

	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		if (_phase == GameManager.RoundPhase.PLAYERTURN && _team != GameManager.Team.NEUTRAL)
		{
			if (_team == GameManager.Team.PRIMARY)
			{
				MoveToPosition(CameraPosition.PRIMARY);
			}
			else if (_team == GameManager.Team.SECONDARY)
			{
				if (GameManager.Instance.GetGameOptions().viewModeBoardGame)
				{
					MoveToPosition(CameraPosition.SECONDARY);
				}
			}
		}
		else
		{
			MoveToPosition(CameraPosition.CENTER);
		}
	}
}
