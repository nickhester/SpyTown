using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Embassy : MonoBehaviour
{
	public GameManager.Team myTeam;
	public GameObject spyPrefab;
	private List<Spy> mySpies = new List<Spy>();
	private PoliceManager policeManager;
	private Embassy opposingEmbassy;
	[HideInInspector] public GraphNode myEmbassyNode;

	public int numActionsPerTurn;
	private int numActionsRemaining;
	private int numSpiesReachedOpposingEmbassy = 0;
	private int numEnemySpiesArrested = 0;

	private List<ActionRecord> turnSummary = new List<ActionRecord>();
	private List<GameManager.Pickups> mySpecialActions = new List<GameManager.Pickups>();

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnActionTaken += OnActionTaken;
		GameManager.Instance.OnEntityHasMoved += OnEntityHasMoved;
		Prompts.Instance.OnSpecialActionInitiated += OnSpecialActionInitiated;
	}

	void OnDestroy()
	{
		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnPhaseStart -= OnPhaseStart;
			GameManager.Instance.OnActionTaken -= OnActionTaken;
		}
		if (Prompts.Instance != null)
		{
			Prompts.Instance.OnSpecialActionInitiated -= OnSpecialActionInitiated;
		}
	}

	void Start ()
	{
		policeManager = GameObject.FindObjectOfType<PoliceManager>();

		for (int i = 0; i < GameManager.Instance.GetEmbassies().Count; i++)
		{
			if (GameManager.Instance.GetEmbassies()[i] != this)
			{
				opposingEmbassy = GameManager.Instance.GetEmbassies()[i];
				break;
			}
		}
		
		for (int i = 0; i < GameManager.Instance.GetGameOptions().numSpiesPerTeam; i++)
		{
			GameObject go = Instantiate(spyPrefab) as GameObject;
			Spy s = go.GetComponent<Spy>();
			s.InitializeSpy(GetComponent<GraphNode>(), myTeam, this);
			mySpies.Add(s);
		}

		numActionsRemaining = numActionsPerTurn;
		myEmbassyNode = GetComponent<GraphNode>();
	}

	public void ArrestSpy(Spy _spyArrested, Entity _entityArresting)		// entity arresting can be null, in which case just assume it's the opposing team
	{
		// determine whether arrested by spy or police
		Spy _arrestingSpy = _entityArresting as Spy;
		Police _arrestingPolice = _entityArresting as Police;

		GameManager.Team _arrestingTeam;
		if (_arrestingSpy != null)
		{
			_arrestingTeam = _arrestingSpy.myTeam;
		}
		else if (_arrestingPolice != null)
		{
			_arrestingTeam = GameManager.Team.NEUTRAL;
		}
		else
		{
			_arrestingTeam = opposingEmbassy.myTeam;
		}

		GameManager.Instance.ReportActionTaken(_arrestingTeam, GameManager.ActionType.ARREST, _spyArrested.currentNode.GetNodeTeamAssociation());

		if (GameManager.Instance.GetGameOptions().spyReturnToEmbassyOnArrest)
		{
			_spyArrested.Move(myEmbassyNode);
			_spyArrested.SpyOutForTurns(GameManager.Instance.GetGameOptions().numTurnsArrested);
		}
		else
		{
			mySpies.Remove(_spyArrested);
			Destroy(_spyArrested.gameObject);
		}
	}
	
	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		numActionsRemaining = numActionsPerTurn;
		CheckSpecialActionsToShow();

		if (_phase == GameManager.RoundPhase.PLAYERTURN)
		{
			if (_team == myTeam)
			{
				turnSummary.Clear();
				opposingEmbassy.DisplayTurnSummary();
			}
		}
	}

	public bool HasActionRemaining()
	{
		return (numActionsRemaining > 0);
	}

	public bool RequestSpySelection(Spy _s)
	{
		// TODO: confirm that no other spy is selected or is in some other game mode

		return true;
	}

	public bool RequestSpyMovement(Spy _s, GraphNode _node)
	{
		// TODO: confirm valid movement

		// confirm one space movement
		if (!GameManager.Instance.graphManager.IsNodesConnected(_s.currentNode, _node))
		{
			return false;
		}

		// confirm no police are occupying
		if (policeManager.IsPoliceHere(_node))
		{
			return false;
		}

		// confirm if movement into opposing embassy, fulfills winning requirements
		Embassy _enteringEmbassy = _node.GetComponent<Embassy>();
		if (_enteringEmbassy && _enteringEmbassy == opposingEmbassy)
		{
			if (GameManager.Instance.GetGameOptions().requireOneArrestToWin && numEnemySpiesArrested < 1)
			{
				return false;
			}
		}

		return true;
	}

	void CheckSpecialActionsToShow()
	{
		if (GameManager.Instance.currentPlayerTurn == myTeam)
		{
			if (numActionsRemaining > 0)
			{
				List<Spy> validArrests = FindValidArrests();
				bool isValidArrests = validArrests.Count > 0;
				Prompts.Instance.ShowActionArrestButton(isValidArrests);
			}
			else
			{
				Prompts.Instance.ShowActionArrestButton(false);
			}

			Prompts.Instance.ShowActionBonusActionButton(mySpecialActions.Contains(GameManager.Pickups.EXTRA_ACTION));
		}


	}

	public void OnActionTaken(GameManager.Team _team, GameManager.ActionType _action, GameManager.Team _buildingTeam)
	{
		if (_team == myTeam)
		{
			// spends action?
			if (_action == GameManager.ActionType.MOVE || _action == GameManager.ActionType.ARREST)
			{
				numActionsRemaining--;
				GameManager.Instance.ReportStatUpdated();
			}

			// recorded in turn summary?
			if (_action == GameManager.ActionType.MOVE || _action == GameManager.ActionType.ARREST)
			{
				ActionRecord thisAction = new ActionRecord();
				thisAction.actionType = _action;
				thisAction.buildingTeam = _buildingTeam;
				turnSummary.Add(thisAction);
			}

			if (_action == GameManager.ActionType.ARREST)
			{
				numEnemySpiesArrested++;
			}
		}

		CheckSpecialActionsToShow();
	}

	public void DisplayTurnSummary()
	{
		Prompts.Instance.ShowTurnSummary(turnSummary);
	}

	private List<Spy> FindValidArrests()
	{
		List<Spy> retList = new List<Spy>();
		List<GraphNode> arrestableNodes = new List<GraphNode>();
		
		List<GraphNode> nodesMyTeamOccupies = GetListOfNodesMyTeamOccupies();
		List<GraphNode> nodesOpponentOccupies = opposingEmbassy.GetListOfNodesMyTeamOccupies();
		for (int i = 0; i < nodesMyTeamOccupies.Count; i++)
		{
			for (int j = 0; j < nodesOpponentOccupies.Count; j++)
			{
				if (nodesMyTeamOccupies[i] == nodesOpponentOccupies[j])
				{
					arrestableNodes.Add(nodesMyTeamOccupies[i]);
				}
			}
		}
		for (int i = 0; i < arrestableNodes.Count; i++)
		{
			List<Entity> arrestableEntities = arrestableNodes[i].GetOccupyingEntities();
			for (int j = 0; j < arrestableEntities.Count; j++)
			{
				Spy s = arrestableEntities[j].GetComponent<Spy>();
				if (s != null && s.myTeam != myTeam)
				{
					retList.Add(s);
				}
			}
		}

		return retList;
	}

	public List<GraphNode> GetListOfNodesMyTeamOccupies()
	{
		List<GraphNode> retList = new List<GraphNode>();
		for (int i = 0; i < mySpies.Count; i++)
		{
			if (!retList.Contains(mySpies[i].currentNode))
			{
				retList.Add(mySpies[i].currentNode);
			}
		}
		return retList;
	}

	public int GetNumActionsRemaining()
	{
		return numActionsRemaining;
	}

	public List<Spy> GetMySpies()
	{
		return mySpies;
	}

	private void OnSpecialActionInitiated(GameManager.ActionType _action)
	{
		if (GameManager.Instance.currentPlayerTurn == myTeam)
		{
			if (_action == GameManager.ActionType.ARREST)
			{
				List<Spy> validArrests = FindValidArrests();
				for (int i = 0; i < validArrests.Count; i++)
				{
					validArrests[i].ShowSpyCanvas(true);
				}
			}
			else if (_action == GameManager.ActionType.BONUS_ACTION)
			{
				if (mySpecialActions.Contains(GameManager.Pickups.EXTRA_ACTION))
				{
					ApplyBonusAction();
				}
				else
				{
					Debug.LogError("Embassy attempting to use bonus action, but doesn't seem to have any");
				}
			}
		}
	}

	protected void OnEntityHasMoved(GraphNode _fromNode, GraphNode _toNode, Entity _entity)
	{
		Spy _spy = _entity as Spy;
		if (mySpies.Contains(_spy))
		{
			Embassy _embassy = _toNode.GetComponent<Embassy>();
			Building _building = _toNode.GetComponent<Building>();

			if (_embassy != null && _embassy == opposingEmbassy)
			{
				numSpiesReachedOpposingEmbassy++;

				if (numSpiesReachedOpposingEmbassy >= GameManager.Instance.GetGameOptions().numSpiesRequiredToReachEmbassy)
				{
					GameManager.Instance.ReportGameHasBeenWon(myTeam);
				}
			}
			else if (_building != null)
			{
				if (_building.IsPickupAvailable())
				{
					mySpecialActions.Add(_building.TakePickup());
				}
			}
		}
	}

	private void ApplyBonusAction()
	{
		numActionsRemaining++;
		mySpecialActions.Remove(GameManager.Pickups.EXTRA_ACTION);
		GameManager.Instance.ReportActionTaken(myTeam, GameManager.ActionType.BONUS_ACTION, GameManager.Team.NEUTRAL);
	}

	public void Debug_ApplyBonusAction()
	{
		Debug.LogWarning("Debug Button Used: Add bonus action");
		ApplyBonusAction();
	}
}

public struct ActionRecord
{
	public GameManager.ActionType actionType;
	public GameManager.Team buildingTeam;
}
