using UnityEngine;
using System.Collections;

public class Blimp : MonoBehaviour
{

    public GameObject explosion;
    public static bool BlimpVisible;

    public delegate void BlimpEvent();

    public static event BlimpEvent BlimpFoundEvent, BlimpLostEvent;

    void Start()
    {

        if (!explosion)
            Debug.LogError("explosion object missing", this);

        BlimpFoundEvent += delegate()
        {
            Debug.Log("blimp found");
        };
        BlimpLostEvent += delegate()
        {
            Debug.Log("blimp lost");
        };
    }

    public void Explode(Transform target)
    {

        var go = Instantiate(explosion, target.position, target.rotation) as GameObject;
        go.transform.parent = Containers.Explosions;
    }

    void OnBecameVisible()
    {

        if (BlimpFoundEvent != null)
            BlimpFoundEvent();

        BlimpVisible = true;

    }

    void OnBecameInvisible()
    {


        if (BlimpLostEvent != null)
            BlimpLostEvent();

        BlimpVisible = false;
    }


}
