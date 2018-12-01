using UnityEngine;

public class Camera : MonoBehaviour
{
    private GameObject player;

    void Start ()
    {
        player = GameObject.Find("Player");
	}
	
	void Update ()
    {
        transform.position = new Vector3(player.transform.position.x, (Mathf.Abs(transform.position.y - player.transform.position.y) > 3 ? Mathf.Lerp(transform.position.y, player.transform.position.y, 0.2f) : transform.position.y), transform.position.z);
	}
}
