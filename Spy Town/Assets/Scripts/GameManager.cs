using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public List<Entity> allEntities = new List<Entity>();

	// object references
	public GraphManager graphManager;

	// delegates
	public delegate void OnPhaseStartEvent(RoundPhase _phase, Team _team);
	public event OnPhaseStartEvent OnPhaseStart;
	public delegate void OnEntityHasMovedEvent(GraphNode _graphNode);
	public event OnEntityHasMovedEvent OnEntityHasMoved;
	public delegate void OnEntityHasMovedLateEvent(GraphNode _graphNode);
	public event OnEntityHasMovedLateEvent OnEntityHasMovedLate;
	public delegate void OnActionTakenEvent();
	public event OnActionTakenEvent OnActionTaken;
	public delegate void OnNodesNeedRevealedEvent();
	public event OnNodesNeedRevealedEvent OnNodesNeedRevealed;
	public delegate void OnEntitiesNeedRevealedEvent();
	public event OnEntitiesNeedRevealedEvent OnEntitiesNeedRevealed;
	// singleton
	private static GameManager instance;
    // instance
	public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
				instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return instance;
        }
    }

	public enum Team
	{
		PRIMARY,
		SECONDARY
	}

	public enum RoundPhase
	{
		START,
		PLAYERTURN,
		MIDTURN
	}
	public RoundPhase currentPhase = RoundPhase.START;
	public Team currentPlayerTurn = Team.PRIMARY;

	private List<Embassy> embassies = new List<Embassy>();

	void Start ()
	{
		allEntities.AddRange(GameObject.FindObjectsOfType<Entity>());
		OnPhaseStart(currentPhase, currentPlayerTurn);
		embassies.AddRange(GameObject.FindObjectsOfType<Embassy>());
		graphManager = GameObject.FindObjectOfType<GraphManager>();
	}

	public Embassy GetActiveEmbassy()
	{
		for (int i = 0; i < embassies.Count; i++)
		{
			if (embassies[i].myTeam == currentPlayerTurn)
			{
				return embassies[i];
			}
		}
		return null;
	}

	public void StartNextPhase()
	{
		if (currentPhase == RoundPhase.START)
		{
			currentPhase = RoundPhase.PLAYERTURN;
		}
		else if (currentPhase == RoundPhase.MIDTURN)	// if MIDTURN, go to next player turn
		{
			currentPhase = RoundPhase.PLAYERTURN;
		}
		else if (currentPhase == RoundPhase.PLAYERTURN)
		{
			currentPhase = RoundPhase.MIDTURN;
			currentPlayerTurn = (currentPlayerTurn == Team.PRIMARY ? Team.SECONDARY : Team.PRIMARY);
		}

		OnPhaseStart(currentPhase, currentPlayerTurn);
		UpdateBoardVisibility();
	}

	public void ReportEntityHasMoved(GraphNode _n)
	{
		UpdateBoardVisibility();
		OnEntityHasMoved(_n);
		//OnEntityHasMovedLate(_n);
	}

	public void ReportActionTaken()
	{
		OnActionTaken();
	}

	void UpdateBoardVisibility()
	{
		graphManager.RevealAll(false);
		OnNodesNeedRevealed();
		OnEntitiesNeedRevealed();
	}
}
