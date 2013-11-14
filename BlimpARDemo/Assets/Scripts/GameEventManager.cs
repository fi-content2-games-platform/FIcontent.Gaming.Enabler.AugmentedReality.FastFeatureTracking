using System;
using UnityEngine;

public abstract class GameEventManager : MonoBehaviour
{
    public static bool GameStarted = false;

    public delegate void GameEvent();
    public static event GameEvent GameStartEvent, GameOverEvent;

    protected abstract void Awake();
    protected abstract void Start();
    protected abstract void Update();

    /// <summary>
    /// Raises the GameStart events
    /// </summary>
    [ContextMenu("Start Game")]
    public void StartGame()
    {
        if (GameStartEvent != null)
        {
            GameStartEvent();
            GameStarted = true;
        }
    }

    public abstract void ShowInterface();
    
    /// <summary>
    /// Raises the GameOver events
    /// </summary>
    [ContextMenu("End Game")]
    public void EndGame()
    {
        if (GameOverEvent != null)
        {
            GameOverEvent();
            GameStarted = false;

            ShowInterface();
        }
    }
}

