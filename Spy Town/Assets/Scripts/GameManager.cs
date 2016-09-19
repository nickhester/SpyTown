using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public List<Entity> allEntities = new List<Entity>();

	// delegates
	public delegate void OnPhaseStartEvent(RoundPhase _phase, Team _team);
	public event OnPhaseStartEvent OnPhaseStart;
	public delegate void OnEntityHasMovedEvent(GraphNode _graphNode);
	public event OnEntityHasMovedEvent OnEntityHasMoved;
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


	void Start ()
	{
		allEntities.AddRange(GameObject.FindObjectsOfType<Entity>());
		OnPhaseStart(currentPhase, currentPlayerTurn);
	}
	
	void Update ()
	{
		
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
	}

	public void ReportEntityHasMoved(GraphNode _n)
	{
		OnEntityHasMoved(_n);
	}
}
