using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flip : MonoBehaviour
{
	private bool isViewModeBoardGame;
	private Quaternion originalRotation;
	private Quaternion flippedRotation;
	private Vector3 originalPosition;
	private Vector3 flippedPosition;
	private RectTransform myRectTransform;

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
		isViewModeBoardGame = GameManager.Instance.GetGameOptions().viewModeBoardGame;

		myRectTransform = GetComponent<RectTransform>();
		if (myRectTransform)
		{
			originalRotation = myRectTransform.rotation;
			originalPosition = myRectTransform.position;

			// flip, get flipped rotation, then flip back
			//myRectTransform.RotateAround(Vector3.zero, Vector3.forward, 180.0f);
			myRectTransform.Rotate(0.0f, 0.0f, 180.0f);
			flippedRotation = myRectTransform.rotation;
			flippedPosition = myRectTransform.position;
			myRectTransform.rotation = originalRotation;
			myRectTransform.position = originalPosition;
		}
		else
		{
			Debug.LogError("Flip Component has no RectTransform to rotate!");
		}
	}

	void FlipObject()
	{
		myRectTransform.rotation = flippedRotation;
		myRectTransform.position = flippedPosition;
	}

	void UnflipObject()
	{
		myRectTransform.rotation = originalRotation;
		myRectTransform.position = originalPosition;
	}

	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		if (_phase == GameManager.RoundPhase.MIDTURN && _team == GameManager.Team.SECONDARY)
		{
			FlipObject();
		}
		else if (_phase == GameManager.RoundPhase.PLAYERTURN && _team == GameManager.Team.SECONDARY)
		{
			FlipObject();
		}
		else
		{
			UnflipObject();
		}
	}
}
