using UnityEngine;
using System.Collections;

public class EmperrorStation : Station
{
    public GameObject stationShieldBlast;
    public GameObject laser;
    public GameObject implosionPrefab;

    public AudioClip approachSound;

    public Transform target;

    public float laserAttackDuration = 3f;

    private bool rotating = false;
    private bool firing = false;
    private GameObject laserInstance;
    private float shieldCharge = 0.0f;
    private Material shieldMat;

    private float initMaxVelocity;
    private float initMinVelocity;
    private bool heatwaved = false;
    private float lastDA = float.MaxValue;


    private static int InstanceCounter = 0;

    public Transform[] gunMuzzles;


    protected override void Awake()
    {
        ignoreOtherStations = true;

        GameObject approachPoints = GameObject.Find("ApproachPoints");
        approachTarget = approachPoints.transform.GetChild(InstanceCounter % approachPoints.transform.childCount);

        InstanceCounter++;


        initMaxVelocity = maxVelocity;
        initMinVelocity = minVelocity;

        base.Awake();

        try { shieldMat = transform.Find("Shield").renderer.material; }
        catch
        {
            Debug.Log("please assign Shield child material", this);
        }

    }

    protected override void Start()
    {
        base.Start();

        //approachTarget = currentPathTarget;
        //approachTarget.y += InstanceCounter % 2 == 0 ? 0 : 100f;
        //approachTarget.z += InstanceCounter % 2 == 0 ? 0 : 100f;

    }
    
    protected override void Update()
    {
        shieldCharge = Mathf.Max(0, shieldCharge - shieldColorDecay);

        shieldMat.SetColor("_TintColor", Color.Lerp(Color.black, Color.red, shieldCharge));

        if (target)
        {
            maxVelocity = minVelocity = 10;
            var rot = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            var dot = Quaternion.Dot(transform.rotation, rot);
            bool targetAligned = dot > .99f || dot < -.99f;


            if (targetAligned)
            {
                if (!firing)
                {
                    StartCoroutine(Fire());
                }
            } // if target aligned
        } // if target
        else
        {
            base.Update();
        }
    }

    protected override void FixedUpdate()
    {

        if (currentPathTargetIndex == 0)
            Approach();
        else
        {
            ignoreOtherStations = false;
            approachTarget = null;
        }
        base.FixedUpdate();
    }

    protected override void Hit(float damage)
    {
        shieldCharge = Mathf.Min(0.75f, shieldCharge + shieldColorBoost);

        base.Hit(damage);
    }

    IEnumerator Fire()
    {
        firing = true;

        unitAudioSource.PlayOneShot(attackSound);

        var tmpTrgt = new GameObject("tmpTarget");
        tmpTrgt.transform.parent = this.transform;
        tmpTrgt.transform.position = target.position;
        Vector3 rand = Random.onUnitSphere * 50f;
        tmpTrgt.transform.position += rand;


        Ray ray = new Ray(transform.position, tmpTrgt.transform.position);
        var hitInfos = Physics.RaycastAll(ray);
        bool freeToShoot = true;
        foreach (var h in hitInfos)
        {            
            if (h.transform == this.transform) 
                continue;
            
            if (h.transform && h.transform.tag != "Emperror")
            {
                freeToShoot = false;
                break;
            }
        }

        if (freeToShoot)
        {
            foreach (var t in gunMuzzles)
            {
                if (!t) continue;

                laserInstance = Instantiate(laser, t.position, t.rotation) as GameObject;
                laserInstance.transform.parent = this.transform;
                laserInstance.SendMessage("SetFrom", t.position, SendMessageOptions.DontRequireReceiver);
                //g.SendMessage("SetTo", target.position, SendMessageOptions.DontRequireReceiver);
                laserInstance.SendMessage("SetTarget", tmpTrgt.transform, SendMessageOptions.DontRequireReceiver);
            }

            target.SendMessage("Explode", tmpTrgt.transform, SendMessageOptions.DontRequireReceiver);
        }
        
        yield return new WaitForSeconds(laserAttackDuration);

        firing = false;
        maxVelocity = initMaxVelocity;
        minVelocity = initMinVelocity;
    }

    private void Approach()
    {
        float dA = Vector3.Distance(approachTarget.position, transform.position);
        float d = dA - path.radius;

        float v = Mathf.Min(900f, Mathf.Pow(3, d) + initMaxVelocity);

        maxVelocity = v;
        minVelocity = v;

        bool approaching = lastDA > dA;

        if (dA < path.radius * 3)
        {
            if (!heatwaved)
            {
                heatwaved = true;
                var e = Instantiate(implosionPrefab, transform.position, transform.rotation) as GameObject;
                e.transform.parent = Containers.Explosions;
                unitAudioSource.PlayOneShot(approachSound);
            }

            maxVelocity = initMaxVelocity;
            minVelocity = initMinVelocity;
        }

        if (!approaching)
        {
            maxVelocity = initMaxVelocity;
            minVelocity = initMinVelocity;
            currentPathTargetIndex++;
        }

        lastDA = dA;

    }
}