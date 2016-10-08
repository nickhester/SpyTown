using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	private List<Entity> allEntities;

	// object references
	public GraphManager graphManager;
	public GameOptions gameOptions;

	// delegates
	public delegate void OnPhaseStartEvent(RoundPhase _phase, Team _team);
	public event OnPhaseStartEvent OnPhaseStart;
	public delegate void OnEntityHasMovedEvent(GraphNode _fromNode, GraphNode _toNode, Entity _entity);
	public event OnEntityHasMovedEvent OnEntityHasMoved;
	public delegate void OnActionTakenEvent(Team _team, Action.ActionType _action, GameManager.Team _buildingTeam);
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
				instance.InstanceInitialize();
			}
            return instance;
        }
    }

	public enum Team
	{
		PRIMARY,
		SECONDARY,
		NEUTRAL
	}
	public enum RoundPhase
	{
		START,
		PLAYERTURN,
		MIDTURN
	}
	public RoundPhase currentPhase = RoundPhase.START;
	public Team currentPlayerTurn = Team.PRIMARY;

	private List<Embassy> embassies;

	void InstanceInitialize()
	{
		graphManager = GameObject.FindObjectOfType<GraphManager>();
		gameOptions = GetComponent<GameOptions>();
	}

	void Start ()
	{
		OnPhaseStart(currentPhase, currentPlayerTurn);
	}

	public List<Entity> GetAllEntities()
	{
		if (allEntities == null)
		{
			allEntities = new List<Entity>();
			allEntities.AddRange(GameObject.FindObjectsOfType<Entity>());
		}
		return allEntities;
	}

	public List<Embassy> GetEmbassies()
	{
		if (embassies == null)
		{
			embassies = new List<Embassy>();
			embassies.AddRange(GameObject.FindObjectsOfType<Embassy>());
		}
		return embassies;
	}
	
	public Embassy GetActiveEmbassy()
	{
		for (int i = 0; i < GetEmbassies().Count; i++)
		{
			if (GetEmbassies()[i].myTeam == currentPlayerTurn)
			{
				return GetEmbassies()[i];
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

	public void ReportEntityHasMoved(GraphNode _fromNode, GraphNode _toNode, Entity _entity)
	{
		UpdateBoardVisibility();
		OnEntityHasMoved(_fromNode, _toNode, _entity);
	}

	public void ReportActionTaken(Team _team, Action.ActionType _action, GameManager.Team _buildingTeam)
	{
		allEntities = null;

		OnActionTaken(_team, _action, _buildingTeam);
	}

	void UpdateBoardVisibility()
	{
		graphManager.RevealAll(false);
		OnNodesNeedRevealed();
		OnEntitiesNeedRevealed();
	}

	public static bool IsInstanceIsNotNull()
	{
		if (instance == null)
		{
			return false;
		}
		return true;
	}

	public void ReportGameHasBeenWon(Embassy _embassy)
	{
		print("The Game Has Been Won!");
	}
}
