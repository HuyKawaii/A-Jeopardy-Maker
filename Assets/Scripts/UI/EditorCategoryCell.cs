using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorCategoryCell : MonoBehaviour
{
    TMP_InputField inputField;
    public int index;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(OnEditComplete);
    }

    private void OnEditComplete(string input)
    {
        CreateQuestionManager.Instance.EditCategory(index, input);
    }
}
