using UnityEngine;
using System.Collections;

public class AttackBlimp : ScriptedBehaviour
{
    public Transform blimp;
    public BattleAI battleAI;
    public UIPanel firePanel, crossHairPanel;
    public UILabel scoreLabel;
    public GameObject fader;
    public Transform messageContainer;
    public GameObject messageGreen, messageRed, messageOrange;
    public GameObject credits;
    public GameObject introText;

    public GameObject buttonShoot;

    private PlayerRailgun playerRailGun;
    private NoiseAndGrain noise;

    public AttackBlimp()
        : base()
    {

    }

    void Awake()
    {

        if (!messageContainer)
            Debug.LogError("message container missing", this);

        if (!messageGreen || !messageOrange || !messageRed)
            Debug.LogError("message prefabs missing", this);

        if (!introText)
            Debug.LogError("intro text missing", this);

        if (!buttonShoot)
            Debug.LogError("button object missing", this);

        if (!firePanel)
            Debug.LogError("fire button panel missing", this);

        if (!crossHairPanel)
            Debug.LogError("crosshair panel panel missing", this);


        if (!scoreLabel)
            Debug.LogError("score panel missing", this);

        if (battleAI == null)
            Debug.LogError("battle ai reference missing", this);

        if (!blimp)
            Debug.LogError("blimp object missing", this);

        if (!credits)
            Debug.LogError("credits object missing", this);

        playerRailGun = Camera.main.GetComponent<PlayerRailgun>();
    }

    public override IEnumerator Run()
    {
        running = true;

        var intro = Instantiate(introText) as GameObject;

        var fadIn = Instantiate(fader) as GameObject;

#if !UNITY_EDITOR
        fadIn.GetComponent<SceneFadeInOut>().inSpeed = .1f;
        yield return new WaitForSeconds(26f);
#endif
        bool test = false;

        if (!test)
        {

            ShowMessage(messageOrange, 2f, "Unknown objects approaching");

            yield return new WaitForSeconds(4f);

            battleAI.emperror.Stations.SetDefeated(false);

            yield return new WaitForSeconds(5f);


            //attack blimp
            battleAI.emperrorStationTarget = blimp;
            battleAI.EmperrorAttackTarget();

            yield return new WaitForSeconds(6.5f);

            ShowMessage(messageRed, 4f, "Sector under attack", TweenMessage.sALARM);

            yield return new WaitForSeconds(4.5f);

            ShowMessage(messageOrange, 4f, "Deploying Skye unit");


            scoreLabel.enabled = true;
            firePanel.enabled = true;
            crossHairPanel.enabled = true;
            UIEventListener.Get(buttonShoot).onPress += OnShootButtonDown;


            foreach (var s in battleAI.emperror.Stations)
                StartCoroutine(Attack((EmperrorStation)s));

            yield return new WaitForSeconds(4.5f);

            ShowMessage(messageRed, 4f, "Engaging");

            //spawn rebel station
            battleAI.rebels.Stations.SetDefeated(false);

            yield return new WaitForSeconds(10f);


            //spawn fighters
            battleAI.emperror.Fighters.SetDefeated(false);
            battleAI.rebels.Fighters.SetDefeated(false);

            yield return new WaitForSeconds(10f);

            battleAI.emperrorStationTarget = null;
            battleAI.EmperrorAttackTarget();

            yield return new WaitForSeconds(20f);

            //stop spawning emperror stations
            battleAI.emperror.Stations.SetDefeated(true);

            yield return new WaitForSeconds(50f);
            //stop spawning emperror fighters
            battleAI.emperror.Fighters.SetDefeated(true);

            yield return new WaitForSeconds(20f);

            foreach (var s in battleAI.emperror.Stations)
                s.Kill();

            ShowMessage(messageGreen, 4f, "Enemy battleships destroyed", TweenMessage.sVICTORY);

            yield return new WaitForSeconds(7f);

            ShowMessage(messageOrange, 4f, "Terminating transmission");

            firePanel.enabled = false;
            crossHairPanel.enabled = false;
            UIEventListener.Get(buttonShoot).onPress -= OnShootButtonDown;


            Instantiate(fader);
        }

        playerRailGun.EndGame();
        scoreLabel.enabled = false;

        var c = Instantiate(credits) as GameObject;
        Destroy(c, 14.0f);

        yield return new WaitForSeconds(12f);


        ((GameEventManager)GameObject.FindObjectOfType(typeof(GameEventManager))).EndGame();

        running = false;

    }


    IEnumerator Attack(EmperrorStation s)
    {
        while (running)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));


            if (s)
                s.target = blimp;

            yield return new WaitForSeconds(Random.Range(5f, 10f));

            if (s)
                s.target = null;
        }
    }

    private void ShowMessage(GameObject message, float duration, string text, int sound = 0)
    {
        var go = Instantiate(message) as GameObject;
        go.transform.parent = messageContainer;
        go.SendMessage("SetDuration", duration, SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetText", text, SendMessageOptions.DontRequireReceiver);
        if (sound > 0)
            go.SendMessage("PlaySound", sound, SendMessageOptions.DontRequireReceiver);
        go.transform.localScale = new Vector3(1, .001f, 1);
    }

    private void OnShootButtonDown(GameObject go, bool state)
    {
        if (state)
            playerRailGun.buttonPressed = true;
    }

    void Update()
    {
        if (scoreLabel.enabled)
        {
            if (playerRailGun.score > playerRailGun.high_score)
            {
                scoreLabel.text = "Score: " + playerRailGun.score.ToString("000000") + " High Score: " + playerRailGun.score.ToString("000000");
                // dont save score until humanity is saved
            }
            else
            {
                scoreLabel.text = "Score: " + playerRailGun.score.ToString("000000") + " High Score: " + playerRailGun.high_score.ToString("000000");
            }
        }
    }
}