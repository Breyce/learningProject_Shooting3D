using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float headth;
    protected bool dead;

    protected virtual void Start()
    {
        headth = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        headth -= damage;
        if(headth <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }
}
