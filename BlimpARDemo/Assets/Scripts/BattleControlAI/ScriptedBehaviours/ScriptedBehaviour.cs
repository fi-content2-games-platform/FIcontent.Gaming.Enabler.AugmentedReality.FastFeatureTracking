using UnityEngine;
using System.Collections;

public abstract class ScriptedBehaviour : MonoBehaviour
{
    protected bool running = false;
    public bool Running
    {
        get { return running; }
    }


    public float nextRunTime;


    public bool MustRun
    {
        get
        {
            return !running && Time.time > nextRunTime + startTime;
        }
    }

    [HideInInspector]
    public float startTime;

    public abstract IEnumerator Run();

    [ContextMenu("Run now!")]
    public void RunNow()
    {
        StartCoroutine(this.Run());
    }
}