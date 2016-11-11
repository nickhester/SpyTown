using UnityEngine;
using System.Collections;

public class GameOptions : MonoBehaviour
{
	// game balance settings
	public int numSpiesPerTeam = 3;
	public int numActionsPerTurn = 3;
	public int numSpiesRequiredToReachEmbassy = 1;
	public int policeSightDistance = 1;
	public bool policeReturnToEmbassyOnArrest = true;

	// game view settings
	public bool viewModeBoardGame = true;	// in this mode, player 2 views everything "upside down" so the device can be handed back and forth directly

	public Color primaryTeamColor;
	public Color secondaryTeamColor;
	public Color neutralColor;

	void Start()
	{
		if (policeSightDistance != 1)
		{
			Debug.LogWarning("Police Sight Can't Be Set Above 1 Right Now");
		}
	}
}
