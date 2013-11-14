using UnityEngine;
using System.Collections;


public class CameraSquad : ScriptedBehaviour
{
    public Transform lateTarget;
    public GameObject squad;
    public int flockSize = 10;
    public BattleAI battleAI;

    public CameraSquad()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;

        Vector3 spawnPos = ScriptedBehaviourManager.behindCamera.transform.position;
        spawnPos.y += 20f;
        spawnPos.z -= 2f;

        var s = GameObject.Instantiate(squad, spawnPos, Camera.main.transform.rotation) as GameObject;
        var boid = s.GetComponent<BoidController>();
        boid.flockSize = flockSize;
        boid.flyInFormation = false;
        boid.maxRotation = 10f;
        boid.maxVelocity *= 2;
        boid.minVelocity *= 2;

        s.transform.parent = this.transform;


        yield return new WaitForSeconds(5f);

        boid.maxRotation = 100f;
        boid.target = battleAI.emperror.Stations[0].transform;
        
        yield return new WaitForSeconds(8f);

        boid.maxVelocity *= 2;
        boid.minVelocity *= 2;
        boid.target = lateTarget;

        GameObject.Destroy(s, 15f);
    }
}