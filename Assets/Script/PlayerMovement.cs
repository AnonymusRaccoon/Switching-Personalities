using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    [Header("Renderer")]
    public new SpriteRenderer renderer;
    public Animator animator;

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
    [Header("Heal")]
    public GameObject healParticules;

    [Space]
    [Header("Slow Projectile")]
    public GameObject SlowProjectile;
    public float projSpeed = 1.2f;

    [Space]
    [Header("Parry")]
    public float parryForce = 12f;

    [Space]
    [Space]
    [Header("Menues")]
    public GameObject switchMenu;
    public GameObject none;
    public GameObject doubleJump;
    public GameObject run;
    public GameObject slowProj;
    public GameObject healer;
    public GameObject parry;
    public GameObject hook;
    public GameObject DeathScreen;
    public GameObject[] HealPoints;
    public TextMeshProUGUI Info;
    public GameObject Dialog;
    private Personalty switchingTo;

    private Personalty personalty;
    private Vector3 checkPoint;
    private int HealPoint = 5;
    private bool isJumping = false;
    private int flip = 1;
    private bool switching = false;
    private bool parrying = false;
    public bool useController = false;
    private Interactable interactable;
    private Rigidbody2D rb;
    private new BoxCollider2D collider;

    private async void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponentInChildren<BoxCollider2D>();
        checkPoint = transform.position;

        if (Input.GetJoystickNames().Length > 0)
        {
            useController = true;
            Info.text = "Gamepad detected, setting keybind for the gamepad. Don't want to use the gamepad ? Press Escape to switch back to the keyboard.";
            Info.gameObject.SetActive(true);
            await Task.Delay(5000);
            Info.gameObject.SetActive(false);
        }
    }

    private bool IsGrounded
    {
        get
        {
            if (Physics2D.BoxCast(transform.position, new Vector2(collider.bounds.size.x, groundDistance), 270, Vector2.down, groundDistance, objectMask))
                return true;
                
            return false;
        }
    }

    private bool RunningOnAWall(float Horizontal)
    {
        if (Horizontal > 0 && Physics2D.BoxCast(transform.position, new Vector2(minWallDistance, collider.bounds.size.y), 0, Vector2.right, minWallDistance, objectMask))
            return true;

        if (Horizontal < 0 && Physics2D.BoxCast(transform.position, new Vector2(minWallDistance, collider.bounds.size.y), 180, Vector2.left, minWallDistance, objectMask))
            return true;

        return false;
    }

    private async void SwitchToController()
    {
        Info.text = "Switching to controller. Press Escape to switch back to the keyboard.";
        Info.gameObject.SetActive(true);
        await Task.Delay(5000);
        Info.gameObject.SetActive(false);
    }

    private async void SwitchToKeyboard()
    {
        Info.text = "Switching to keyboard. Press Escape to switch back to the controller.";
        Info.gameObject.SetActive(true);
        await Task.Delay(5000);
        Info.gameObject.SetActive(false);
    }

    private void Update ()
    {
        if(Time.timeScale == 0)
        {
            if (Input.anyKeyDown)
            {
                transform.position = checkPoint;
                renderer.enabled = true;

                HealPoint = 5;
                foreach (GameObject hp in HealPoints)
                    hp.SetActive(true);

                Time.timeScale = 1;
                DeathScreen.SetActive(false);
            }

            return;
        }

        //Switch from keyboard to controller and vice versa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (useController)
            {
                useController = false;
                SwitchToKeyboard();
            }
            else
            {
                useController = true;
                SwitchToController();
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
            if (direction == new Vector3(0, 0, 0))
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

                if ((Input.GetAxis("Vertical") < 0 && useController) || (Input.mousePosition.y < Screen.height / 2 && !useController))
                    angle = 360 - angle;



                if (angle >= 0 && angle < 60)
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

        if (switching)
            return;

        //Basic movements and runner
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            flip = Input.GetAxis("Horizontal") > 0 ? 1 : -1;

        renderer.flipX = flip == -1;

        if (!hooked)
        {
            if (!RunningOnAWall(Input.GetAxis("Horizontal")))
            {
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed * (personalty == Personalty.Run ? runnerSpeed : 1), rb.velocity.y, 0);
                animator.SetBool("isRunning", Mathf.Abs(Input.GetAxis("Horizontal")) > .3f);
            }
            else
                rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
                animator.SetBool("isJumping", isJumping);
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0.8f && !IsGrounded && isJumping)
            {
                isJumping = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, -jumpDownForce * rb.velocity.y / 9), ForceMode2D.Impulse);
                animator.SetBool("isJumping", isJumping);
            }
        }

        //Double Jump
        if (personalty == Personalty.DoubleJump && Input.GetButtonDown("Jump") && hasDoubleJump && !IsGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, doubleJumpForce), ForceMode2D.Impulse);
            hasDoubleJump = false;
            animator.SetBool("isJumping", isJumping);
        }

        //Hook
        if(personalty == Personalty.Hook)
        {
            CalculateHook();
        }

        //Heal
        if (personalty == Personalty.Heal && Input.GetButtonDown("Action"))
        {
            StartHealing();
        }

        //Slow Projectiles
        if(personalty == Personalty.SlowProj && Input.GetButtonDown("Action"))
        {
            GameObject proj = Instantiate(SlowProjectile, transform.position + Vector3.right * flip, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().velocity = GetDirection(true, true) * projSpeed;
        }

        //Parry
        if(personalty == Personalty.Parry && Input.GetButtonDown("Action"))
        {
            Parry();
        }

        //Interactions
        if(interactable != null && Input.GetButtonDown("Interact"))
        {
            if (interactable.index + 1 < interactable.dialog.Length)
            {
                Info.gameObject.SetActive(false);
                Dialog.SetActive(true);
                Dialog.GetComponentInChildren<TextMeshProUGUI>().text = interactable.dialog[interactable.index + 1];
                interactable.index++;
            }
            else
            {
                Dialog.SetActive(false);
                interactable.index = -1;
                Info.gameObject.SetActive(true);
            }
        }
	}

    private async void CalculateHook()
    {
        if (Input.GetButtonDown("Action"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, GetDirection(true, true), hookDistance, objectMask);
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
                hookRenderer.SetPositions(new Vector3[] { transform.position, transform.position + GetDirection(true, true) * hookDistance });
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
                    rb.AddForce(ortho * swingSpeed, ForceMode2D.Force);
                }
                else
                {
                    Vector2 ortho = new Vector2(-vector.y, vector.x);
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

    private async void StartHealing()
    {
        healParticules.SetActive(true);
        await Task.Delay(3000);
        if(personalty == Personalty.Heal && HealPoint < 5)
        {
            HealPoint++;
            HealPoints[HealPoint - 1].SetActive(true);
            if(HealPoint < 5)
            {
                HealPoint++;
                HealPoints[HealPoint - 1].SetActive(true);
            }
        }
        healParticules.SetActive(false);
    }

    private async void Parry()
    {
        parrying = true;
        await Task.Delay(1500);
        parrying = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsGrounded)
            hasDoubleJump = true;

        if (collision.gameObject.tag == "Enemy")
        {
            Damage(1);
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            if (parrying)
            {
                rb.AddForce(new Vector2(0, parryForce), ForceMode2D.Impulse);
                Destroy(collision.gameObject);
                parrying = false;
                animator.SetBool("isJumping", isJumping);
            }
            else
            {
                Destroy(collision.gameObject);
                Damage(1);
            }
        }
        else if (collision.gameObject.tag == "DeathZone")
            Death();
        if (HealPoint <= 0)
            Death();
    }

    public void Damage(int dmg)
    {
        for(int i = 0; i < dmg; i++)
        {
            HealPoints[HealPoint - 1].SetActive(false);
            HealPoint--;
        }
    }

    private void Death()
    {
        DeathScreen.SetActive(true);
        renderer.enabled = false;
        Time.timeScale = 0;
        foreach (GameObject hp in HealPoints)
            hp.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "CheckPoint")
        {
            checkPoint = transform.position;
            //Play a sound
        }
        else if (collision.tag == "Sign")
        {
            interactable = collision.GetComponent<Interactable>();
            Info.text = "Sign: Press B or enter to read.";
            Info.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Sign")
        {
            interactable = null;
            Info.gameObject.SetActive(false);
            Dialog.SetActive(false);
        }
    }

    private Vector3 GetDirection(bool useAimCorrection = true, bool fromPlayer = false)
    {
        if (useController)
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
        else if(!fromPlayer)
        {
            Vector3 direction = Input.mousePosition;
            direction.x -= Screen.width / 2;
            direction.y -= Screen.height / 2;

            if (Mathf.Abs(direction.y) < Screen.height / 10 && Mathf.Abs(direction.x) < Screen.width / 10)
            {
                print(Mathf.Abs(direction.y) + " " + Screen.height / 10);
                return new Vector3(0, 0, 0);
            }

            direction.Normalize();
            return direction;
        }
        else
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z)) - transform.position;
            return direction.normalized;
        }
    }
}

public enum Personalty { Normal, DoubleJump, Run, Hook, Parry, Heal, SlowProj }
