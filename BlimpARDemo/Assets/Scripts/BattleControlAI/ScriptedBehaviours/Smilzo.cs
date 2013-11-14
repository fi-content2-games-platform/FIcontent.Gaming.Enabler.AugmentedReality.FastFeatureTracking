using UnityEngine;
using System.Collections;


public class Smilzo : ScriptedBehaviour
{
    public GameObject smilzoPrefab;
    public AudioClip music;

    private Vector3 eulerAngleVelocity = new Vector3(50f, 90f, 20f);

    public Smilzo()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;

        var audio = this.gameObject.AddComponent<UnitAudioSource>();
        audio.AddSource(true);
        audio.priority = 256;

        Vector3 spawnPos = Camera.main.transform.position;
        spawnPos += Camera.main.transform.right * 20f;
        spawnPos += Camera.main.transform.forward * 10f;
        spawnPos += Camera.main.transform.up * 5f;

        Vector3 destPos = Camera.main.transform.position;
        destPos += Camera.main.transform.right * -20f;
        destPos += Camera.main.transform.forward * 7f;
        destPos += Camera.main.transform.up * -5f;

        var smilzo = Instantiate(smilzoPrefab, spawnPos, Quaternion.identity) as GameObject;
        var rb = smilzo.GetComponent<Rigidbody>();
        
        yield return new WaitForSeconds(.1f);

        float t0 = Time.time;

        audio.PlayOneShot(music);
        while (Time.time < t0 + 10f)
        {
            var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
            
            var v = destPos - smilzo.transform.position;
            v.Normalize();
            rb.velocity = v * 3f;

            yield return new WaitForFixedUpdate();
        }

        Destroy(audio);
        Destroy(smilzo);
        yield break;
    }
}