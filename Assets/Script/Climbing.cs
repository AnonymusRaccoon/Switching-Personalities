using UnityEngine;

public class Climbing : MonoBehaviour
{
    public float height = 10;
    public float speed = 2;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, height * Mathf.Sin(Time.time * speed), transform.position.y);
    }
}
