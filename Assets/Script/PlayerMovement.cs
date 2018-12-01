using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float jumpDownForce = 3;
    public float groundDistance = 0.8f;
    public int flip = 1;
    public LayerMask objectMask;

    [Space]
    [Header("Double Jump")]
    public float doubleJumpForce = 8;
    public bool hasDoubleJump = true;

    [Space]
    [Header("Runner")]
    public float runnerSpeed = 1.5f;

    [Space]
    [Header("Hook")]
    public DistanceJoint2D distanceJoint;
    public float hookDistance = 5;
    public LineRenderer hookRenderer;
    public bool hooked = false;

    [Space]
    public Personalty personalty;
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
            if (Physics2D.Raycast(transform.position, Vector2.down, groundDistance, objectMask))
                return true;
                
            return false;
        }
    }

    void Update ()
    {
        //Basic movements and runner
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            flip = Input.GetAxis("Horizontal") > 0 ? 1 : -1;

        if (!hooked)
        {
            rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed * (personalty == Personalty.Run ? runnerSpeed : 1), rb.velocity.y, 0);

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0.8f && !IsGrounded && isJumping)
            {
                isJumping = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, -jumpDownForce * rb.velocity.y / 9), ForceMode2D.Impulse);
            }
        }
            
        //Personality Switch
        //if (Input.GetButtonDown("SwitchMenu"))
        //{

        //}
        //else if (Input.GetButtonUp("SwitchMenu"))
        //{

        //}

        //Double Jump
        if (personalty == Personalty.DoubleJump && Input.GetButtonDown("Jump") && hasDoubleJump && !IsGrounded)
        {
            rb.AddForce(new Vector2(0, doubleJumpForce), ForceMode2D.Impulse);
            hasDoubleJump = false;
        }

        //Hook
        if(personalty == Personalty.Hook)
        {
            CalculateHook();
        }
        if (hooked)
            hookRenderer.SetPosition(0, transform.position);
	}

    private async void CalculateHook()
    {
        if (Input.GetButtonDown("Action"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, GetDirection(), hookDistance, objectMask);
            if (hit.collider != null)
            {
                hooked = true;
                distanceJoint.connectedAnchor = hit.point;
                distanceJoint.distance = Vector3.Distance(hit.point, transform.position);
                distanceJoint.enabled = true;

                hookRenderer.SetPositions(new Vector3[] { transform.position, hit.point });
                hookRenderer.enabled = true;
            }
            else
            {
                hookRenderer.SetPositions(new Vector3[] { transform.position, transform.position + GetDirection() * hookDistance });
                hookRenderer.enabled = true;
                await Task.Delay(100);
                hookRenderer.enabled = true;
            }
        }
        if (Input.GetButtonUp("Action"))
        {
            distanceJoint.enabled = false;
            hooked = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsGrounded)
            hasDoubleJump = true;
    }

    private Vector3 GetDirection()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        //if (Horizontal == 0)
        //    Horizontal = 1 * flip;
        if (Vertical == 0)
            Vertical = 1 * flip;

        return new Vector3(Horizontal, Vertical, 0).normalized;
    }
}

public enum Personalty { Normal, DoubleJump, Run, Hook }
