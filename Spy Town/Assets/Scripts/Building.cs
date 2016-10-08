using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
	public GameManager.Team teamAssociation;

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
	}
}
