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
    public float minWallDistance = 0.6f;
    public LayerMask objectMask;

    [Space]
    [Header("Double Jump")]
    public float doubleJumpForce = 8;
    public bool hasDoubleJump = true;

    [Space]
    [Header("Runner")]
    public float runnerSpeed = 1.6f;

    [Space]
    [Header("Hook")]
    public DistanceJoint2D distanceJoint;
    public float hookDistance = 5;
    public float swingSpeed = 3;
    public LineRenderer hookRenderer;
    public bool hooked = false;

    [Space]
    [Space]
    [Header("SwitchMenu")]
    public GameObject switchMenu;
    public GameObject none;
    public GameObject doubleJump;
    public GameObject run;
    public GameObject slowProj;
    public GameObject healer;
    public GameObject parry;
    public GameObject hook;
    private Personalty switchingTo;

    [Space]
    public Personalty personalty;
    private bool isJumping = false;
    private int flip = 1;
    private bool switching = false;
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

    private bool RunningOnAWall(float Horizontal)
    {
        if (Horizontal > 0 && Physics2D.Raycast(transform.position, Vector2.right, minWallDistance, objectMask))
            return true;

        if (Horizontal < 0 && Physics2D.Raycast(transform.position, Vector2.left, minWallDistance, objectMask))
            return true;

        return false;
    }

    void Update ()
    {
        //Basic movements and runner
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            flip = Input.GetAxis("Horizontal") > 0 ? 1 : -1;

        if (!hooked)
        {
            if(!RunningOnAWall(Input.GetAxis("Horizontal")))
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed * (personalty == Personalty.Run ? runnerSpeed : 1), rb.velocity.y, 0);
            else
                rb.velocity = new Vector3(0, rb.velocity.y, 0);

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
        if (Input.GetButtonDown("SwitchMenu"))
        {
            switchMenu.SetActive(true);
            Time.timeScale = 0.05f;
            switching = true;
        }
        else if (switching)
        {
            Vector3 direction = GetDirection(false);
            if(direction == new Vector3(0, 0, 0))
            {
                none.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                switchingTo = Personalty.Normal;

                run.transform.localScale = new Vector3(1, 1, 1);
                doubleJump.transform.localScale = new Vector3(1, 1, 1);
                hook.transform.localScale = new Vector3(1, 1, 1);
                parry.transform.localScale = new Vector3(1, 1, 1);
                healer.transform.localScale = new Vector3(1, 1, 1);
                slowProj.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                float angle = Vector3.Angle(direction, Vector3.right);
                if (Input.GetAxis("Vertical") < 0)
                    angle = 360 - angle;
                if(angle >= 0 && angle < 60)
                {
                    run.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                    switchingTo = Personalty.Run;

                    none.transform.localScale = new Vector3(1, 1, 1);
                    doubleJump.transform.localScale = new Vector3(1, 1, 1);
                    hook.transform.localScale = new Vector3(1, 1, 1);
                    parry.transform.localScale = new Vector3(1, 1, 1);
                    healer.transform.localScale = new Vector3(1, 1, 1);
                    slowProj.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (angle >= 60 && angle < 120)
                {
                    doubleJump.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                    switchingTo = Personalty.DoubleJump;

                    run.transform.localScale = new Vector3(1, 1, 1);
                    none.transform.localScale = new Vector3(1, 1, 1);
                    hook.transform.localScale = new Vector3(1, 1, 1);
                    parry.transform.localScale = new Vector3(1, 1, 1);
                    healer.transform.localScale = new Vector3(1, 1, 1);
                    slowProj.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (angle >= 120 && angle < 180)
                {
                    hook.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                    switchingTo = Personalty.Hook;

                    run.transform.localScale = new Vector3(1, 1, 1);
                    doubleJump.transform.localScale = new Vector3(1, 1, 1);
                    none.transform.localScale = new Vector3(1, 1, 1);
                    parry.transform.localScale = new Vector3(1, 1, 1);
                    healer.transform.localScale = new Vector3(1, 1, 1);
                    slowProj.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (angle >= 180 && angle < 240)
                {
                    parry.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                    switchingTo = Personalty.Parry;

                    run.transform.localScale = new Vector3(1, 1, 1);
                    doubleJump.transform.localScale = new Vector3(1, 1, 1);
                    hook.transform.localScale = new Vector3(1, 1, 1);
                    none.transform.localScale = new Vector3(1, 1, 1);
                    healer.transform.localScale = new Vector3(1, 1, 1);
                    slowProj.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (angle >= 240 && angle < 300)
                {
                    healer.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                    switchingTo = Personalty.Heal;

                    run.transform.localScale = new Vector3(1, 1, 1);
                    doubleJump.transform.localScale = new Vector3(1, 1, 1);
                    hook.transform.localScale = new Vector3(1, 1, 1);
                    parry.transform.localScale = new Vector3(1, 1, 1);
                    none.transform.localScale = new Vector3(1, 1, 1);
                    slowProj.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (angle >= 300 && angle < 360)
                {
                    slowProj.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                    switchingTo = Personalty.SlowProj;

                    run.transform.localScale = new Vector3(1, 1, 1);
                    doubleJump.transform.localScale = new Vector3(1, 1, 1);
                    hook.transform.localScale = new Vector3(1, 1, 1);
                    parry.transform.localScale = new Vector3(1, 1, 1);
                    healer.transform.localScale = new Vector3(1, 1, 1);
                    none.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            

            if (Input.GetButtonUp("SwitchMenu"))
            {
                //Change skin
                personalty = switchingTo;
                switchingTo = Personalty.Normal;
                switchMenu.SetActive(false);
                Time.timeScale = 1;
                switching = false;
            }
        }

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
                hookRenderer.enabled = false;
            }
        }

        if (hooked)
        {
            hookRenderer.SetPosition(0, transform.position);

            if (Input.GetAxis("Horizontal") != 0)
            {
                Vector3 vector = (transform.position - hookRenderer.GetPosition(1)).normalized;
                //Getting the orthogonal vector of the vector witch represent the rope.
                if(Input.GetAxis("Horizontal") < 0)
                {
                    Vector2 ortho = new Vector2(vector.y, -vector.x);
                    Debug.DrawLine(transform.position, transform.position + (Vector3)ortho);
                    rb.AddForce(ortho * swingSpeed, ForceMode2D.Force);
                }
                else
                {
                    Vector2 ortho = new Vector2(-vector.y, vector.x);
                    Debug.DrawLine(transform.position, transform.position + (Vector3)ortho);
                    rb.AddForce(ortho * swingSpeed, ForceMode2D.Force);
                }
            }
        }

        if (Input.GetButtonUp("Action"))
        {
            distanceJoint.enabled = false;
            hookRenderer.enabled = false;
            hooked = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsGrounded)
            hasDoubleJump = true;
    }

    private Vector3 GetDirection(bool useAimCorrection = true)
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        if (useAimCorrection)
        {
            if (Horizontal == 0 && Vertical < 0.8f)
                Horizontal = 0.3f * flip;
            if (Vertical == 0)
                Vertical = 1;
        }

        return new Vector3(Horizontal, Vertical, 0).normalized;
    }
}

public enum Personalty { Normal, DoubleJump, Run, Hook, Parry, Heal, SlowProj }
