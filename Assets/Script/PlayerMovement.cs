using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float jumpDownForce = 3;
    public float groundDistance = 0.8f;
    public LayerMask groundMask;

    private bool isJumping = false;
    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool IsGrounded
    {
        get
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundMask))
                return true;
                
            return false;
        }
    }

    void Update ()
    {
        rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, rb.velocity.y, 0);

        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
        }
            
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0.8f && !IsGrounded && isJumping)
        {
            isJumping = false;
            rb.AddForce(new Vector2(0, -jumpDownForce * rb.velocity.y / 5), ForceMode2D.Impulse);
        }
	}
}
