using System;
using UnityEngine;

class GameEventManagerIOS : GameEventManager
{
    public GameObject buttonStart;

    public UIPanel initPanel;
    public UILabel fireText;

    public Color colorOff;
    public Color colorOn;

    private PlayerRailgun playerRailGun;
    private SimpleColorPicker picker;
        
    protected override void Awake()
    {
        if (!buttonStart)
            Debug.LogError("button object missing", this);

        if (!initPanel)
            Debug.LogError("init panel object missing", this);

        if (!fireText)
            Debug.LogError("fire button label object missing", this);
    }

    protected override void Start()
    {
        Invoke("ShowInterface", 7f);

        playerRailGun = Camera.main.GetComponent<PlayerRailgun>();
        picker = GetComponent<SimpleColorPicker>();

        UIEventListener.Get(buttonStart).onClick += OnStartButtonClicked;

        GameStartEvent += GameEventManagerStandAlone_GameStartEvent;
        GameOverEvent += GameEventManagerStandAlone_GameOverEvent;

    }

    void GameEventManagerStandAlone_GameOverEvent()
    {
        picker.enabled = true;
        picker.updateColor = true;
    }

    void GameEventManagerStandAlone_GameStartEvent()
    {
        initPanel.enabled = false;
        picker.updateColor = false;
    }


   protected override void Update()
    {
        SetFire(!playerRailGun.firing);
    }


    public override void ShowInterface()
    {
        initPanel.enabled = true;
        picker.enabled = true;
    }

    /// <summary>
    /// Starts the game when clicking on the button
    /// </summary>
    /// <param name="sender"></param>
    private void OnStartButtonClicked(GameObject sender)
    {
        StartGame();
    }


    private void SetFire(bool on)
    {
        if (on)
        {
            fireText.color = colorOn;
        }
        else
        {
            fireText.color = colorOff;
        }
    }

}

