using Cynteract.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public LevelSelectButton levelSelectButton;
    [ReadOnly]
    public LevelSelectButton[] buttons;
    public InstantiatedMenu instantiatedMenu;


    public Button leftArrow, rightArrow;
    public TextMeshProUGUI pageText;
    [MinValue(1)]
    public int entriesPerPage;
    [ReadOnly]
    [ShowInInspector]
    public int numberOfButtons
    {
        get; private set;
    }
    [ReadOnly]
    [ShowInInspector]
    public int numberOfPages
    {
        get; private set;
    }
    [ReadOnly]
    [ShowInInspector]
    public int currentPage
    {
        get; private set;
    }
    public void SelectLevel(int level)
    {
        LevelLoading.LoadLevel(level);
        instantiatedMenu.Close();
    }
    private void OnEnable()
    {
        numberOfButtons = LevelLoading.GetNumberOfLevels();
        numberOfPages=(int)Math.Ceiling(((double) numberOfButtons)/entriesPerPage);
        DrawButtons();
    }
    public void DrawButtons()
    {
        if (buttons!=null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i]!=null)
                {

                    Destroy(buttons[i].gameObject);
                }
            }
        }

        buttons = new LevelSelectButton[entriesPerPage];
        DrawCurrentPage();
    }
    private void DrawCurrentPage()
    {
        DrawPage(currentPage);
    
    }
    public void PreviousPage()
    {
        currentPage = (currentPage - 1) % numberOfPages;
        DrawButtons();
    }
    public void NextPage()
    {
        currentPage = (currentPage + 1) % numberOfPages;
        DrawButtons();

    }

    private void DrawPage(int index)
    {
        int startIndex = entriesPerPage * (currentPage);
        for (int i = 0; i < entriesPerPage&&i+startIndex<numberOfButtons; i++)
        {
            buttons[i] = Instantiate(levelSelectButton, transform);
            buttons[i].text.text = (i + 1+startIndex).ToString();
            buttons[i].owner = this;
            buttons[i].index = i+startIndex;
        }
        pageText.text = (currentPage+1) + "/" + numberOfPages;
    }
}
