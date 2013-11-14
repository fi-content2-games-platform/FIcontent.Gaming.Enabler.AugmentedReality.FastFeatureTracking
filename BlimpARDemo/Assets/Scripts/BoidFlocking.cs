using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidFlocking : Ship
{
    // Editor
    public GameObject laser;
    public Transform subTarget;
    public Transform[] laserGuns;
    public float trailTime = .07f;

    // Logic 
    internal BoidController controller;
    private bool firing = false;
    private bool flocking = false;
    private bool collidingWithStation = false;
    private Vector3 distanceToTarget;
    private List<Transform> inCollision = new List<Transform>();
    private Transform stationInCollision;
    private Vector3 collisionDirection;

    // Visualization
    private Vector3 center;
    private Vector3 velocity;
    private Vector3 follow;
    private Vector3 avoid;
    private Vector3 evasion;
    private Vector3 steer = Vector3.zero;

    TrailRenderer trail;
    public int MAX_LASER_SHOTS = 3;



    protected override void Awake()
    {
        health = 50.0f;

        unitAudioSource = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudioSource.AddSource();
        unitAudioSource.volumeFactor = .15f;
        unitAudioSource.maxDistance = 500;
        unitAudioSource.pitch += pitchVariation;
        unitAudioSource.SetVolume();
    }

    protected override void Start()
    {
        //while (true)
        //{

        //    float waitTime = Random.Range(0.3f, 0.5f);
        //    yield return new WaitForSeconds(waitTime);
        //}

    }

    Transform GetTarget()
    {
        if (subTarget)
            return subTarget;
        else
            return controller.target;
    }

    protected override void Explode()
    {
        var e = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
        e.transform.parent = Containers.Explosions;
        Destroy(gameObject);
    }

    protected override void FixedUpdate()
    {
        if (!controller)
            return;

        CleanColliders();

        if (!subTarget)
            subTarget = controller.GetSubTarget();

        if (GetTarget())
        {
            distanceToTarget = GetTarget().position - transform.position;
            FireAI();
        }

        if (!flocking)
            StartCoroutine(FlockAI());

        trail = GetComponentInChildren<TrailRenderer>();
        if (trail)
        {
            trail.time = (rigidbody.velocity.magnitude / 200) * trailTime;
        }
        //else
        //    Debug.Log("asd");
    }

    protected override void Update()
    {

    }

    private void CleanColliders()
    {
        for (int i = inCollision.Count - 1; i >= 0; i--)
            if (!inCollision[i])
                inCollision.RemoveAt(i);
    }

    IEnumerator FlockAI()
    {
        flocking = true;
        if (controller)
        {
            if (collidingWithStation && stationInCollision)
            {
                float size = stationInCollision.collider.bounds.extents.x * 1.5f;
                collisionDirection = stationInCollision.position - transform.position;
                if (collisionDirection.sqrMagnitude > size * size)
                {
                    collidingWithStation = false;
                    stationInCollision = null;
                    evasion = Vector3.zero;
                }
            }

            // Compute desired steering, but apply only according to max rotation
            rigidbody.velocity += Steer() * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rigidbody.velocity), controller.maxRotation * Time.deltaTime);

            // Individuals velocity can only in the direction they are orientated
            float speed = rigidbody.velocity.magnitude;
            rigidbody.velocity = transform.rotation * (Vector3.forward * speed);

            ControlSpeed(controller.maxVelocity, controller.minVelocity);
        }
        flocking = false;

        yield break;
    }

    void StationCollision(Transform station)
    {
        stationInCollision = station;
        collidingWithStation = true;
    }

    Vector3 Steer()
    {
        Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);
        randomize.Normalize();
        randomize *= controller.randomness;

        center = controller.flockCenter - transform.localPosition;
        velocity = controller.flockVelocity - rigidbody.velocity;
        follow = Vector3.zero;

        if (GetTarget())
        {
            Vector3 tail = GetTarget().position - GetTarget().forward * controller.attackDistance;
            follow = tail - transform.position;
        }

        avoid = Vector3.zero;

        if (inCollision.Count > 0 /*&& !controller.flyInFormation*/)
        {
            foreach (Transform t in inCollision)
            {
                if (!t) continue;

                Vector3 direction = transform.position - t.position;
                float dist = direction.sqrMagnitude - t.collider.bounds.extents.x;

                direction.Normalize();
                avoid += direction / dist;
            }
            avoid /= inCollision.Count;

            if (Vector3.Dot(avoid, transform.forward) < 0)
                avoid = Vector3.zero;

        }

        //follow.Normalize();
        //center.Normalize();
        //avoid.Normalize();
        Vector3 uniCenter = Vector3.zero;
        //if (controller.universeCenter == null)
        //    uniCenter = controller.target ? controller.target.position : Containers.UniverseCenter.transform.position;
        //else

        if (controller.universeCenter != null)
            uniCenter = (Vector3)controller.universeCenter - transform.position; //Containers.UniverseCenter.transform.position - transform.position;            

        uniCenter /= 2;

        if (controller.flyInFormation)
            steer = center + velocity + follow + randomize + 1000 * avoid + uniCenter;
        else
            steer = velocity + follow + randomize + 1000 * avoid + uniCenter;

        if (collidingWithStation)
        {
            evasion = Vector3.Cross(collisionDirection, steer).normalized;
            steer = evasion * steer.magnitude;
        }


        return steer;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!inCollision.Contains(other.transform))
            inCollision.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (inCollision.Contains(other.transform))
            inCollision.Remove(other.transform);
    }

    void FireAI()
    {
        if (distanceToTarget.sqrMagnitude < controller.attackDistance * controller.attackDistance)
        {
            float dot = Vector3.Dot(distanceToTarget.normalized, transform.forward);
            if (!firing && dot > 0.85)
            {
                StartCoroutine(Fire());
            }
        }

    }

    IEnumerator Fire()
    {
        firing = true;

        unitAudioSource.PlayOneShot(attackSound);

        for (int n = 0; n < MAX_LASER_SHOTS; n++)
        {
            for (int i = 0; i < laserGuns.Length; i++)
            {
                var go = Instantiate(laser, laserGuns[i].position, Quaternion.LookRotation(distanceToTarget)) as GameObject;
                go.transform.parent = Containers.Lasers;
                if (GetTarget()) GetTarget().SendMessage("Hit", controller.attackDamage, SendMessageOptions.DontRequireReceiver);
                yield return new WaitForSeconds(0.1f);
            } //for lasers
        } //for n shots 
              
        firing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + evasion * 100);
    }

    void OnDrawGizmosSelected()
    {
        if (subTarget)
        {
            Gizmos.color = new Color(1f, 0f, 0f, .2f);
            Gizmos.DrawLine(transform.position, subTarget.position);
        }

        if (controller.universeCenter != null)
        {
            Gizmos.color = new Color(0f, 0f, 1f, .2f);
            Gizmos.DrawLine(transform.position, (Vector3)controller.universeCenter);
        }
    }

}