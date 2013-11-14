using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public GameObject laserPrefab;
    public float rotationSpeed = 5f;
    private UnitAudioSource unitAudioSource;
    public AudioClip attackSound;
    private const int MAX_LASER_SHOTS = 3;

    // Use this for initialization
    void Start()
    {
        unitAudioSource = this.gameObject.AddComponent<UnitAudioSource>();
        unitAudioSource.AddSource();
        unitAudioSource.volumeFactor = .2f;
        unitAudioSource.maxDistance = 500;
        unitAudioSource.SetVolume();
    }

    // Update is called once per frame
    void Update()
    {

        var x = -Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed;
        var y = -Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;


        this.transform.Rotate(new Vector3(x, y, 0));


        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray))
                {
                    StartCoroutine(Fire());
                }

            }
            ++i;
        }
    }

    IEnumerator Fire()
    {

        unitAudioSource.PlayOneShot(attackSound);

        for (int n = 0; n < MAX_LASER_SHOTS; n++)
        {
            var g = Instantiate(laserPrefab, transform.position, transform.rotation) as GameObject;
            g.transform.parent = Containers.Lasers;

            //if (GetTarget()) GetTarget().SendMessage("Hit", controller.attackDamage, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(0.1f);
        } //for n shots 


    }

    [ContextMenu("Shoot")]
    void ShootFromCam()
    {
        StartCoroutine(Fire());
    }


}
