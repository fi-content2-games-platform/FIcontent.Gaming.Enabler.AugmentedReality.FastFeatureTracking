using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour
{

    public GameObject explosion;
    
    // Use this for initialization
    void Start()
    {
        if (!explosion)
            Debug.LogError("Assign the explosion", this);

        this.transform.localScale *= Random.Range(.9f, 2f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        other.gameObject.SendMessage("Hit", 51f,  SendMessageOptions.DontRequireReceiver);      
    }

    public void Explode(bool asd)
    {
        var expl = Instantiate(explosion, this.transform.position, Random.rotation) as GameObject;
        expl.transform.parent = Containers.Explosions;

        Destroy(gameObject);
    }
}
