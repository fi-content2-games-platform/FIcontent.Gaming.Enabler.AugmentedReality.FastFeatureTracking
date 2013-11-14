using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class BoidController : MonoBehaviour
{
    public bool flyInFormation = true;
    public float minVelocity = 5;
    public float maxVelocity = 20;
    public float maxRotation = 10;
    public float randomness = 1;
    public int flockSize = 20;
    public float attackDamage = 7.5f;
    public float attackDistance = 200.0f;
    public BoidFlocking prefab;
    public Transform target;

    public Vector3? universeCenter;                // used to specify a center the squad will be attracted to
        
    internal Vector3 flockCenter;
    internal Vector3 flockVelocity;

    private List<Transform> subTargets = new List<Transform>();
    List<BoidFlocking> boids = new List<BoidFlocking>();
    
    private float nextChangeTargetTime;

    /// <summary>
    /// True if the squad must change target
    /// </summary>
    public bool NewTarget { get { return !target || Time.time > nextChangeTargetTime; } }
    
    /// <summary>
    /// Gets the current target
    /// Sets the target for all boids
    /// </summary>
    public Transform Target
    {
        get
        {
            return target;
        }

        set
        {
            if (target == null)
            {
                foreach (BoidFlocking b in boids)
                    b.subTarget = null;
            }
            SearchSubTargets();
            if (subTargets.Count > 1)
            {
                flyInFormation = false;
                AssignSubtargets();
            }
            else
            {
                flyInFormation = true;
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < flockSize; i++)
        {
            BoidFlocking boid = Instantiate(prefab, transform.position, transform.rotation) as BoidFlocking;
            boid.transform.parent = transform;
            boid.transform.localPosition = new Vector3(
                            Random.value * collider.bounds.size.x,
                            Random.value * collider.bounds.size.y,
                            Random.value * collider.bounds.size.z) - collider.bounds.extents;
            boid.transform.localPosition.Normalize();
            boid.controller = this;
            boids.Add(boid);
        }

        collider.enabled = false;
    }

    void Update()
    {
        for (int i = this.boids.Count - 1; i >= 0; i--)
            if (!this.boids[i])
                this.boids.RemoveAt(i);


        if (boids.Count == 1)
        {
            boids[0].SendMessage("Hit", 999, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject, 1.0f);
        }

        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        foreach (BoidFlocking boid in boids)
        {
            if (!boid)
                continue;
            center += boid.transform.localPosition;
            velocity += boid.rigidbody.velocity;
        }
        flockCenter = center / flockSize;
        flockVelocity = velocity / flockSize;
    }

    private void SearchSubTargets()
    {
        subTargets.Clear();
        if (target) foreach (Transform t in target.transform)
            {
                BoidFlocking b = t.gameObject.GetComponent<BoidFlocking>();
                if (b) subTargets.Add(b.transform);
            }

        //flyInFormation = subTargets.Count > 3;
    }

    private void AssignSubtargets()
    {
        if (subTargets.Count == 0)
            return;


        foreach (BoidFlocking b in boids)
        {
            b.subTarget = subTargets[Random.Range(0, subTargets.Count - 1)];
        }
    }

    public Transform GetSubTarget()
    {
        SearchSubTargets();
        if (subTargets.Count > 0)
            return subTargets[Random.Range(0, subTargets.Count - 1)];
        else
            return null;
    }

    

    void OnDrawGizmos()
    {
        if (target)
            Gizmos.DrawSphere(target.position, 1);

    }
   
    /// <summary>
    /// Sets a target for the controller and sets the next change target time
    /// The first time the delay is random  [0; delay]
    /// </summary>
    /// <param name="transform">the target</param>
    /// <param name="targetChangeDelay">next change target delay</param>
    internal void SetTarget(Transform transform, float targetChangeDelay = 0)
    {
        //targets change every n seconds to avoid loops
        if (nextChangeTargetTime == 0)        
            this.nextChangeTargetTime = Random.Range(0, targetChangeDelay);
        else
            nextChangeTargetTime = Time.time + targetChangeDelay;

        this.target = transform;
    }

  
}