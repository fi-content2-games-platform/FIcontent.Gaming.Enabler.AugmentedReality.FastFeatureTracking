using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Asteroids : ScriptedBehaviour
{
    public GameObject asteroidPrefab;
    public Vector3 spawnPos;

    public int asteroidCount = 50;
    private List<RotatingAsteroid> asteroids = new List<RotatingAsteroid>();

    public Asteroids()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;

        yield return new WaitForSeconds(.1f);

        StartCoroutine(MoveAsteroids());

        yield return new WaitForSeconds(80f);

        foreach (var a in asteroids)
        {
            Destroy(a.obj);
        }

        yield break;
    }

    IEnumerator MoveAsteroids()
    {
        float t0 = Time.time;
        spawnPos = Camera.main.transform.position;
        spawnPos += Camera.main.transform.right * -150f;
        spawnPos += Camera.main.transform.forward * 125f;
        spawnPos += Camera.main.transform.up * 60f;

        Vector3 destPos = Camera.main.transform.position;
        destPos += Camera.main.transform.right * 200f;
        destPos += Camera.main.transform.forward * 70f;
        destPos += Camera.main.transform.up * -50f;

        for (int i = 0; i < asteroidCount; i++)
        {
            var rot = Random.insideUnitSphere * 100f;
            var smilzo = Instantiate(asteroidPrefab, spawnPos + Random.insideUnitSphere * 50f, Quaternion.identity) as GameObject;
            smilzo.transform.parent = this.transform;
            asteroids.Add(new RotatingAsteroid() { obj = smilzo, rot = rot, v = Random.Range(4f, 8f) });
        }

        while (Time.time < t0 + 70f)
        {
            foreach (var a in asteroids)
            {
                if (!a.obj) continue;

                var deltaRotation = Quaternion.Euler(a.rot * Time.deltaTime);
                a.obj.rigidbody.MoveRotation(a.obj.rigidbody.rotation * deltaRotation);

                var v = destPos - a.obj.transform.position;
                v.Normalize();
                a.obj.rigidbody.velocity = v * a.v;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    struct RotatingAsteroid
    {
        public Vector3 rot;
        public GameObject obj;
        public float v;
    }
}