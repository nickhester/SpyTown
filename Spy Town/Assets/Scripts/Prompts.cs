using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	// action icons
	[SerializeField] private GameObject actionIcon_move;
	[SerializeField] private GameObject actionIcon_arrest;
	private List<GameObject> actionIcons = new List<GameObject>();
	[SerializeField] private float actionIconSeparation = 10.0f;

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

	public void ShowTurnSummary(List<ActionRecord> _actions)
	{
		PreviousTurnSummarySet.SetActive(true);

		for (int i = 0; i < _actions.Count; i++)
		{
			GameObject objectToInstantiate = null;
			if (_actions[i].actionType == GameManager.ActionType.MOVE || _actions[i].actionType == GameManager.ActionType.BONUS_ACTION)
			{
				objectToInstantiate = actionIcon_move;
			}
			else if (_actions[i].actionType == GameManager.ActionType.ARREST)
			{
				objectToInstantiate = actionIcon_arrest;
			}

			GameObject go = Instantiate(objectToInstantiate) as GameObject;
			actionIcons.Add(go);

			// position it
			go.transform.SetParent(PreviousTurnSummarySet.transform);
			RectTransform this_rt = go.GetComponent<RectTransform>();
			RectTransform prefab_rt = objectToInstantiate.GetComponent<RectTransform>();
			this_rt.anchoredPosition = new Vector2(prefab_rt.anchoredPosition.x + (actionIconSeparation * i), prefab_rt.anchoredPosition.y);
			this_rt.localRotation = prefab_rt.localRotation;
			this_rt.localScale = prefab_rt.localScale;

			// color it
			Color color = Color.cyan;
			if (_actions[i].buildingTeam == GameManager.Team.PRIMARY)
			{
				color = GameManager.Instance.GetGameOptions().primaryTeamColor;
			}
			else if (_actions[i].buildingTeam == GameManager.Team.SECONDARY)
			{
				color = GameManager.Instance.GetGameOptions().secondaryTeamColor;
			}
			else if (_actions[i].buildingTeam == GameManager.Team.NEUTRAL)
			{
				color = GameManager.Instance.GetGameOptions().neutralColor;
			}

			go.GetComponent<Image>().color = color;
		}
	}

	public void HideTurnSummary()
	{
		for (int i = 0; i < actionIcons.Count; i++)
		{
			Destroy(actionIcons[i]);
		}
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
