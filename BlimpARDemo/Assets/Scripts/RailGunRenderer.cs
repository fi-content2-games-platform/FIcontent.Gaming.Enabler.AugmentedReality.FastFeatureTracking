using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class RailGunRenderer : MonoBehaviour
{
    public GameObject explosionPrefab;

    LineRenderer lineRenderer;
    float railSpawnTime = 0f;
    float railTime = .5f;

    Transform target = null;
    Vector3 firePos;
    Vector3 targetPos;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.SetPosition(1, firePos);
        this.lineRenderer.SetPosition(0, targetPos);         
        StartCoroutine(DrawRail());
    }

    IEnumerator DrawRail()
    {
        yield return new WaitForSeconds(railSpawnTime);
              
        this.lineRenderer.enabled = true;        
        Destroy(gameObject, railTime);
    }
  
    public void SetFrom(Vector3 p)
    {
        this.firePos = p;
    }
    public void SetTo(Vector3 p)
    {
        this.targetPos = p;
    }
    public void SetTarget(Transform t)
    {
        target = t;
    }
    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            this.lineRenderer.SetPosition(0, target.position);            
            target.SendMessage("Hit", 2, SendMessageOptions.DontRequireReceiver);
        }
        
        Color c = this.lineRenderer.material.GetColor("_TintColor");
        //c -= new Color(.01f, .01f, .01f);
        c.a -= .01f;
        this.lineRenderer.material.SetColor("_TintColor", c);
        /**/
        this.lineRenderer.SetPosition(1, firePos);

        if (c.a >= 0f && c.a < .1f)
        {
            var g = Instantiate(explosionPrefab, target.position, target.rotation) as GameObject;
            g.transform.parent = Containers.Explosions;
        }

    }
}
