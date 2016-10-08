using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphNode : MonoBehaviour
{
	[SerializeField]
	private List<GraphNode> connectedNodes = new List<GraphNode>();
	public bool isRevealed = false;

	void Start ()
	{
		for (int i = 0; i < connectedNodes.Count; i++)
		{
			Debug.DrawLine(transform.position, connectedNodes[i].transform.position, Color.green, 30.0f);

			LineGeneric.CreateLineMesh(transform.position, connectedNodes[i].transform.position, 1.0f, 1.0f, 1.0f, Vector3.up);

			connectedNodes[i].AddConnectedNode(this);
		}
	}

	public void AddConnectedNode(GraphNode _node)
	{
		if (!connectedNodes.Contains(_node))
		{
			connectedNodes.Add(_node);
		}
	}

	public List<GraphNode> GetConnectedNodes()
	{
		return connectedNodes;
	}

	public bool GetIsRevealed()
	{
		return isRevealed;
	}

	public void Reveal(bool b)
	{
		isRevealed = b;
	}

	public List<Entity> GetOccupyingEntities()
	{
		// this method is inefficient, but quick and easy for now
		List<Entity> allEntities = new List<Entity>();
		List<Entity> retList = new List<Entity>();
		allEntities.AddRange(GameObject.FindObjectsOfType<Entity>());
		for (int i = 0; i < allEntities.Count; i++)
		{
			if (allEntities[i].currentNode == this)
			{
				retList.Add(allEntities[i]);
			}
		}
		return retList;
	}

	public GameManager.Team GetNodeTeamAssociation()
	{
		Building b = GetComponent<Building>();
		if (b != null)
		{
			return b.teamAssociation;
		}

		Embassy e = GetComponent<Embassy>();
		if (e != null)
		{
			return e.myTeam;
		}
		Debug.LogError("Graph Node " + gameObject.name + " has no building or embassy");
		return GameManager.Team.NEUTRAL;
	}
}
