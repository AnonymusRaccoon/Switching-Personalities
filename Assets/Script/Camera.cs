using UnityEngine;

public class Camera : MonoBehaviour
{
    public bool FollowY = true;
    public float offsetY;
    private GameObject player;

    void Start ()
    {
        player = GameObject.Find("Player");
	}
	
	void Update ()
    {
        transform.position = new Vector3(player.transform.position.x, ((FollowY && Mathf.Abs(transform.position.y - player.transform.position.y) > 3) ? Mathf.Lerp(transform.position.y, player.transform.position.y + offsetY, 0.02f) : transform.position.y), transform.position.z);
	}
}
