using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphNode : MonoBehaviour
{
	public List<GraphNode> connectedNodes = new List<GraphNode>();

	void Start ()
	{
		for (int i = 0; i < connectedNodes.Count; i++)
		{
			Debug.DrawLine(transform.position, connectedNodes[i].transform.position, Color.green, 30.0f);

			connectedNodes[i].AddConnectedNode(this);
		}
	}
	
	void Update ()
	{
		
	}

	public void AddConnectedNode(GraphNode _node)
	{
		if (!connectedNodes.Contains(_node))
		{
			connectedNodes.Add(_node);
		}
	}
}
