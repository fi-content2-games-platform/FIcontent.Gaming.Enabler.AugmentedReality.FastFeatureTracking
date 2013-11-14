using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptedBehaviourManager : MonoBehaviour
{
    public static GameObject behindCamera;
    public static GameObject infrontCamera;
    
    public UILabel label;
    
    private float startime;
    private ScriptedBehaviour[] behaviours;

    void Awake()
    {
        //init reference points
        behindCamera = new GameObject("behind camera");
        behindCamera.transform.position = Camera.main.transform.position - Camera.main.transform.forward * 300;
        behindCamera.transform.parent = Camera.main.transform;


        infrontCamera = new GameObject("in front of camera");
        infrontCamera.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 300;
        infrontCamera.transform.parent = Camera.main.transform;
        
        //get children
        behaviours = this.GetComponentsInChildren<ScriptedBehaviour>();
        
        //game start event
        GameEventManager.GameStartEvent += StartGameEvent;
    }

    // Update is called once per frame
    void Update()
    {

        if (startime > 0)
            label.text = (Time.time - startime).ToString("0.00");

        if (!GameEventManager.GameStarted)
            return;

        if (behaviours != null)
            foreach (var b in behaviours)
            {
                if (b.MustRun && b.gameObject.active)
                    StartCoroutine(b.Run());
            }
    }
    
    void OnDrawGizmos()
    {
        float a = Camera.main.fieldOfView / 2;
        float d = Camera.main.farClipPlane;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Quaternion.Euler(0, a, 0) * Camera.main.transform.forward * d);
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Quaternion.Euler(0, -a, 0) * Camera.main.transform.forward * d);

        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Quaternion.Euler(a, 0, 0) * Camera.main.transform.forward * d);
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Quaternion.Euler(-a, 0, 0) * Camera.main.transform.forward * d);


        if (behindCamera && infrontCamera)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Camera.main.transform.position, behindCamera.transform.position);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Camera.main.transform.position, infrontCamera.transform.position);
        }

        Gizmos.color = new Color(.5f, .5f, .5f, .2f);
        Gizmos.DrawSphere(Camera.main.transform.position, 20f);
    }

    void StartGameEvent()
    {
        startime = Time.time;

        if (behaviours != null)
            foreach (var b in behaviours)
            {
                b.startTime = Time.time;
            }
    }

}








