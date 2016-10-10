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
	public GameObject MidturnReadySet;
	public GameObject PreviousTurnSummarySet;
	public Text actionsLeft;

	public Button actionArrest;
	public Button actionUseBonusAction;

	public string playerPrimaryStartMessage;
	public string playerSecondaryStartMessage;
	public string playerStartButtonMessage;

    private GameManager.Team nextTeamUp;

	[SerializeField] private Text previousTurnSummaryDescription;

	// delegates
	public delegate void OnSpecialActionInitiatedEvent(Action.ActionType _action);
	public event OnSpecialActionInitiatedEvent OnSpecialActionInitiated;
	// singleton
	private static Prompts instance;
	// instance
	public static Prompts Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(Prompts)) as Prompts;
			}
			return instance;
		}
	}

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnActionTaken += OnActionTaken;
        PoliceManager.Instance.OnPoliceMovementComplete += OnPoliceMovementComplete;
    }

	void OnDestroy()
	{
		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnPhaseStart -= OnPhaseStart;
			GameManager.Instance.OnActionTaken -= OnActionTaken;
		}
		if (PoliceManager.Instance != null)
		{
			PoliceManager.Instance.OnPoliceMovementComplete -= OnPoliceMovementComplete;
		}
	}

	void Start()
	{
		ShowActionArrestButton(false);
		ShowActionBonusActionButton(false);
	}

	// event on start of a new phase
	void OnPhaseStart(GameManager.RoundPhase _phase, GameManager.Team _team)
	{
		TurnOffAllPrompts();
		UpdateActionsLeft();

        nextTeamUp = _team;


		if (_phase == GameManager.RoundPhase.START)
		{
			ShowPlayerStartUI();
		}
		else if (_phase == GameManager.RoundPhase.MIDTURN)
		{
			ShowMidturnReadyUI();
			HideTurnSummary();
		}
		else
		{
			PlayerTurnSet.SetActive(true);
		}
	}

	void OnActionTaken(GameManager.Team _team, Action.ActionType _action, GameManager.Team _buildingTeam)
	{
		UpdateActionsLeft();
	}

	void UpdateActionsLeft()
	{
		Embassy e = GameManager.Instance.GetActiveEmbassy();
		if (e != null)
		{
			actionsLeft.text = "Actions Left: " + e.GetNumActionsRemaining();
		}
	}

	void TurnOffAllPrompts()
	{
		PlayerTurnSet.SetActive(false);
		MidturnSet.SetActive(false);
		MidturnReadySet.SetActive(false);
	}

	public void TriggerPoliceMovementStart()
	{
		TurnOffAllPrompts();
		PoliceManager.Instance.TakePoliceTurn();
	}

    void OnPoliceMovementComplete()
    {
        ShowPlayerStartUI();
    }

	void ShowMidturnReadyUI()
	{
		MidturnReadySet.SetActive(true);
	}

    void ShowPlayerStartUI()
    {
        MidturnSet.SetActive(true);

        if (nextTeamUp == GameManager.Team.PRIMARY)
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

	public void ShowActionArrestButton(bool _b)
	{
		actionArrest.gameObject.SetActive(_b);
	}

	public void ShowActionBonusActionButton(bool _b)
	{
		actionUseBonusAction.gameObject.SetActive(_b);
	}

	public void ButtonPressArrest()
	{
		// trigger event
		OnSpecialActionInitiated(Action.ActionType.ARREST);
	}

	public void ButtonPressBonusAction()
	{
		// trigger event
		OnSpecialActionInitiated(Action.ActionType.BONUS_ACTION);
	}

	public void ShowTurnSummary(string s)
	{
		PreviousTurnSummarySet.SetActive(true);
		previousTurnSummaryDescription.text = s;
	}

	public void HideTurnSummary()
	{
		PreviousTurnSummarySet.SetActive(false);
	}
}
