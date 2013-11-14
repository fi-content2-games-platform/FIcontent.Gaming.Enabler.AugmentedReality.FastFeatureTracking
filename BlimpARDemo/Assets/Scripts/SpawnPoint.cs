using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        this.transform.LookAt(Containers.UniverseCenter, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
