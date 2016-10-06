using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Entity : MonoBehaviour
{
	public GraphNode currentNode;

	public float pushAwayRadius = 2.0f;
	private List<GameObject> currentPushAwayObjects = new List<GameObject>();
	public float pushAwaySpeed = 1.0f;

	private Renderer myMesh = null;
    private Canvas myCanvas = null;

	protected void Awake()
	{
		GameManager.Instance.OnEntityHasMoved += OnEntityHasMoved;
	}

	protected void OnDestroy()
	{
		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnEntityHasMoved -= OnEntityHasMoved;
		}
	}

	protected void Start ()
	{
		FindPushAwayTargets();
    }
	
	protected void Update ()
	{
		ContinuousPushAway();
	}

	protected Renderer GetMyMesh()
	{
		if (myMesh == null)
		{
			myMesh = GetComponentInChildren<Renderer>();
		}
		return myMesh;
	}

    protected Canvas GetMyCanvas()
    {
        if (myCanvas == null)
        {
            myCanvas = GetComponentInChildren<Canvas>();
        }
        return myCanvas;
    }

    void FindPushAwayTargets()
	{
		List<Entity> allEntities = GameManager.Instance.GetAllEntities();
		for (int i = 0; i < allEntities.Count; i++)
		{
			if (allEntities[i].currentNode == currentNode && GameManager.Instance.GetAllEntities()[i] != this)
			{
				if (allEntities[i] != null)
				{
					currentPushAwayObjects.Add(allEntities[i].gameObject);
				}
			}
		}
	}

	void ContinuousPushAway()
	{
		for (int i = 0; i < currentPushAwayObjects.Count; i++)
		{
			if (currentPushAwayObjects[i] != null)
			{
				Vector3 differenceVector = transform.position - currentPushAwayObjects[i].transform.position;
				if (differenceVector.magnitude == 0.0f)
				{
					transform.position += new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized * pushAwaySpeed * Time.deltaTime;
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
		
	}

	protected void ActivateEntity(bool _activate)
	{
		GetMyMesh().GetComponent<Collider>().enabled = _activate;
		RevealEntity(_activate);
	}

	protected void RevealEntity(bool b)
	{
		GetMyMesh().enabled = b;
	}

	protected void ResetPosition()
	{
		transform.position = currentNode.transform.position;
		FindPushAwayTargets();
	}

	public virtual void Move(GraphNode _newNode)
	{
        GraphNode oldNode = currentNode;
		currentNode = _newNode;
		ResetPosition();
		GameManager.Instance.ReportEntityHasMoved(oldNode, _newNode, this);
	}

	virtual protected void OnEntityHasMoved(GraphNode _fromNode, GraphNode _toNode, Entity _entity)
	{
		FindPushAwayTargets();
	}
}
