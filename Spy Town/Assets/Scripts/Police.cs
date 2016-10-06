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

	void OnDestroy()
	{
		base.OnDestroy();

		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnPhaseStart -= OnPhaseStart;
			GameManager.Instance.OnNodesNeedRevealed -= OnNodesNeedRevealed;
			GameManager.Instance.OnEntitiesNeedRevealed -= OnEntitiesNeedRevealed;
		}
		if (InputEvent.Instance != null)
		{
			InputEvent.Instance.OnObjectClicked -= OnObjectClicked;
		}
	}

	public void InitializePolice(GraphNode _startingNode, PoliceManager _policeManager)
	{
		currentNode = _startingNode;
		policeManager = _policeManager;
		transform.position = currentNode.transform.position;
	}

	public IEnumerator MoveToNewNode(GraphNode _node, float _afterDelay, float _moveTime)
	{
        yield return new WaitForSeconds(_afterDelay);

        float lerpProgress = 0.0f;
		while (lerpProgress < 1.0f)
		{
            lerpProgress += Time.deltaTime * (1.0f / _moveTime);
            transform.position = Vector3.Lerp(currentNode.transform.position, _node.transform.position, lerpProgress);
			yield return null;
		}
		
		Move(_node);

		// arrest occupying spies
		List<Entity> entities = _node.GetOccupyingEntities();
		for (int i = 0; i < entities.Count; i++)
		{
			Spy s = entities[i].GetComponent<Spy>();
			if (s != null)
			{
				s.GetMyEmbassy().ArrestSpy(s);
			}
		}
        policeManager.PoliceReportMovementComplete();

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
