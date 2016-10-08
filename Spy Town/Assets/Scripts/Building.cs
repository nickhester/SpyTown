using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
	public GameManager.Team teamAssociation;
	public bool generateRandomPickup = false;
	private GameManager.Pickups myPickupType;
	private bool isPickupAvailable = false;
	public GameObject canvasPrefab;
	private GameObject myCanvas;

	void Start()
	{
		GameOptions gameOptions = GameObject.FindObjectOfType<GameOptions>();

		if (teamAssociation == GameManager.Team.PRIMARY)
		{
			GetComponent<Renderer>().material.color = gameOptions.primaryTeamColor;
		}
		else if (teamAssociation == GameManager.Team.SECONDARY)
		{
			GetComponent<Renderer>().material.color = gameOptions.secondaryTeamColor;
		}

		if (generateRandomPickup)
		{
			AddPickup();
		}
	}

	void AddPickup()
	{
		// TODO: spawn building canvas


		// TODO: choose pickup type procedurally
		myPickupType = GameManager.Pickups.EXTRA_ACTION;
	}
}
