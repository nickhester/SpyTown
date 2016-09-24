using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
	private List<GraphNode> nodes = new List<GraphNode>();

	void Start ()
	{
		nodes.AddRange(GameObject.FindObjectsOfType<GraphNode>());
	}

	public void RevealAll(bool b)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].Reveal(false);
		}
	}

	public List<GraphNode> GetAllNodes()
	{
		return nodes;
	}

	/*
	public int GetNodeDistance(GraphNode _start, GraphNode _end)
	{

	}
	*/

	public bool IsNodesConnected(GraphNode _n1, GraphNode _n2)
	{
		List<GraphNode> _nodes = _n1.GetConnectedNodes();
		for (int i = 0; i < _nodes.Count; i++)
		{
			if (_nodes[i] == _n2)
			{
				return true;
			}
		}
		return false;
	}
}
