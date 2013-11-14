using System;
using UnityEngine;

public class GameEventManagerStandAlone : GameEventManager
{
    public GameObject buttonStart, buttonScan;
    public GameObject initPanel, startPanel;

    private SimpleColorPicker picker;

    protected override void Awake()
    {
        if (!buttonStart || !buttonScan)
            Debug.LogError("button objects missing", this);

        if (!initPanel || !startPanel)
            Debug.LogError("panel objects missing", this);
    }

    protected override void Start()
    {
        initPanel.SetActive(true);
        picker = GetComponent<SimpleColorPicker>();
        UIEventListener.Get(buttonScan).onClick += OnScanButtonClicked;

        GameStartEvent += GameEventManagerStandAlone_GameStartEvent;
        GameOverEvent += GameEventManagerStandAlone_GameOverEvent;        
    }

    protected override void Update()
    {

    }


    public override void ShowInterface()
    {
        
    }

    void GameEventManagerStandAlone_GameOverEvent()
    {
        startPanel.SetActive(true);
    }

    void GameEventManagerStandAlone_GameStartEvent()
    {
        startPanel.SetActive(false);
    }

    /// <summary>
    /// Starts the game when clicking on the button
    /// </summary>
    /// <param name="sender"></param>
    private void OnStartButtonClicked(GameObject sender)
    {
        StartGame();
    }

    private void OnScanButtonClicked(GameObject sender)
    {
        initPanel.SetActive(false);
        picker.updateColor = false;

        startPanel.SetActive(true);

        Camera.main.SendMessage("DisableDebug", true, SendMessageOptions.DontRequireReceiver);

        UIEventListener.Get(buttonScan).onClick -= OnScanButtonClicked;
        UIEventListener.Get(buttonStart).onClick += OnStartButtonClicked;
    }

}

