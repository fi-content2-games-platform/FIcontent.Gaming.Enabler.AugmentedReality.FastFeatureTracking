using UnityEngine;
using System.Collections;

public class EditorTweenSpeedup : MonoBehaviour
{
     

#if UNITY_EDITOR
    // Use this for initialization
    void Start()
    {
        //GetComponent<TweenPosition>().duration = 1f;
    }
#endif
}
