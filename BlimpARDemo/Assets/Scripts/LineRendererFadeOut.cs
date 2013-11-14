using UnityEngine;
using System.Collections;

public class LineRendererFadeOut : MonoBehaviour {
	
	public float decayTime = 1.0f;
	public float decayDelay = 0.33f;
	
	private float startTime;
	private float decayStep;
	LineRenderer lineRenderer;
	
	// Use this for initialization
	void Start () {
		this.lineRenderer = this.GetComponent<LineRenderer>();
		
		Color c = this.lineRenderer.material.GetColor("_TintColor");
		decayStep = c.grayscale / decayTime;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > startTime + decayDelay)
		{
			Color c = this.lineRenderer.material.GetColor("_TintColor");
			c.a -= decayStep * Time.deltaTime;
        	this.lineRenderer.material.SetColor("_TintColor", c);
		}
	}
}
