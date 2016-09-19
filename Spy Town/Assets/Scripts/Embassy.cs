using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Embassy : MonoBehaviour
{
	public GameManager.Team myTeam;
	public int numStartingSpies = 3;
	public GameObject spyPrefab;
	private List<Spy> mySpies = new List<Spy>();

	public int numActionsPerTurn;
	private int numActionsRemaining;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
	}

	void Start ()
	{
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

		return true;
	}

	public void OnActionSpent()
	{
		numActionsRemaining--;
	}
}
