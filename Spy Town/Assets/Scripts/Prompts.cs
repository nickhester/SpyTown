using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Prompts : MonoBehaviour
{
	public GameManager gameManager;

	public Text playerPromptMessage;
	public Button playerAcceptButton;
	public Button turnDoneButton;
	public GameObject PlayerTurnSet;
	public GameObject MidturnSet;
	public Text actionsLeft;

	public string playerPrimaryStartMessage;
	public string playerSecondaryStartMessage;
	public string playerStartButtonMessage;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnActionTaken += OnActionTaken;
	}

	void Start()
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		TurnOffAllPrompts();
		UpdateActionsLeft();

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

	void OnActionTaken()
	{
		UpdateActionsLeft();
	}

	void UpdateActionsLeft()
	{
		Embassy e = gameManager.GetActiveEmbassy();
		if (e != null)
		{
			actionsLeft.text = "Actions Left: " + e.GetNumActionsRemaining();
		}
	}

	void TurnOffAllPrompts()
	{
		PlayerTurnSet.SetActive(false);
		MidturnSet.SetActive(false);
	}
}
