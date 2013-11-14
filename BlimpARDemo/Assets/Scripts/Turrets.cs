using UnityEngine;
using System.Collections;

public class Turrets : MonoBehaviour {

    public GameObject turretPrefab;

	// Use this for initialization
	void Start () {

        foreach (Transform t in transform)
        {
            var g = Instantiate(turretPrefab, t.position, t.rotation) as GameObject;
            g.transform.parent = t;
        }

	}	
}
