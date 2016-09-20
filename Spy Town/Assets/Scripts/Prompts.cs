using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Prompts : MonoBehaviour
{
	public Text playerPromptMessage;
	public Button playerAcceptButton;
	public Button turnDoneButton;
	public GameObject PlayerTurnSet;
	public GameObject MidturnSet;

	public string playerPrimaryStartMessage;
	public string playerSecondaryStartMessage;
	public string playerStartButtonMessage;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
	}

	void Start ()
	{
		
	}
	
	void Update ()
	{
	
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		TurnOffAllPrompts();

		if (_phase == GameManager.RoundPhase.MIDTURN || _phase == GameManager.RoundPhase.START)
		{
			MidturnSet.SetActive(true);

			if (_team == GameManager.Team.PRIMARY)
			{
				playerPromptMessage.text = playerPrimaryStartMessage;
				playerAcceptButton.GetComponentInChildren<Text>().text = playerStartButtonMessage;
			}
			else
			{
				playerPromptMessage.text = playerSecondaryStartMessage;
				playerAcceptButton.GetComponentInChildren<Text>().text = playerStartButtonMessage;
			}
		}
		else
		{
			PlayerTurnSet.SetActive(true);
		}
	}

	void TurnOffAllPrompts()
	{
		PlayerTurnSet.SetActive(false);
		MidturnSet.SetActive(false);
	}
}
