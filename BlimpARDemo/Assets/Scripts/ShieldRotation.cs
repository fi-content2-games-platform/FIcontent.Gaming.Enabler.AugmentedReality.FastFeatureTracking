using UnityEngine;
using System.Collections;

public class ShieldRotation : MonoBehaviour
{

    public float angleSpeed;
            
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {        
        transform.RotateAroundLocal(transform.up, Time.deltaTime * angleSpeed);
    }
}
