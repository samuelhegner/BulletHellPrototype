using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float maxSpeed = 2f;

    public float damping;

    bool charging;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float hAxis = Input.GetAxis("HorizontalLeftStick");
        float vAxis = Input.GetAxis("VerticalLeftStick");

        Vector3 moveDir = new Vector3(hAxis, 0, vAxis);

        rb.AddForce(moveDir, ForceMode.VelocityChange);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        rb.velocity = rb.velocity *= (1.0f - (damping * Time.deltaTime));

        if (Input.GetAxis("Axis 10") > 0f)
        {
            charging = true;
        }
        else {
            charging = false;
        }

        if (!charging)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        }
        else {
            transform.rotation = Quaternion.LookRotation(new Vector3(Input.GetAxis("HorizontalRightStick"), 0, Input.GetAxis("VerticalRightStick")), Vector3.up);
        }
    }
}
