using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceManager : MonoBehaviour
{
	public Police policePrefab;
	public List<GraphNode> startingBuildings = new List<GraphNode>();
	private List<Police> allPolice = new List<Police>();

    [SerializeField]
    private float timeBeforePoliceMove;
    [SerializeField]
    private float movementDuration;
    
    // delegates
    public delegate void OnPoliceMovementCompleteEvent();
    public event OnPoliceMovementCompleteEvent OnPoliceMovementComplete;
    // singleton
    private static PoliceManager instance;
    // instance
    public static PoliceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(PoliceManager)) as PoliceManager;
            }
            return instance;
        }
    }

    private int numPoliceWaitingToComplete = 0;

    protected void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
	}

	protected void OnDestroy()
	{
		GameManager.Instance.OnPhaseStart -= OnPhaseStart;
	}

	private void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		if (_phase == GameManager.RoundPhase.MIDTURN)
		{
			TakePoliceTurn();
		}
	}

	void Start ()
	{
		for (int i = 0; i < startingBuildings.Count; i++)
		{
			Police p = Instantiate(policePrefab.gameObject).GetComponent<Police>();
			p.InitializePolice(startingBuildings[i], this);
			allPolice.Add(p);
		}
	}

	public List<Police> GetAllPolice()
	{
		return allPolice;
	}

	public List<GraphNode> GetAllPoliceLocations()
	{
		List<GraphNode> nodes = new List<GraphNode>();
		for (int i = 0; i < allPolice.Count; i++)
		{
			nodes.Add(allPolice[i].currentNode);
		}
		return nodes;
	}

	public bool IsPoliceHere(GraphNode _node)
	{
		for (int i = 0; i < allPolice.Count; i++)
		{
			if (allPolice[i].currentNode == _node)
			{
				return true;
			}
		}
		return false;
	}

	void TakePoliceTurn()
	{
		List<GraphNode> chosenDestinations = new List<GraphNode>();
		for (int i = 0; i < allPolice.Count; i++)
		{
            List<GraphNode> _connectedNodes = new List<GraphNode>();
            _connectedNodes.AddRange(allPolice[i].currentNode.GetConnectedNodes());
            
            // remove invalid possibile destinations
			for (int j = 0; j < _connectedNodes.Count; j++)
			{
				if (_connectedNodes[j].GetComponent<Embassy>() != null												// if it's an embassy
					|| _connectedNodes[j].GetComponent<Building>().teamAssociation != GameManager.Team.NEUTRAL		// if it's not a neutral building
					|| chosenDestinations.Contains(_connectedNodes[j])) 				            				// if it's already been chosen as another police's destination
				{
					_connectedNodes.RemoveAt(j);
					j--;
				}
			}

			if (_connectedNodes.Count > 0)
			{
				GraphNode newNode = _connectedNodes[Random.Range(0, _connectedNodes.Count)];
				StartCoroutine(allPolice[i].MoveToNewNode(newNode, timeBeforePoliceMove, movementDuration));
                numPoliceWaitingToComplete++;
                chosenDestinations.Add(newNode);
			}
		}
	}

    public void PoliceReportMovementComplete()
    {
        numPoliceWaitingToComplete--;

        if (numPoliceWaitingToComplete <= 0)
        {
            OnPoliceMovementComplete();
        }
    }
}
