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
	
	void Update ()
	{
		
	}
}
