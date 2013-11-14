using UnityEngine;
using System.Collections;

public class Stars : MonoBehaviour
{
    public Texture2D[] spaceObjectsTextures;
    public GameObject planePrefab;
    public float spawnRadius = 500f;
    public float size = 1000f;

    private ParticleSystem particles;
    private Transform[] spaceObjects;
    private Transform target;
        

    // Use this for initialization
    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.Simulate(40f);  //prewarms the star system

        target = Camera.main.transform;

        GameObject tmpGo;

        //spawn the objects around a circle
        spaceObjects = new Transform[spaceObjectsTextures.Length];
        for(int i = 0;i < spaceObjectsTextures.Length ; i++)
        {
            Vector3 position = Random.onUnitSphere * spawnRadius;
            tmpGo = Instantiate(planePrefab, position, Quaternion.identity) as GameObject;
            tmpGo.transform.parent = this.transform;

            var renderer = tmpGo.GetComponentInChildren<Renderer>();
            renderer.material.mainTexture = spaceObjectsTextures[i];
            tmpGo.transform.localScale *= size  * Random.Range(0.8f, 4f);
            spaceObjects[i] = tmpGo.transform;

        }

    }

    // Update is called once per frame
    void Update()
    {
        //face the camera
        foreach (var t in spaceObjects)
        {                      
            //t.LookAt(t.position + target.rotation * Vector3.forward, target.rotation * Vector3.up);                       
            t.LookAt(target.position - t.position);
            
        }                
    }
}
