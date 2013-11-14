using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : Ship
{
    public GameObject laser;
    public Transform target;
    public float turretRange = 400.0f;

    private List<Transform> inCollision = new List<Transform>();
    private bool firing = false;
    private GameObject laserInstance;

    protected override void Awake()
    {
        //health = 300;
        unitAudioSource = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudioSource.AddSource(true);

        if (this.tag == "Resistance")
            attackSound = AudioControl.RebelRailSound;
        else
            attackSound = AudioControl.EmperrorRailSound;

        unitAudioSource.volumeFactor = .5f;
        unitAudioSource.pitch += pitchVariation;
        unitAudioSource.SetVolume();

    }
    // Use this for initialization
    protected override void Start()
    {
    }

    protected override void FixedUpdate()
    {

    }

    protected override void Explode()
    {
        var e = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
        e.transform.parent = Containers.Explosions;
        Destroy(gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {

        CleanColliders();

        if (target)
        {
            Vector3 dist = target.position - transform.position;
            bool angle = Vector3.Dot(transform.forward, dist) > 0;
            bool range = dist.sqrMagnitude < turretRange * turretRange;

            if (!angle || !range)
                target = getNextTarget();
        }
        else
            target = getNextTarget();

        if (!target)
            return;


        if (!firing)
            StartCoroutine(Fire());
        else
        {
            if (laserInstance)
                laserInstance.SendMessage("SetFrom", transform.position, SendMessageOptions.DontRequireReceiver);
        }

    }

    IEnumerator Fire()
    {
        firing = true;

        unitAudioSource.PlayOneShot(attackSound);

        laserInstance = Instantiate(laser, transform.position, transform.rotation) as GameObject;
        laserInstance.transform.parent = this.transform;
        laserInstance.SendMessage("SetFrom", transform.position, SendMessageOptions.DontRequireReceiver);
        laserInstance.SendMessage("SetTarget", target, SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(2);

        firing = false;
    }

    Transform getNextTarget()
    {
        if (inCollision.Count == 0)
            return null;

        int scanned = 0; ;
        while (!target && scanned < inCollision.Count)
        {
            Transform t = inCollision[scanned];
            Vector3 dist = t.position - transform.position;
            bool angle = Vector3.Dot(transform.forward, dist) < 0;
            bool range = dist.sqrMagnitude < turretRange * turretRange;

            if (angle && range)
                return t;

            scanned++;
        }
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(TagsManager.GetEnemyOf(this.tag)))
            if (!inCollision.Contains(other.transform))
                inCollision.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(TagsManager.GetEnemyOf(this.tag)))
            if (inCollision.Contains(other.transform))
                inCollision.Remove(other.transform);
    }

    private void CleanColliders()
    {
        for (int i = inCollision.Count - 1; i >= 0; i--)
            if (!inCollision[i])
                inCollision.RemoveAt(i);
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + center);
        //Gizmos.DrawLine(transform.position, transform.position + follow);
        //Gizmos.DrawLine(transform.position, transform.position + avoid);
        //if(target)
        //Gizmos.DrawLine(transform.position, target.position);
    }
}
