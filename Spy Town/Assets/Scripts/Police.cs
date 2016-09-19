using UnityEngine;
using System.Collections;

public class Police : Entity
{
	PoliceManager policeManager;

	void Awake()
	{
		base.Awake();

		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		InputEvent.Instance.OnObjectClicked += OnObjectClicked;
	}

	public void InitializePolice(GraphNode _startingNode, PoliceManager _policeManager)
	{
		currentNode = _startingNode;
		policeManager = _policeManager;
		transform.position = currentNode.transform.position;
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		
	}

	void OnObjectClicked(GameObject _go, RaycastHit _hit)
	{
		if (_go == gameObject)
		{
			// when I get clicked...
		}
	}
}
