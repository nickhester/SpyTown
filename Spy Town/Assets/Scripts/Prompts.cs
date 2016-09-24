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
	public Text actionsLeft;

	public Button actionArrest;

	public string playerPrimaryStartMessage;
	public string playerSecondaryStartMessage;
	public string playerStartButtonMessage;

    private GameManager.Team nextTeamUp;

	// delegates
	public delegate void OnSpecialActionInitiatedEvent(Action.Actions _action);
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
		if (GameManager.Instance != null)
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
		ShowActionArrest(false);
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
            // don't do anything till police report completion
        }
		else
		{
			PlayerTurnSet.SetActive(true);
		}
	}

	void OnActionTaken(GameManager.Team _team, Action.Actions _action)
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
	}

    void OnPoliceMovementComplete()
    {
        ShowPlayerStartUI();
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

	public void ShowActionArrest(bool _b)
	{
		actionArrest.gameObject.SetActive(_b);
	}

	public void ButtonPressArrest()
	{
		// trigger event
		OnSpecialActionInitiated(Action.Actions.ARREST);
	}
}
