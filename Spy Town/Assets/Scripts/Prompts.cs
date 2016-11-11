using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Prompts : MonoBehaviour
{

	[SerializeField] private Text playerPromptMessage;
	[SerializeField] private Button playerAcceptButton;
	[SerializeField] private Button turnDoneButton;
	[SerializeField] private GameObject PlayerTurnSet;
	[SerializeField] private GameObject MidturnSet;
	[SerializeField] private GameObject MidturnReadySet;
	[SerializeField] private GameObject PreviousTurnSummarySet;
	[SerializeField] private GameObject GameWonSet;

	[SerializeField] private Text actionsLeft;

	[SerializeField] private Button actionArrest;
	[SerializeField] private Button actionUseBonusAction;

	[SerializeField] private string playerPrimaryStartMessage;
	[SerializeField] private string playerSecondaryStartMessage;
	[SerializeField] private string playerStartButtonMessage;

    private GameManager.Team nextTeamUp;
	
	[SerializeField] private Text previousTurnSummaryDescription = null;

	// delegates
	public delegate void OnSpecialActionInitiatedEvent(GameManager.ActionType _action);
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

	[SerializeField] private float timeDelayToShowPoliceMove = 1.0f;

	void Awake()
	{
		GameManager.Instance.OnPhaseStart += OnPhaseStart;
		GameManager.Instance.OnStatUpdated += OnStatUpdated;
		GameManager.Instance.OnGameEnd += OnGameEnd;
		PoliceManager.Instance.OnPoliceMovementComplete += OnPoliceMovementComplete;
    }

	void OnDestroy()
	{
		if (GameManager.IsInstanceIsNotNull())
		{
			GameManager.Instance.OnPhaseStart -= OnPhaseStart;
			GameManager.Instance.OnStatUpdated -= OnStatUpdated;
			GameManager.Instance.OnGameEnd -= OnGameEnd;
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
		GameWonSet.SetActive(false);
	}

	public void TriggerPoliceMovementStart()
	{
		TurnOffAllPrompts();
		PoliceManager.Instance.TakePoliceTurn();
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
		OnSpecialActionInitiated(GameManager.ActionType.ARREST);
	}

	public void ButtonPressBonusAction()
	{
		// trigger event
		OnSpecialActionInitiated(GameManager.ActionType.BONUS_ACTION);
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

	// events
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

	void OnStatUpdated()
	{
		UpdateActionsLeft();
	}

	void OnPoliceMovementComplete()
	{
		Invoke("ShowPlayerStartUI", timeDelayToShowPoliceMove);
	}

	void OnGameEnd(GameManager.Team _teamWon)
	{
		GameWonSet.SetActive(true);
	}

}
