using UnityEngine;
using System.Collections;

public class CameraSounds : MonoBehaviour {
        
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	


	}

    public void PlaySound(AudioClip clip)
    {
        audio.PlayOneShot(clip);
    }
}

