using UnityEngine;
using System.Collections;

public class FlareExplosion : MonoBehaviour
{
    public float maxBrightness;
    public float duration;
    private LensFlare lensflare;
    private float startTime;
            

    void Start()
    {
        startTime = Time.time;
        lensflare = this.GetComponent<LensFlare>();
        lensflare.enabled = true;
        lensflare.brightness = 0.0f;
        Destroy(gameObject, duration);
            
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - startTime;
        float t = currentTime * Mathf.PI / duration;
        lensflare.brightness = maxBrightness * Mathf.Sin(t);
    }

    void SetDuration(float d) { duration = d; }
    void SetMaxBrightness(float b) { maxBrightness = b; }
}
