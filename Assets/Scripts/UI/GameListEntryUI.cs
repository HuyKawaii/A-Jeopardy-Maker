using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameListEntryUI : MonoBehaviour
{
    private int index;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetSelectedGameIndex);
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void SetSelectedGameIndex()
    {
        GameSelectionManager.Instance.SelectGame(index);
    }
}
