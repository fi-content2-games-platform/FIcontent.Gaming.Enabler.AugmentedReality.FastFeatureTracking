using UnityEngine;
using System.Collections;

public class FightNearCamera : ScriptedBehaviour
{
    public GameObject squadPrefab1;
    public GameObject squadPrefab2;
    public int flockSize = 10;

    public FightNearCamera()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;

        Vector3 spawnPos = Camera.main.transform.position;
        spawnPos += Camera.main.transform.right * 500f;
        spawnPos += Camera.main.transform.forward * 100f;

        var squad1 = GameObject.Instantiate(squadPrefab1, spawnPos, Quaternion.LookRotation(ScriptedBehaviourManager.infrontCamera.transform.position)) as GameObject;
        squad1.transform.parent = this.transform;
        var boid1 = squad1.GetComponent<BoidController>();

        spawnPos = Camera.main.transform.position;
        spawnPos += Camera.main.transform.right * -500f;
        spawnPos += Camera.main.transform.up * 100f;

        var squad2 = GameObject.Instantiate(squadPrefab2, spawnPos, Quaternion.LookRotation(ScriptedBehaviourManager.infrontCamera.transform.position)) as GameObject;
        squad2.transform.parent = this.transform;
        var boid2 = squad2.GetComponent<BoidController>();


        boid2.maxVelocity = boid1.maxVelocity = 150;
        boid2.minVelocity = boid1.minVelocity = 100;
        boid2.flyInFormation = boid1.flyInFormation = false;
        boid2.randomness = boid1.randomness = 180;
        boid2.flockSize = boid1.flockSize = flockSize;
        boid2.target = boid1.target = ScriptedBehaviourManager.infrontCamera.transform;
        boid2.attackDistance = boid1.attackDistance = 10f;

        yield return new WaitForSeconds(.5f);

        boid1.maxRotation = 300f;
        boid2.maxRotation = 200f;
        boid2.attackDistance = boid1.attackDistance = 200f;
        boid2.universeCenter = boid1.universeCenter = ScriptedBehaviourManager.infrontCamera.transform.position;
        boid1.target = boid2.transform;
        boid2.target = boid1.transform;

        yield return new WaitForSeconds(11f);

        var go = new GameObject("destination");
        go.transform.parent = this.transform;
        go.transform.position = spawnPos * 3;

        if (boid1)
        {
            boid1.maxVelocity = 300f;
            boid1.minVelocity = 300f;
            boid1.universeCenter = null;
            boid1.target = go.transform;
            GameObject.Destroy(squad1, 15f);
        }

        if (boid2)
        {
            boid2.maxVelocity = 300f;
            boid2.minVelocity = 300f;
            boid2.universeCenter = null;
            boid2.target = go.transform;
            GameObject.Destroy(squad2, 15f);
        }

        GameObject.Destroy(go, 20f);

    }
}