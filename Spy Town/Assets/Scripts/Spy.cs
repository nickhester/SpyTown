using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spy : Entity
{
	public GameManager.Team myTeam;
	private Embassy myEmbassy;

	private bool isBeingMoved = false;

	public Color primaryTeamColor;
	public Color secondaryTeamColor;

	new void Awake()
	{
		base.Awake();

		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		InputEvent.Instance.OnObjectClicked += OnObjectClicked;
		GameManager.Instance.OnNodesNeedRevealed += OnNodesNeedRevealed;
		GameManager.Instance.OnEntitiesNeedRevealed += OnEntitiesNeedRevealed;
	}

	new void OnDestroy()
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

	new void Start()
	{
		base.Start();
		
		ShowSpyCanvas(false);
	}

	new void Update()
	{
		base.Update();

		if (isBeingMoved)
		{
			Vector3 positionOnPlane;
			GraphNode hoveredGraphNode = null;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			// raycast against the ground for the spy movement
			Plane groundPlane = new Plane(Vector3.up, 0.0f);
			float rayDistance;
			if (groundPlane.Raycast(ray, out rayDistance))
			{
				positionOnPlane = ray.GetPoint(rayDistance);
				transform.position = positionOnPlane;
			}

			// raycast against colliders to see what's being hit
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				hoveredGraphNode = hit.collider.gameObject.GetComponent<GraphNode>();
			}

			if (Input.GetMouseButtonUp(0))
			{
				isBeingMoved = false;
				GetMyMesh().GetComponent<Collider>().enabled = true;

				// move spy if possible, otherwise return to origin
				if (hoveredGraphNode != null && hoveredGraphNode != currentNode && myEmbassy.RequestSpyMovement(this, hoveredGraphNode))
				{
					Move(hoveredGraphNode);
					GameManager.Instance.ReportActionTaken(myTeam, GameManager.ActionType.MOVE, hoveredGraphNode.GetNodeTeamAssociation());
				}
				else
				{
					ResetPosition();
				}
			}
		}
	}

	public void InitializeSpy(GraphNode _startingNode, GameManager.Team _team, Embassy _embassy)
	{
		currentNode = _startingNode;
		myTeam = _team;
		myEmbassy = _embassy;
		transform.position = currentNode.transform.position;

		if (_team == GameManager.Team.PRIMARY)
		{
			GetMyMesh().material.SetColor("_Color", primaryTeamColor);
		}
		else if (_team == GameManager.Team.SECONDARY)
		{
			GetMyMesh().material.SetColor("_Color", secondaryTeamColor);
		}
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		bool isMyTurn = _team == myTeam;
		if (_phase == GameManager.RoundPhase.PLAYERTURN)
		{
			ResetPosition();
			ActivateEntity(isMyTurn);
		}
		else if (_phase == GameManager.RoundPhase.MIDTURN || _phase == GameManager.RoundPhase.START)
		{
			ActivateEntity(false);
		}

		
	}

	void OnObjectClicked(GameObject _go, RaycastHit _hit)
	{
		if (_go == gameObject || _go.transform.root.gameObject == gameObject)
		{
			if (myEmbassy.HasActionRemaining() && myEmbassy.RequestSpySelection(this))
			{
				isBeingMoved = true;
				GetMyMesh().GetComponent<Collider>().enabled = false;
			}
		}
	}

	bool GetIsMyTurn()
	{
		return GameManager.Instance.currentPlayerTurn == myTeam;
	}

	void OnNodesNeedRevealed()
	{
		if (GameManager.Instance.currentPhase == GameManager.RoundPhase.PLAYERTURN && GetIsMyTurn())
		{
			// reveal this node, and all nodes connected
			currentNode.Reveal(true);
		}
	}

	void OnEntitiesNeedRevealed()
	{
		if (GameManager.Instance.currentPhase != GameManager.RoundPhase.PLAYERTURN || !GetIsMyTurn())
		{
			RevealEntity(currentNode.GetIsRevealed());
		}
	}

    override protected void OnEntityHasMoved(GraphNode _fromNode, GraphNode _toNode, Entity _entity)
    {
        base.OnEntityHasMoved(_fromNode, _toNode, _entity);
		
		ShowSpyCanvas(false);
    }

	public void Button_Arrest()
	{
		myEmbassy.ArrestSpy(this, null);
	}

	public void ShowSpyCanvas(bool _b)
	{
		GetMyCanvas().enabled = _b;
	}

	public Embassy GetMyEmbassy()
	{
		return myEmbassy;
	}
}
