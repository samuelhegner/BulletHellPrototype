using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float maxSpeed = 2f;

    public float damping;

    bool charging;
    bool shootCooldown;
    bool dashing;


    public float maxChargeTime;
    public float cooldownTime;
    public float maxBulletScale;
    public float dashDuration;
    public float dashSpeed;
    public float force;


    public LineRenderer line;

    public float minWidth;
    public float maxWidth;


    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    public List<Collider> cols = new List<Collider>();

    public Renderer[] rends;



    void Start()
    {
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders) {
            if (col.transform.parent != null) {
                cols.Add(col);
            }
        }

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float hAxis = Input.GetAxis("HorizontalLeftStick");
        float vAxis = Input.GetAxis("VerticalLeftStick");

        Vector3 moveDir = new Vector3(hAxis, 0, vAxis);

        if(!dashing)
        rb.AddForce(moveDir * force * Time.deltaTime, ForceMode.VelocityChange);

        if (!charging)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        else {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed/4f);
        }
        

        rb.velocity *= (1.0f - (damping * Time.deltaTime));

        if (Input.GetAxis("Axis 10") > 0f && !charging && !shootCooldown)
        {
            StartCoroutine(ChargeShot());
        }

        if (Input.GetButtonDown("Dash") && !charging && !dashing) {
            StartCoroutine(Dash());
        }
        

        if (!charging)
        {
            if (rb.velocity != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
            }
        }
        else {
            Vector3 rightStick = new Vector3(Input.GetAxis("HorizontalRightStick"), 0, Input.GetAxis("VerticalRightStick"));
            if (rightStick != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(rightStick, Vector3.up);
            }
        }
    }

    IEnumerator ChargeShot() {
        charging = true;

        line.startWidth = minWidth;
        line.endWidth = minWidth;
        line.enabled = true;


        float charge = 0;

        GameObject newBullet = Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        newBullet.GetComponent<Bullet>().enabled = false;
        float scaleModifier = 1;

        Vector3 startScale = newBullet.transform.localScale;

        foreach (Renderer rend in rends)
        {
            rend.material.color = Color.blue;
        }

        while (Input.GetAxis("Axis 10") > 0f && charge < maxChargeTime) {
            charge += maxChargeTime / (1f / Time.deltaTime) * (1f / maxChargeTime);

            newBullet.transform.position = bulletSpawnPoint.transform.position;
            newBullet.transform.rotation = bulletSpawnPoint.transform.rotation;

            line.startWidth = GameManager.Map(charge, 0, maxChargeTime, minWidth, maxWidth);
            line.endWidth = GameManager.Map(charge, 0, maxChargeTime, minWidth, maxWidth);

            scaleModifier = GameManager.Map(charge, 0, maxChargeTime, 1, maxBulletScale);
            newBullet.transform.localScale = startScale * scaleModifier;
            yield return new WaitForEndOfFrame();
        }

        foreach (Renderer rend in rends)
        {
            rend.material.color = Color.white;
        }

        newBullet.GetComponent<Bullet>().enabled = true;


        charging = false;
        shootCooldown = true;
        StartCoroutine(CoolDown());
        line.enabled = false;
    }

    IEnumerator CoolDown() {
        float cooldownAmount = cooldownTime;

        while (cooldownAmount > 0) {
            cooldownAmount -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shootCooldown = false;
    }

    IEnumerator Dash() {
        dashing = true;

        float dashTime = dashDuration;

        

        foreach (Renderer rend in rends) {
            rend.material.color = Color.red;
        }

        while (dashTime > 0) {
            dashTime -= Time.deltaTime;
            rb.AddForce(transform.forward * dashSpeed * Time.deltaTime, ForceMode.Impulse);
            yield return new WaitForEndOfFrame();
        }


        foreach (Renderer rend in rends)
        {
            rend.material.color = Color.white;
        }
        dashing = false;
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
