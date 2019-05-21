using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float maxSpeed = 2f;

    public float damping;

    bool charging;
    bool cooldown;

    public float maxChargeTime;
    public float cooldownTime;
    public float maxBulletScale;


    public GameObject bullet;
    public GameObject bulletSpawnPoint;

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

        if (Input.GetAxis("Axis 10") > 0f && !charging && !cooldown)
        {
            StartCoroutine(ChargeShot());
        }
        

        if (!charging)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        }
        else {
            transform.rotation = Quaternion.LookRotation(new Vector3(Input.GetAxis("HorizontalRightStick"), 0, Input.GetAxis("VerticalRightStick")), Vector3.up);
        }
    }

    IEnumerator ChargeShot() {
        charging = true;

        float charge = 0;

        GameObject newBullet = Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        newBullet.GetComponent<Bullet>().enabled = false;
        float scaleModifier = 1;

        Vector3 startScale = newBullet.transform.localScale;

        

        while (Input.GetAxis("Axis 10") > 0f && charge < maxChargeTime) {
            charge += maxChargeTime / (1f / Time.deltaTime) * (1f / maxChargeTime);

            newBullet.transform.position = bulletSpawnPoint.transform.position;
            newBullet.transform.rotation = bulletSpawnPoint.transform.rotation;

            scaleModifier = GameManager.Map(charge, 0, maxChargeTime, 1, maxBulletScale);
            newBullet.transform.localScale = startScale * scaleModifier;
            yield return new WaitForEndOfFrame();
        }



        newBullet.GetComponent<Bullet>().enabled = true;


        charging = false;
        cooldown = true;
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown() {
        float cooldownAmount = cooldownTime;

        while (cooldownAmount > 0) {
            cooldownAmount -= Time.deltaTime;
            print(cooldownAmount);
            yield return new WaitForEndOfFrame();
        }

        cooldown = false;
        print(cooldown);
    }
}

public class GameManager {
    public static float Map(float a, float b, float c, float d, float e)
    {
        float cb = c - b;
        float de = e - d;
        float howFar = (a - b) / cb;
        return d + howFar * de;
    }
}
