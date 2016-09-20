using UnityEngine;
using System.Collections;

public class Spy : Entity
{
	private GameManager.Team myTeam;
	private Embassy myEmbassy;

	private bool isBeingMoved = false;

	void Awake()
	{
		base.Awake();

		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		InputEvent.Instance.OnObjectClicked += OnObjectClicked;
	}

	void Update()
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
				gameObject.GetComponent<Collider>().enabled = true;

				// move spy if possible, otherwise return to origin
				if (hoveredGraphNode != null && hoveredGraphNode != currentNode && myEmbassy.RequestSpyMovement(this, hoveredGraphNode))
				{
					myEmbassy.OnActionSpent();
					Move(hoveredGraphNode);
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
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		bool isMyTurn = _team == myTeam;
		if (_phase == GameManager.RoundPhase.PLAYERTURN)
		{
			// activate entity if it's their turn
			ActivateEntity(isMyTurn);
		}
		else if (_phase == GameManager.RoundPhase.MIDTURN || _phase == GameManager.RoundPhase.START)
		{
			ActivateEntity(false);
		}
	}

	void OnObjectClicked(GameObject _go, RaycastHit _hit)
	{
		if (_go == gameObject)
		{
			if (myEmbassy.HasActionRemaining() && myEmbassy.RequestSpySelection(this))
			{
				isBeingMoved = true;
				gameObject.GetComponent<Collider>().enabled = false;
			}
		}
	}
}
