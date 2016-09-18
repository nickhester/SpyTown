using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	protected GameManager gameManager;
	protected GraphNode currentNode;

	public float pushAwayRadius = 2.0f;
	private List<GameObject> currentPushAwayObjects = new List<GameObject>();
	public float pushAwaySpeed = 1.0f;

	void Start ()
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
		FindPushAwayTargets();
	}
	
	void Update ()
	{
		ContinuousPushAway();
	}

	void FindPushAwayTargets()
	{
		for (int i = 0; i < gameManager.allEntities.Count; i++)
		{
			if (gameManager.allEntities[i].currentNode == currentNode && gameManager.allEntities[i] != this)
			{
				currentPushAwayObjects.Add(gameManager.allEntities[i].gameObject);
			}
		}
	}

	void ContinuousPushAway()
	{
		for (int i = 0; i < currentPushAwayObjects.Count; i++)
		{
			Vector3 differenceVector = transform.position - currentPushAwayObjects[i].transform.position;
			if (differenceVector.magnitude == 0.0f)
			{
				transform.position +=  new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized * pushAwaySpeed * Time.deltaTime;
			}
			else if (differenceVector.magnitude < pushAwayRadius)
			{
				transform.position += differenceVector.normalized * pushAwaySpeed * Time.deltaTime;
			}
			else if (differenceVector.magnitude >= pushAwayRadius)
			{
				currentPushAwayObjects.RemoveAt(i);
				i--;
			}
		}
		
	}

	protected void ActivateEntity(bool _activate)
	{
		GetComponent<Collider>().enabled = _activate;
		GetComponent<Renderer>().enabled = _activate;
	}
}
