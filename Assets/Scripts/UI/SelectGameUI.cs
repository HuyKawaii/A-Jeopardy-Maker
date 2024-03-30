using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectGameUI : MonoBehaviour
{
    public static SelectGameUI Instance;
    private Color selectedColor = new Color(0.9f, 0.9f, 0.15f, 0.3f);
    private Color normalColor = new Color(0, 0, 0, 0.8f);
    [SerializeField]
    private RectTransform gameList;
    [SerializeField]
    private RectTransform gameListEntryPrefab;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private TextMeshProUGUI selectedGameText;

    private Color normalTextColor = new Color32(231, 228, 38, 255);
    private Color highlightTextColor = new Color32(204, 51, 51, 255);
    private const float flickerInterval = 0.1f;
    private float flickerIntervalCounter;
    private const float flickerDuration = 1.0f;
    private float flickerDurationCounter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        GameSelectionManager.Instance.onGameSelectedCallback += HighLightSelectedGame;
    }

    private void Update()
    {
        HandleChooseGameTextFlicker();
    }

    private void HighLightSelectedGame(int newIndex)
    {
        int oldIndex = GameSelectionManager.Instance.selectedIndex;
        if (oldIndex != -1)
            gameList.GetChild(oldIndex).GetComponent<Image>().color = normalColor;

        gameList.GetChild(newIndex).GetComponent<Image>().color = selectedColor;
    }

    public void DisplayGameList()
    {
        for (int i = gameList.childCount; i > 0; i--)
        {
            Destroy(gameList.GetChild(i).gameObject);
        }

        for (int i = 0; i < GameSelectionManager.Instance.gameDataList.Count; i++) 
        {
            RectTransform gameListEntry = Instantiate(gameListEntryPrefab, gameList);
            gameListEntry.GetComponentInChildren<TextMeshProUGUI>().text = GameSelectionManager.Instance.gameDataList[i].title;
            gameListEntry.GetComponent<GameListEntryUI>().SetIndex(i);
        }
    }

    private void OnConfirmButtonClick()
    {
        GameSelectionManager.Instance.ConfirmSelectedGame();
        selectedGameText.text = GameSelectionManager.Instance.GetSelectedGameTitle();
    }

    public void FlickerChooseGameText()
    {
        flickerDurationCounter = flickerDuration;
    }

    private void HandleChooseGameTextFlicker()
    {
        if (flickerDurationCounter > 0)
        {
            if (flickerIntervalCounter > 0)
            {
                flickerIntervalCounter -= Time.deltaTime;
                
            }
            else
            {
                flickerIntervalCounter = flickerInterval;
                if (selectedGameText.color == normalTextColor)
                    selectedGameText.color = highlightTextColor;
                else
                    selectedGameText.color = normalTextColor;
            }
            flickerDurationCounter -= Time.deltaTime;
        }
        else
        {
            if (selectedGameText.color != normalTextColor)
                selectedGameText.color = normalTextColor;
        }
    }
}
