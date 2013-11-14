using UnityEngine;
using System.Collections;

public class AttackCamera : ScriptedBehaviour
{
    public int flockSize =2 ;
    public GameObject prefab;

    public AttackCamera()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;
        Vector3 spawnPos = ScriptedBehaviourManager.behindCamera.transform.position;
        spawnPos += ScriptedBehaviourManager.behindCamera.transform.up * -150f;
                        
        var s = GameObject.Instantiate(prefab, spawnPos, Camera.main.transform.rotation) as GameObject;
        s.transform.parent = this.transform;

        var boid = s.GetComponent<BoidController>();
        boid.flockSize = flockSize;
        boid.flyInFormation = false;       
        boid.maxVelocity = boid.minVelocity = 150f;
        boid.attackDistance = 200f;
        boid.universeCenter = Containers.UniverseCenter.transform.position;

        yield return new WaitForSeconds(6f);

        var targetGo = new GameObject("target");
        targetGo.transform.parent = this.transform;
        targetGo.transform.position = Camera.main.transform.position;
        targetGo.transform.position += targetGo.transform.up * -2f;
        targetGo.transform.position += targetGo.transform.right;
        Destroy(targetGo, 30f);
        boid.target = Camera.main.transform;
        

        yield return new WaitForSeconds(12f);

        var go = new GameObject("destination");
        go.transform.parent = this.transform;
        go.transform.position = spawnPos + ScriptedBehaviourManager.behindCamera.transform.position;
        //boid.maxVelocity = boid.minVelocity = 350f;     

        boid.universeCenter = null;
        boid.target = go.transform;
        GameObject.Destroy(s, 5f);

        GameObject.Destroy(go, 10f);
    }
}