using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceManager : MonoBehaviour
{
	public Police policePrefab;
	public List<GraphNode> startingBuildings = new List<GraphNode>();
	private List<Police> allPolice = new List<Police>();

	void Start ()
	{
		for (int i = 0; i < startingBuildings.Count; i++)
		{
			Police p = Instantiate(policePrefab.gameObject).GetComponent<Police>();
			p.InitializePolice(startingBuildings[i], this);
		}
	}
}
