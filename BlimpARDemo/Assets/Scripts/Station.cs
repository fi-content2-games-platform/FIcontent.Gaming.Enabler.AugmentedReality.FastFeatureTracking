using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Station : Ship
{
    public GameObject preExplosionPrefab;
    public float shrinkDuration;
    public float explosionStartingPercentage;

    private Vector3 startScale;
    private float shrinkStartTime;
    private bool shrinking = false;
    private bool exploding = false;

    public float shieldColorDecay;
    public float shieldColorBoost;

    public bool fixedRotation = false;

    private Vector3 evasion = Vector3.zero;
    private Vector3 lastSteer = Vector3.zero;


    private List<Transform> inCollision = new List<Transform>();
    private Transform stationInCollision;
    public bool collidingWithStation;

    public float maxVelocity = 100;
    public float minVelocity = 50;
    public float movementMaxRotation = 90;
    public float movementRandomness = 2;

    protected bool ignoreOtherStations = false;

    public Transform approachTarget;
    public Path path;

    [HideInInspector]
    public int currentPathTargetIndex;
    protected Vector3 currentPathTarget
    {
        get
        {
            if (approachTarget)
                return approachTarget.position;
            else
                return path.path[currentPathTargetIndex % path.path.Length];
        }
    }
    protected float distanceToTarget
    {
        get { return Vector3.Distance(transform.position, currentPathTarget); }
    }

    protected override void Awake()
    {
        if (!preExplosionPrefab || !explosionPrefab)
            Debug.LogError("Please define the explosions in the inspector!", this);

        unitAudioSource = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudioSource.AddSource(true);
        unitAudioSource.volumeFactor = .5f;
        unitAudioSource.pitch += pitchVariation;
    }

    // Use this for initialization
    protected override void Start()
    {
        //init path        
        if (!this.path)
            Debug.LogError("Please assign a path in the inspector!", this);
    }


    protected override void FixedUpdate()
    {
        FollowPath();
    }

    void FollowPath()
    {
        //next point                
        if (distanceToTarget < path.radius)
            currentPathTargetIndex++;

        Vector3 steer = currentPathTarget - this.transform.position;
        steer.Normalize();
        lastSteer = steer;

        if (collidingWithStation && stationInCollision && !ignoreOtherStations)
        {
            var collisionDirection = stationInCollision.position - transform.position;
            evasion = -collisionDirection.normalized;
            steer = evasion * 10f;
        }

        this.rigidbody.velocity += steer * Time.deltaTime;

        ControlSpeed(maxVelocity, minVelocity);
    }

    // Update is called once per frame
    protected override void Update()
    {
        foreach (Transform t in inCollision)
        {
            if (t) t.SendMessage("StationCollision", this.transform, SendMessageOptions.DontRequireReceiver);
        }

        if (shrinking)
            Shrink();

        if (fixedRotation)
            transform.RotateAround(transform.up, movementMaxRotation * Time.deltaTime);
        else
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rigidbody.velocity), Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigidbody.velocity), Time.deltaTime);
    }

    protected override void Hit(float damage)
    {
        base.Hit(damage);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!inCollision.Contains(other.transform))
        {
            inCollision.Add(other.transform);
        }        
    }

    void StationCollision(Transform station)
    {
        stationInCollision = station;
        collidingWithStation = station;
    }

    void StationCollisionStop()
    {
        stationInCollision = null;
        collidingWithStation = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (inCollision.Contains(other.transform))
        {
            inCollision.Remove(other.transform);
            StationCollisionStop();
            other.SendMessage("StationCollisionStop", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Shrink()
    {
        float currentTime = Time.time - shrinkStartTime;
        float t = currentTime * Mathf.PI / (2.0f * shrinkDuration);

#if UNITY_IPHONE
		Handheld.Vibrate();
#endif

        try
        {
            transform.localScale = startScale * Mathf.Pow(Mathf.Cos(t), 5.0f);
        }
        catch (System.Exception ex)
        {
            //TODO: manage 
            // Actor::updateMassFromShapes: Can't compute mass from shapes: must have at least one non-trigger shape!
            // UnityEngine.Transform:set_localScale(Vector3)
            Debug.Log(ex);
        }

        if (currentTime >= shrinkDuration * (1 - explosionStartingPercentage) && !exploding)
        {
            var e = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
            e.transform.parent = Containers.Explosions;
            Destroy(gameObject, shrinkDuration * explosionStartingPercentage);
            exploding = true;
        }
    }

    IEnumerator StartShrinking()
    {
        // Give the pre-explosion enough time to show
        yield return new WaitForSeconds(0.5f);

        // Start Shrinking
        shrinking = true;
        shrinkStartTime = Time.time;
        startScale = transform.localScale;
    }

    protected override void Explode()
    {
        // Create pre-explosion
        var e = Instantiate(preExplosionPrefab, transform.position, transform.rotation) as GameObject;
        e.transform.parent = Containers.Explosions;

        // Start Shrinking
        StartCoroutine(StartShrinking());
    }

    public void SetPath(Transform pathPfb)
    {
        this.path = pathPfb.GetComponent<Path>();
    }


    void OnDrawGizmosSelected()
    {
        if (path)
        {
            Gizmos.DrawLine(transform.position, currentPathTarget);
        }
    }

    void OnDrawGizmos()
    {
        if (collidingWithStation)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + evasion * 1000.0f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + lastSteer * 1000.0f);
        }
    }
}
