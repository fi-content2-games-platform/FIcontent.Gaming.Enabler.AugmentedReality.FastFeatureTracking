using UnityEngine;

public class StationCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Asteroid>())
        {
            other.gameObject.SendMessage("Explode", true, SendMessageOptions.DontRequireReceiver);
        }
    }
}

