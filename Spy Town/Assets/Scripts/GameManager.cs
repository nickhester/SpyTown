using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private List<Entity> allEntities;

	// object references
	public GraphManager graphManager;
	private GameOptions gameOptions;
	public GameObject gameOptionsPrefab;

	// delegates
	public delegate void OnPhaseStartEvent(RoundPhase _phase, Team _team);
	public event OnPhaseStartEvent OnPhaseStart;
	public delegate void OnEntityHasMovedEvent(GraphNode _fromNode, GraphNode _toNode, Entity _entity);
	public event OnEntityHasMovedEvent OnEntityHasMoved;
	public delegate void OnActionTakenEvent(Team _team, ActionType _action, GameManager.Team _buildingTeam);
	public event OnActionTakenEvent OnActionTaken;
	public delegate void OnNodesNeedRevealedEvent();
	public event OnNodesNeedRevealedEvent OnNodesNeedRevealed;
	public delegate void OnEntitiesNeedRevealedEvent();
	public event OnEntitiesNeedRevealedEvent OnEntitiesNeedRevealed;
	public delegate void OnStatUpdatedEvent();
	public event OnStatUpdatedEvent OnStatUpdated;
	public delegate void OnGameEndEvent(Team _teamWon);
	public event OnGameEndEvent OnGameEnd;
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
		MIDTURN,
		END
	}
	public enum Pickups
	{
		EXTRA_ACTION
	}
	public enum ActionType
	{
		MOVE,
		ARREST,
		BONUS_ACTION
	}
	public RoundPhase currentPhase = RoundPhase.START;
	public Team currentPlayerTurn = Team.PRIMARY;

	private List<Embassy> embassies;

	void InstanceInitialize()
	{
		graphManager = GameObject.FindObjectOfType<GraphManager>();
	}

	void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;

		OnPhaseStart(currentPhase, currentPlayerTurn);
	}

	public GameOptions GetGameOptions()
	{
		if (gameOptions == null)
		{
			gameOptions = FindObjectOfType<GameOptions>();

			if (gameOptions == null)
			{
				GameObject go = Instantiate(gameOptionsPrefab) as GameObject;
				gameOptions = go.GetComponent<GameOptions>();
			}
		}
		return gameOptions;
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

	public void ReportActionTaken(Team _team, GameManager.ActionType _action, GameManager.Team _buildingTeam)
	{
		allEntities = null;

		OnActionTaken(_team, _action, _buildingTeam);
	}

	public void ReportStatUpdated()
	{
		OnStatUpdated();
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

	public void ReportGameHasBeenWon(Team _teamWon)
	{
		print("The Game Has Been Won!");
		OnGameEnd(_teamWon);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
	
	public void Debug_AddAction()
	{
		GetActiveEmbassy().Debug_ApplyBonusAction();
	}
	
}
