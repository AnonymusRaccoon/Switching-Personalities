using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingCloud : MonoBehaviour
{
    private Rigidbody2D rb;
    private new SpriteRenderer renderer;
    private Vector3 initialPosition;

	private void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
	}

    private async void OnCollisionEnter2D(Collision2D collision)
    {
        await Task.Delay(1500);
        rb.isKinematic = false;
        await Task.Delay(2000);
        renderer.enabled = false;
        await Task.Delay(1500);
        rb.isKinematic = true;
        transform.position = initialPosition;
        renderer.enabled = true;
    }
}
