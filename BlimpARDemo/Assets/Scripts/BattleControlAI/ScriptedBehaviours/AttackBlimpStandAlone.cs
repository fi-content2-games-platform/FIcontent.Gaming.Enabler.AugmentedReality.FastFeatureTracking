using UnityEngine;
using System.Collections;

public class AttackBlimpStandAlone : ScriptedBehaviour
{
    public Transform blimp;
    public BattleAI battleAI;
    public Transform farFarAwayPanel;
    public GameObject farFarAwayLabel;
    public GameObject fader;
    public Transform messageContainer;
    public GameObject messageGreen, messageRed, messageOrange;
    public GameObject credits;
    public GameObject introText;
    public GameObject titleShips;
    public AudioClip introMusicClip, outroMusicClip, backgroundNoiseClip;

    private UnitAudioSource unitAudio;
    private NoiseAndGrain noise;


    public AttackBlimpStandAlone()
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


        if (!farFarAwayPanel || !farFarAwayLabel)
            Debug.LogError("far far away panel or label missing", this);

        noise = Camera.main.GetComponent<NoiseAndGrain>();
        if (!noise)
            Debug.LogError("noise effect not found on main camera", this);

        if (battleAI == null)
            Debug.LogError("battle ai reference missing", this);

        if (!blimp)
            Debug.LogError("blimp object missing", this);

        if (!credits)
            Debug.LogError("credits object missing", this);

        if (!titleShips)
            Debug.LogError("title ship animation object missing", this);
    }

    public override IEnumerator Run()
    {
        running = true;
        var t = new TrackTime();
        unitAudio = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudio.AddSource(true);

        //goto SkipBattle;

        var intro = Instantiate(introText) as GameObject;

        var fadIn = Instantiate(fader) as GameObject;

        yield return new WaitForSeconds(t.Add(4));

        unitAudio.PlayOneShot(introMusicClip);

        //#if !UNITY_EDITOR
        var fadInObj = fadIn.GetComponent<SceneFadeInOut>();
        fadInObj.inSpeed = .7f;
        fadInObj.delay = 26f;

        yield return new WaitForSeconds(t.Add(22));
        //#endif



        var ffaLab = Instantiate(farFarAwayLabel) as GameObject;
        ffaLab.transform.parent = farFarAwayPanel;
        ffaLab.transform.position = Vector3.zero;
        Destroy(ffaLab, 15f);

        var ts = Instantiate(titleShips) as GameObject;
        Destroy(ts, 20f);

        unitAudio.loop = true;
        unitAudio.clip = backgroundNoiseClip;
        unitAudio.Play();

        yield return new WaitForSeconds(t.Add(8));

        //StartCoroutine(Noise());
        //goto SkipBattle;

        ShowMessage(messageOrange, 2f, "Unknown objects approaching");


        yield return new WaitForSeconds(t.Add(4));



        battleAI.emperror.Stations.SetDefeated(false);

        yield return new WaitForSeconds(t.Add(5));


        //attack blimp
        battleAI.emperrorStationTarget = blimp;
        battleAI.EmperrorAttackTarget();

        yield return new WaitForSeconds(t.Add(6.5f));

        ShowMessage(messageRed, 4f, "Sector under attack", TweenMessage.sALARM);

        yield return new WaitForSeconds(t.Add(4.5f));

        ShowMessage(messageOrange, 4f, "Deploying Skye unit");


        foreach (var s in battleAI.emperror.Stations)
            StartCoroutine(Attack((EmperrorStation)s));

        yield return new WaitForSeconds(t.Add(4.5f));

        ShowMessage(messageRed, 4f, "Engaging");

        //spawn rebel station
        battleAI.rebels.Stations.SetDefeated(false);

        yield return new WaitForSeconds(t.Add(10));

    SkipIntro:
        //spawn fighters
        battleAI.emperror.Fighters.SetDefeated(false);
        battleAI.rebels.Fighters.SetDefeated(false);

        yield return new WaitForSeconds(t.Add(10));

        battleAI.emperrorStationTarget = null;
        battleAI.EmperrorAttackTarget();

        yield return new WaitForSeconds(t.Add(20));

        //stop spawning emperror stations
        battleAI.emperror.Stations.SetDefeated(true);
        battleAI.emperror.Fighters.SetDefeated(true);

        yield return new WaitForSeconds(t.Add(50));

        //foreach (var s in battleAI.rebels.Fighters) s.attackDistance = 1000;
                
        yield return new WaitForSeconds(t.Add(20));

        foreach (var s in battleAI.emperror.Stations)
            s.Kill();

        yield return new WaitForSeconds(t.Add(6));

        ShowMessage(messageGreen, 4f, "Enemy battleships destroyed", TweenMessage.sVICTORY);

        yield return new WaitForSeconds(t.Add(10));

    SkipBattle:
        ShowMessage(messageOrange, 4f, "Terminating transmission");


        var fadOutObj = Instantiate(fader) as GameObject;
        fadOutObj.GetComponent<SceneFadeInOut>().delay = 60;

        var c = Instantiate(credits) as GameObject;
        Destroy(c, 40.0f);
        unitAudio.Stop();
        unitAudio.PlayOneShot(outroMusicClip);

        yield return new WaitForSeconds(t.Add(45));

        ((GameEventManagerStandAlone)GameObject.FindObjectOfType(typeof(GameEventManagerStandAlone))).EndGame();

        running = false;

    }

    #region ADD NOISE

    IEnumerator Noise()
    {
        while (running)
        {
            StartCoroutine(ApplyNoise());
            yield return new WaitForSeconds(Random.Range(10f, 30f));
        }
    }

    IEnumerator ApplyNoise()
    {
        var n = noise.intensityMultiplier;

        for (int i = 0; i < Random.Range(10, 30); i++)
        {
            noise.intensityMultiplier = Mathf.PingPong(i, 5f);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < Random.Range(20, 40); i++)
        {
            noise.intensityMultiplier = Mathf.PingPong(i, 10f);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < Random.Range(10, 30); i++)
        {
            noise.intensityMultiplier = Mathf.PingPong(i, 5f);
            yield return new WaitForEndOfFrame();
        }
        noise.intensityMultiplier = n;
    }

    #endregion

    IEnumerator Attack(EmperrorStation s)
    {
        while (running)
        {
            yield return new WaitForSeconds(Random.Range(10f, 15f));


            if (s)
                s.target = blimp;

            yield return new WaitForSeconds(Random.Range(5f, 10f));

            if (s)
                s.target = null;
        }
    }

    private void ShowMessage(GameObject message, float duration, string text, int sound = 0)
    {
        StartCoroutine(ApplyNoise());
        var go = Instantiate(message) as GameObject;
        go.transform.parent = messageContainer;
        go.SendMessage("SetDuration", duration, SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetText", text, SendMessageOptions.DontRequireReceiver);
        if (sound > 0)
            go.SendMessage("PlaySound", sound, SendMessageOptions.DontRequireReceiver);
        go.transform.localScale = new Vector3(1, .001f, 1);
    }

    class TrackTime
    {
        public float totalTime = 0;

        public float Add(int t)
        {
            return Add((float)t);
        }
        public float Add(float t)
        {
            totalTime += t;
            return t;
        }
    }
}