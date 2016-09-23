using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Police : Entity
{
	PoliceManager policeManager;

	void Awake()
	{
		base.Awake();

		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		InputEvent.Instance.OnObjectClicked += OnObjectClicked;
		GameManager.Instance.OnNodesNeedRevealed += OnNodesNeedRevealed;
		GameManager.Instance.OnEntitiesNeedRevealed += OnEntitiesNeedRevealed;
	}

	public void InitializePolice(GraphNode _startingNode, PoliceManager _policeManager)
	{
		currentNode = _startingNode;
		policeManager = _policeManager;
		transform.position = currentNode.transform.position;
	}

	public void MoveToNewNode(GraphNode _node)
	{
		Move(_node);
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

	void OnNodesNeedRevealed()
	{
		// reveal this node, and all nodes connected
		currentNode.Reveal(true);
		List<GraphNode> connectedNodes = currentNode.GetConnectedNodes();
		for (int i = 0; i < connectedNodes.Count; i++)
		{
			connectedNodes[i].Reveal(true);
		}
	}

	void OnEntitiesNeedRevealed()
	{

	}
}
