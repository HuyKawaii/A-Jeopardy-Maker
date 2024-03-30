using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorQuestionCell : MonoBehaviour, IPointerEnterHandler
{
    public int index
    { private set; get; }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => CreateQuestionUIManager.Instance.OpenEditPanel(index));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CreateQuestionUIManager.Instance.questionIndex = index;
        RectTransform rectTransform = GetComponent<RectTransform>();
        CreateQuestionUIManager.Instance.DisplayEditTool(rectTransform.anchoredPosition);
    }

    public void Initialize(int index)
    {
        this.index = index;
        GetComponentInChildren<TextMeshProUGUI>().text = "" + CreateQuestionManager.Instance.CalculatePoint(index);
    }
}
