using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateQuestionUIManager : MonoBehaviour
{
    [HideInInspector]
    public static CreateQuestionUIManager Instance;

    private float horizontalEditToolOffset = 50.0f;

    [HideInInspector]
    public int questionIndex;
    [HideInInspector]
    public int selectedQuestion;

    [SerializeField]
    private TMP_InputField titleInputField;

    [Header("Edit tools")]
    [SerializeField]
    private Button addRowButton;
    [SerializeField]
    private Button addColumnButton;
    [SerializeField]
    private RectTransform horizontalEditTool;
    [SerializeField]
    private RectTransform verticalEditTool;
    [SerializeField]
    private Button deleteRowButton;
    [SerializeField]
    private Button deleteColumnButton;

    [Header("Question grid")]
    [SerializeField]
    private FlexibleGridLayout questionGrid;
    [SerializeField]
    private RectTransform categoryBar;
    [SerializeField]
    private RectTransform categoryCellPrefab;
    [SerializeField]
    private RectTransform questionCellPrefab;

    private List<RectTransform> categoryList;

    [Header("Edit QnA panel")]
    [SerializeField]
    private RectTransform editQnAPanel;
    [SerializeField]
    private TMP_InputField questionInputField;
    [SerializeField]
    private TMP_InputField answerInputField;
    [SerializeField]
    private Button exitWithoutSaveButton;
    [SerializeField]
    private Button confirmEditButton;
    [SerializeField]
    private TextMeshProUGUI questionInfoText;

    [Header("Save and finish")]
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button finishButton;

    [Header("Notification")]
    [SerializeField]
    private CanvasGroup saveSuccessNotification;
    [SerializeField]
    private CanvasGroup saveFailNotification;

    private bool isSaveSuccess;
    private float notificationMaxTimer = 2.0f;
    private float notificationTimer;

    // Start is called before the first frame update

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

    void Start()
    {
        AssignButtons();
        titleInputField.text = CreateQuestionManager.Instance.GetTitle();
        UpdateQuesionStructureUI();
        CreateQuestionManager.Instance.onQuestionStructureUpdateCallback += UpdateQuesionStructureUI;

    }

    // Update is called once per frame
    void Update()
    {
        DisplayNotification();
    }

    private void AssignButtons()
    {
        addRowButton.onClick.AddListener(OnAddRowButtonClick);
        addColumnButton.onClick.AddListener(OnAddColumnButtonClick);
        deleteRowButton.onClick.AddListener(OnDeleteRowButtonClick);
        deleteColumnButton.onClick.AddListener(OnDeleteColumnButtonClick);
        exitWithoutSaveButton.onClick.AddListener(OnExitWithouSaveButtonClick);
        confirmEditButton.onClick.AddListener(OnConfirmEditButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        finishButton.onClick.AddListener(OnFinishButtonCLick);
    }

    private void UpdateQuesionStructureUI()
    {
        UpdateCategoryListUI();
        UpdateQuestionListUI();
    }

    private void UpdateCategoryListUI()
    {
        CleanCategoryListUI();
        categoryBar.GetComponent<FlexibleGridLayout>().SetSize(1, CreateQuestionManager.Instance.GetNumberOfCategory());
        categoryList = new List<RectTransform>();
        for (int i = 0; i < CreateQuestionManager.Instance.GetNumberOfCategory(); i++)
        {
            RectTransform rect = Instantiate(categoryCellPrefab, categoryBar.transform);
            rect.GetComponent<EditorCategoryCell>().index = i;
            rect.GetComponent<TMP_InputField>().text = CreateQuestionManager.Instance.GetCategoryName(i);
            categoryList.Add(rect);
        }
    }

    private void UpdateQuestionListUI()
    {
        questionGrid.SetSize(CreateQuestionManager.Instance.GetNumberOfQuestionTier(), CreateQuestionManager.Instance.GetNumberOfCategory());
        CleanQuestionGridUI();
        for (int i = 0; i < CreateQuestionManager.Instance.GetNumberOfQuestionTier(); i++)
            for (int j = 0; j < CreateQuestionManager.Instance.GetNumberOfCategory(); j++)
            {
                RectTransform questionCell = Instantiate(questionCellPrefab, questionGrid.transform);
                int index = i * CreateQuestionManager.Instance.GetNumberOfCategory() + j;
                questionCell.GetComponent<EditorQuestionCell>().Initialize(index);
            }
    }

    private void CleanCategoryListUI()
    {
        for (int i = categoryBar.childCount - 1; i >= 0; i--)
        {
            Destroy(categoryBar.GetChild(i).gameObject);
        }
    }

    private void CleanQuestionGridUI()
    {
        for (int i = questionGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(questionGrid.transform.GetChild(i).gameObject);
        }
    }

    private void OnAddRowButtonClick()
    {
        CreateQuestionManager.Instance.AddQuestionTier();
    }

    private void OnAddColumnButtonClick()
    {
        CreateQuestionManager.Instance.AddCategory();
    }

    private void OnDeleteRowButtonClick()
    {
        CreateQuestionManager.Instance.RemoveQuestionTier(questionIndex);
    }

    private void OnDeleteColumnButtonClick()
    {
        CreateQuestionManager.Instance.RemoveCategory(questionIndex);
    }

    private void OnExitWithouSaveButtonClick()
    {
        editQnAPanel.gameObject.SetActive(false);
    }

    private void OnConfirmEditButtonClick()
    {
        CreateQuestionManager.Instance.EditQuestionAndAnswer(selectedQuestion, questionInputField.text, answerInputField.text);
        editQnAPanel.gameObject.SetActive(false);
    }

    private void OnSaveButtonClick()
    {
        if (CreateQuestionManager.Instance.SaveGameData())
        {
            isSaveSuccess = true;
            notificationTimer = notificationMaxTimer;
            saveSuccessNotification.gameObject.SetActive(true);
        }
        else
        {
            isSaveSuccess= false;
            notificationTimer = notificationMaxTimer;
            saveFailNotification.gameObject.SetActive(true);
        }
    }

    public void DisplayEditTool(Vector2 position)
    {
        horizontalEditTool.anchoredPosition = new Vector2 (horizontalEditTool.anchoredPosition.x, position.y - horizontalEditToolOffset);
        verticalEditTool.anchoredPosition = new Vector2 (position.x, verticalEditTool.anchoredPosition.y);
    }

    public void OpenEditPanel(int questionIndex)
    {
        questionInfoText.text = CreateQuestionManager.Instance.GetCategoryName(questionIndex) + " for " + CreateQuestionManager.Instance.CalculatePoint(questionIndex);
        selectedQuestion = questionIndex;
        editQnAPanel.gameObject.SetActive(true);
        questionInputField.text = CreateQuestionManager.Instance.ViewQuestion(questionIndex);
        answerInputField.text = CreateQuestionManager.Instance.ViewAnswer(questionIndex);
    }

    private void OnFinishButtonCLick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void DisplayNotification()
    {
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0)
            {
                if (isSaveSuccess)
                    saveSuccessNotification.gameObject.SetActive(false);
                else   
                    saveFailNotification.gameObject.SetActive(false);
            }
            else
            {

                if (isSaveSuccess)
                    saveSuccessNotification.alpha = Mathf.Lerp(0, 1, notificationTimer / notificationMaxTimer);
                else
                    saveFailNotification.alpha = Mathf.Lerp(0, 1, notificationTimer / notificationMaxTimer);
            }
        }
    }
}
