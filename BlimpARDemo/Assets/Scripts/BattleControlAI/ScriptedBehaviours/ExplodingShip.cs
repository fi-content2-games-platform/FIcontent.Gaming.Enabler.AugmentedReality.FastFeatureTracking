using UnityEngine;
using System.Collections;

public class ExplodingShip : ScriptedBehaviour
{
    public GameObject tShip;
    public GameObject laser;
    public GameObject explosion, explosion2;
    public AudioClip laserClip;
    public float startShootingAt = 3.7f;
    public Vector3 deltaPosition = Vector3.zero;

    public ExplodingShip()
        : base()
    {

    }

    public override IEnumerator Run()
    {
        running = true;

        var audio = this.gameObject.AddComponent<UnitAudioSource>();
        audio.AddSource(true);

        Vector3 spawnPos = Camera.main.transform.position;
        spawnPos += Camera.main.transform.right * 400f;
        spawnPos += Camera.main.transform.forward * 570f;
        spawnPos += deltaPosition;

        var dest = new GameObject("dest");
        dest.transform.parent = this.transform;
        dest.transform.position = Camera.main.transform.position;
        dest.transform.position += Camera.main.transform.right * -20f;

        var shotPos = new GameObject("shotPos1");
        shotPos.transform.parent = this.transform;
        shotPos.transform.position = Camera.main.transform.position;
        shotPos.transform.position += Camera.main.transform.right * -10f;
        shotPos.transform.position += Camera.main.transform.up * -10f;

        var shotPos2 = new GameObject("shotPos2");
        shotPos2.transform.parent = this.transform;
        shotPos2.transform.position = Camera.main.transform.position;
        shotPos2.transform.position += Camera.main.transform.right * -15f;
        shotPos2.transform.position += Camera.main.transform.up * -10f;

        var ship = Instantiate(tShip, spawnPos, Quaternion.identity) as GameObject;
        ship.transform.parent = this.transform;
        var ts = ship.GetComponent<TitleShip>();
        ts.duration = 40f;
        ts.speed = 100f;
        ts.direction = dest.transform;


        yield return new WaitForSeconds(startShootingAt);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(.5f);
            audio.PlayOneShot(laserClip);
            for (int i1 = 0; i1 < 3; i1++)
            {
                StartCoroutine(ShootTheShip(ship.transform, shotPos.transform.position, shotPos2.transform.position));
                StartCoroutine(HitTheShip(ship.transform));
                yield return new WaitForSeconds(.05f);
            }
        }
        ts.Explode();
        
        yield return new WaitForSeconds(1f);

        var expl2 = Instantiate(explosion2, ship.transform.position, Quaternion.identity) as GameObject;
        expl2.transform.parent = Containers.Explosions;
        Destroy(ship);

        yield return new WaitForSeconds(5f);

        Destroy(dest);
        Destroy(audio);
        Destroy(shotPos);
        Destroy(shotPos2);

        yield break;
    }

    IEnumerator HitTheShip(Transform ship)
    {
        yield return new WaitForSeconds(.5f);

        var expl = Instantiate(explosion, ship.position, Quaternion.identity) as GameObject;
        expl.transform.parent = Containers.Explosions;
    }

    IEnumerator ShootTheShip(Transform ship, Vector3 p1, Vector3 p2)
    {

        var targetPos = ship.position + ship.forward * 15f - p1;

        var lazor = Instantiate(laser, p1, Quaternion.LookRotation(targetPos)) as GameObject;
        lazor.transform.parent = Containers.Lasers;
        yield return new WaitForSeconds(.05f);

        var lazor2 = Instantiate(laser, p2, Quaternion.LookRotation(targetPos)) as GameObject;
        lazor2.transform.parent = Containers.Lasers;
        yield return new WaitForSeconds(.05f);

    }

}