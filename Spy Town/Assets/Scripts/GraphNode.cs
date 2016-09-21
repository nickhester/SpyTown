using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphNode : MonoBehaviour
{
	private GameManager gameManager;
	public List<GraphNode> connectedNodes = new List<GraphNode>();
	public bool isRevealed = false;

	void Awake ()
	{
		GameManager.Instance.OnEntityHasMoved += OnEntityHasMoved;
	}

	void Start ()
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();

		for (int i = 0; i < connectedNodes.Count; i++)
		{
			Debug.DrawLine(transform.position, connectedNodes[i].transform.position, Color.green, 30.0f);

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

	void OnEntityHasMoved(GraphNode _node)
	{
		//
	}
}
