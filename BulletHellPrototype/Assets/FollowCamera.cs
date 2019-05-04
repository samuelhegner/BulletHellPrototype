using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;

    Vector3 offsetPosition;

    public float camSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        offsetPosition = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offsetPosition, Time.deltaTime * camSpeed);
        transform.LookAt(player.transform);
    }
}
