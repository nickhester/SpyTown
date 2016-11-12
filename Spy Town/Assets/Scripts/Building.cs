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
		GameOptions gameOptions = GameManager.Instance.GetGameOptions();

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
		// spawn building canvas
		if (myCanvas == null)
		{
			myCanvas = Instantiate(canvasPrefab) as GameObject;
			myCanvas.transform.position += transform.position;
		}

		// TODO: choose pickup type procedurally
		myPickupType = GameManager.Pickups.EXTRA_ACTION;
		isPickupAvailable = true;
	}

	public bool IsPickupAvailable()
	{
		return isPickupAvailable;
	}

	public GameManager.Pickups TakePickup()
	{
		isPickupAvailable = false;
		Destroy(myCanvas);
		myCanvas = null;
		return myPickupType;
	}
}
