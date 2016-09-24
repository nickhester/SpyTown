using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Embassy : MonoBehaviour
{
	public GameManager.Team myTeam;
	public int numStartingSpies = 3;
	public GameObject spyPrefab;
	private List<Spy> mySpies = new List<Spy>();
	private PoliceManager policeManager;
	private Embassy opposingEmbassy;

	public int numActionsPerTurn;
	private int numActionsRemaining;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnActionTaken += OnActionTaken;
		Prompts.Instance.OnSpecialActionInitiated += OnSpecialActionInitiated;
	}

	void OnDestroy()
	{
		GameManager.Instance.OnPhaseStart -= OnPhaseStart;
		GameManager.Instance.OnActionTaken -= OnActionTaken;
		Prompts.Instance.OnSpecialActionInitiated -= OnSpecialActionInitiated;
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

		for (int i = 0; i < numStartingSpies; i++)
		{
			GameObject go = Instantiate(spyPrefab) as GameObject;
			Spy s = go.GetComponent<Spy>();
			s.InitializeSpy(GetComponent<GraphNode>(), myTeam, this);
			mySpies.Add(s);
		}

		numActionsRemaining = numActionsPerTurn;
	}

	public void ArrestSpy(Spy _spy)
	{
		mySpies.Remove(_spy);
		Destroy(_spy.gameObject);
		GameManager.Instance.ReportActionTaken(myTeam, Action.Actions.ARREST);
	}
	
	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		numActionsRemaining = numActionsPerTurn;

		CheckSpecialActionsToShow();
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

		return true;
	}

	void CheckSpecialActionsToShow()
	{
		if (GameManager.Instance.currentPlayerTurn == myTeam && numActionsRemaining > 0)
		{
			// TODO: (minor optimization) could cache this list somewhere so it doesn't have to be regenerated again later
			List<Spy> validArrests = FindValidArrests();
			bool isValidArrests = validArrests.Count > 0;
			Prompts.Instance.ShowActionArrest(isValidArrests);
		}
		else
		{
			Prompts.Instance.ShowActionArrest(false);
		}
	}

	public void OnActionTaken(GameManager.Team _team, Action.Actions _action)
	{
		if (_team == myTeam)
		{
			numActionsRemaining--;

			CheckSpecialActionsToShow();
		}
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

	private void OnSpecialActionInitiated(Action.Actions _action)
	{
		if (GameManager.Instance.currentPlayerTurn == myTeam)
		{
			List<Spy> validArrests = FindValidArrests();
			for (int i = 0; i < validArrests.Count; i++)
			{
				validArrests[i].ShowPlayerCanvas(true);
			}
		}
	}
}
