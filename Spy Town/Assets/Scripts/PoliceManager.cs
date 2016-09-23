using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceManager : MonoBehaviour
{
	public Police policePrefab;
	public List<GraphNode> startingBuildings = new List<GraphNode>();
	private List<Police> allPolice = new List<Police>();

	protected void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
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
		for (int i = 0; i < allPolice.Count; i++)
		{
			List<GraphNode> _connectedNodes = allPolice[i].currentNode.GetConnectedNodes();

			for (int j = 0; j < _connectedNodes.Count; j++)
			{
				// if it's an embassy, or it's a non-neutral building, remove it as a possibility
				if (_connectedNodes[j].GetComponent<Embassy>() != null
					|| _connectedNodes[j].GetComponent<Building>().teamAssociation != GameManager.Team.NEUTRAL)
				{
					_connectedNodes.RemoveAt(j);
					j--;
				}
			}

			if (_connectedNodes.Count > 0)
			{
				GraphNode newNode = _connectedNodes[Random.Range(0, _connectedNodes.Count)];
				allPolice[i].MoveToNewNode(newNode);
			}
		}
	}
}
