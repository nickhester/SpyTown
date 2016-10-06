using UnityEngine;
using System.Collections;

public class GameOptions : MonoBehaviour
{
	public int numSpiesPerTeam = 3;
	public int numActionsPerTurn = 3;
	public int numSpiesRequiredToReachEmbassy = 1;
	public int policeSightDistance = 1;

	void Start()
	{
		if (policeSightDistance != 1)
		{
			Debug.LogWarning("Police Sight Can't Be Set Above 1 Right Now");
		}
	}
}
