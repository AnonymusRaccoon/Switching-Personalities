using UnityEngine;

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
}
