using UnityEngine;
using System.Collections;
using System;

public class GameOptions : MonoBehaviour
{
	// game balance settings
	public int numSpiesPerTeam = 3;
	public int numActionsPerTurn = 3;
	public int numSpiesRequiredToReachEmbassy = 1;
	public int policeSightDistance = 1;
	public bool spyReturnToEmbassyOnArrest = true;
	public bool requireOneArrestToWin = true;
	public int numTurnsArrested = 2;

	// game view settings
	public bool viewModeBoardGame = true;	// in this mode, player 2 views everything "upside down" so the device can be handed back and forth directly

	public Color primaryTeamColor;
	public Color secondaryTeamColor;
	public Color neutralColor;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		
		if (policeSightDistance != 1)
		{
			Debug.LogWarning("Police Sight Can't Be Set Above 1 Right Now");
		}
	}

	// setters

	public void Toggle_spyReturnToEmbassyOnArrest(bool b)
	{
		spyReturnToEmbassyOnArrest = b;
	}

	public void Toggle_requireOneArrestToWin(bool b)
	{
		requireOneArrestToWin = b;
	}

	public void Toggle_viewModeBoardGame(bool b)
	{
		viewModeBoardGame = b;
	}

	public void Slider_NumTurnsArrested(Single n)
	{
		numTurnsArrested = (int)n;
	}
}
