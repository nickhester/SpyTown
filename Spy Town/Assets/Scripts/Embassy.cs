using UnityEngine;
using System.Collections;

public class Embassy : MonoBehaviour
{
	public GameManager.Team myTeam;
	public int numStartingSpies = 3;
	public GameObject spyPrefab;

	void Start ()
	{
		for (int i = 0; i < numStartingSpies; i++)
		{
			GameObject go = Instantiate(spyPrefab, transform.position, Quaternion.identity) as GameObject;
			go.GetComponent<Spy>().InitializeSpy(GetComponent<GraphNode>(), myTeam);
		}
	}
	
	void Update ()
	{
	
	}
}
