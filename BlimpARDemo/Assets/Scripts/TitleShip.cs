using UnityEngine;
using System.Collections;

public class TitleShip : MonoBehaviour
{
    public Transform direction;
    public float speed = 40f;
    public float duration = 10f;

    private bool exploding = false;

    private Vector3 eulerAngleVelocity = new Vector3(540f, 900f, 200f);

    // Use this for initialization
    void Start()
    {
        this.transform.LookAt(direction);

        Destroy(gameObject, duration);

    }

    // Update is called once per frame
    void Update()
    {
        if (!exploding)
            this.transform.LookAt(direction);
    }

    void FixedUpdate()
    {
        if (exploding)
        {
            //this.rigidbody.AddTorque(new Vector3(100f, 50f, 30f), ForceMode.Acceleration);
            var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        }
        else
        {
            var v = direction.position - transform.position;
            v.Normalize();
            this.rigidbody.velocity = v * speed;
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(this.transform.position, direction.position);
    }

    internal void Explode()
    {
        exploding = true;
    }
}
