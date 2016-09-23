using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Embassy : MonoBehaviour
{
	public GameManager.Team myTeam;
	public int numStartingSpies = 3;
	public GameObject spyPrefab;
	private List<Spy> mySpies = new List<Spy>();
	private PoliceManager policeManager;

	public int numActionsPerTurn;
	private int numActionsRemaining;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnActionTaken += OnActionTaken;
	}

	void Start ()
	{
		policeManager = GameObject.FindObjectOfType<PoliceManager>();

		for (int i = 0; i < numStartingSpies; i++)
		{
			GameObject go = Instantiate(spyPrefab) as GameObject;
			Spy s = go.GetComponent<Spy>();
			s.InitializeSpy(GetComponent<GraphNode>(), myTeam, this);
			mySpies.Add(s);
		}

		numActionsRemaining = numActionsPerTurn;
	}
	
	void Update ()
	{
	
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		numActionsRemaining = numActionsPerTurn;
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

	public void OnActionTaken()
	{
		numActionsRemaining--;
	}

	public int GetNumActionsRemaining()
	{
		return numActionsRemaining;
	}
}
