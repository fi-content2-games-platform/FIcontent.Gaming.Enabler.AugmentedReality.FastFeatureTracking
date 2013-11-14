using UnityEngine;
using System.Collections;

public abstract class Ship : MonoBehaviour
{
    public float health;
    public GameObject explosionPrefab;

    public AudioClip attackSound;

    protected UnitAudioSource unitAudioSource;

    protected float pitchVariation
    {
        get { return Random.Range(-.1f, .1f); }
    }

    protected bool alive = true;

    protected abstract void Awake();
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void FixedUpdate();

    /// <summary>
    /// Explosion
    /// </summary>
    protected abstract void Explode();    

    /// <summary>
    /// Damages the ship, if the hp drops below zero the ship explodes
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    protected virtual void Hit(float damage)
    {
        health -= damage;

        if (health < 0 && alive)
        {
            alive = false;
            Explode();
        }
    }

    /// <summary>
    /// Enforce minimum and maximum speeds for the ship
    /// </summary>
    /// <param name="max">max speed</param>
    /// <param name="min">min speed</param>
    protected void ControlSpeed(float max, float min)
    {
        // 
        float speed = rigidbody.velocity.magnitude;
        if (speed > max)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * max;
        }
        else if (speed < min)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * min;
        }
    }

    [ContextMenu("Kill Ship!")]
    public void Kill()
    {
        this.Hit(999999f);
    }
}
