using UnityEngine;
using System.Collections;

public class PlayerRailgun : MonoBehaviour {
	
	public GameObject railgunEffect;
	public GameObject railgunHit;
	public float railDamage = 20.0f;
	public float railLength = 100.0f;
	
	public int score = 0;
	public int high_score = 0;
	
	public AudioClip attackSound;
	private UnitAudioSource unitAudioSource;
	
	public bool firing = false;
	public bool buttonPressed = false;
	
	public void EndGame()
	{
		if (score>high_score)
		{
			high_score = score;
			PlayerPrefs.SetInt(@"High Score", high_score);
			PlayerPrefs.Save();
		}
		score = 0;
		enabled = false;
	}
	
	void Awake()
	{
		high_score = PlayerPrefs.GetInt(@"High Score", 0);
	}

	// Use this for initialization
	void Start () {
		unitAudioSource = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudioSource.AddSource(true);
		unitAudioSource.volumeFactor=0.1f;
		
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!firing && !buttonPressed)
			return;
		
		if(!firing)
			StartCoroutine(Fire());
	}
	
	IEnumerator Fire()
	{
		firing = true;
		
		var r = Instantiate(railgunEffect, transform.position, transform.rotation) as GameObject;
		Destroy(r, 0.1f);
		
		unitAudioSource.PlayOneShot(attackSound);

		
		LineRenderer lr = r.GetComponent<LineRenderer>();
		if(lr == null)
			Debug.LogError("Linerenderer component expected in LaserEffect prefab");
		else
		{
			lr.SetPosition(0, transform.position-transform.up-transform.forward);
			lr.SetPosition(1, transform.position+transform.forward * railLength);
		}
		
		RaycastHit[] hits;
		hits = Physics.RaycastAll(transform.position, transform.forward);
        foreach(RaycastHit hit in hits)
		{
			hit.collider.gameObject.SendMessage("Hit", railDamage, SendMessageOptions.DontRequireReceiver);
			if (hit.collider.gameObject.name.Contains("Emperror"))
			{
				score += 10;
			}
			else if (hit.collider.gameObject.name.Contains("Rebel"))
			{
				score -= 2;
			}
			
			var e = Instantiate(railgunHit, hit.point, Random.rotation) as GameObject;
			Destroy(e, 0.2f);
		}
		
		yield return new WaitForSeconds(0.2f);
		buttonPressed = false;
		firing = false;
	}
}
