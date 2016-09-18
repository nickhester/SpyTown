using UnityEngine;
using System.Collections;

public class Spy : Entity
{
	private GameManager.Team myTeam;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
	}

	public void InitializeSpy(GraphNode _startingNode, GameManager.Team _team)
	{
		currentNode = _startingNode;
		myTeam = _team;
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		if (_phase == GameManager.RoundPhase.PLAYERTURN)
		{
			// activate entity if it's their turn
			ActivateEntity(_team == myTeam);
		}
	}
}
