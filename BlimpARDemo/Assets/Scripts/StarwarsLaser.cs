using UnityEngine;
using System.Collections;

public class StarwarsLaser : MonoBehaviour
{
    public float speed = 400.0f;
    public float lifeTime = 3.0f;

    // Use this for initialization
    void Start()
    {
        speed = 600.0f;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(TagsManager.GetEnemyOf(this.tag)))
        {
           DamageTarget(other.transform); 
        }
    }

    void DamageTarget(Transform target)
    {
        target.SendMessage("Hit", 10, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
