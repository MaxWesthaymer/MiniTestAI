using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Text endTxt; 
    [SerializeField] private Button restartBtn; 
    void Start()
    {
        GameController.OnEndGame += ShowEndPanel;
        restartBtn.onClick.AddListener((() =>
        {
            GameController.instance.RestartGame();
        }));
    }

    private void ShowEndPanel(bool isWin)
    {
        endGamePanel.SetActive(true);
        endTxt.text = isWin ? "YOU\nWIN!" : "YOU\nLOOSE!";
    }

    private void OnDestroy()
    {
        GameController.OnEndGame -= ShowEndPanel;
    }
}