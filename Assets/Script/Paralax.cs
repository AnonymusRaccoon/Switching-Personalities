using UnityEngine;

public class Paralax : MonoBehaviour
{
    public float speed = 1;
    private GameObject player;
    private new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        renderer.material.mainTextureOffset = new Vector2(speed * player.transform.position.x * 0.1f, 0);
    }
}
