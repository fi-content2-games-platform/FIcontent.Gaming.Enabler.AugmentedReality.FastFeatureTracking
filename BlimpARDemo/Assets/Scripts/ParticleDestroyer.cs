using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDestroyer : MonoBehaviour
{
    private ParticleSystem particleSys;

    // Use this for initialization
    void Start()
    {
        this.particleSys = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, this.particleSys.duration);
    }
}
