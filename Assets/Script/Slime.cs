﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Slime : MonoBehaviour
{
    private float nextJump = 0;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        nextJump -= Time.deltaTime;
        if(nextJump < 1)
        {
            rb.velocity = new Vector2(Random.Range(-5, 6), Random.Range(1, 15));
            nextJump = Random.Range(1, 5);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "PushProjectile")
        {
            rb.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity * 5;
            Destroy(collision.gameObject);
        }
    }
}
