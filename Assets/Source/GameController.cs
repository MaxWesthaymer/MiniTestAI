using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField]public EntitySettings entitySettings;
    public static event Action<bool> OnEndGame;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void EndOfGame(bool isWin)
    {
        OnEndGame?.Invoke(isWin);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}

