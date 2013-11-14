using UnityEngine;
using System.Collections;

public class AttackBlimpSquadron : ScriptedBehaviour
{
    public int flockSize = 4;
    public GameObject prefab;
    public Transform blimp;

    public AttackBlimpSquadron()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;
        Vector3 spawnPos = ScriptedBehaviourManager.behindCamera.transform.position;

        var s = GameObject.Instantiate(prefab, spawnPos, Camera.main.transform.rotation) as GameObject;
        var boid = s.GetComponent<BoidController>();
        boid.flockSize = flockSize;
        boid.attackDistance = 1500f;
        boid.flyInFormation = false;
        boid.target = blimp;
        boid.universeCenter = blimp.transform.position;

        s.transform.parent = this.transform;

        yield return new WaitForSeconds(20f);

        var go = new GameObject("destination");
        go.transform.parent = this.transform;
        go.transform.position = spawnPos;

        boid.universeCenter = null;
        boid.target = go.transform;
        GameObject.Destroy(s, 10f);

        GameObject.Destroy(go, 10f);
    }
}