using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
	List<GraphNode> nodes = new List<GraphNode>();

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
}
