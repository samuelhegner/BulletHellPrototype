using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    StateMachine stateMachine;

    public GameObject[] guns;

    public GameObject[] arms;

    public GameObject body;

    public GameObject bullet;

    public GameObject player;



    public bool shoot;
    public bool spin;
    public bool spray;
    public bool ready;

    public float rotSpeed;

    public Image img;

    Vector3 imgScaleStart;

    public int count = 0;


    // Start is called before the first frame update
    void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        stateMachine.ChangeState(new SprayAndPray());

        ready = false;

        imgScaleStart = img.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shoot(int shotsPerSecond, float bulletSpeed) {
        while (shoot) {

            foreach (GameObject gun in guns) {
                GameObject nBullet = Instantiate(bullet, gun.transform.position, gun.transform.rotation);
                nBullet.GetComponent<Bullet>().speed = bulletSpeed;
            }

            yield return new WaitForSeconds(1.0f / shotsPerSecond);
        }
    }

    public IEnumerator Spin() {

        for (int i = 0; i < arms.Length; i++) {
            if (i == 0)
            {
                arms[i].transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
            }
            else {
                arms[i].transform.rotation = Quaternion.Euler(new Vector3(0, -90f, 0));
            }
        }


        int ran = Random.Range(0, 2);

        if (ran == 0) {
            ran = -1;
        }

        while (spin) {
            body.transform.Rotate(new Vector3(0, Time.deltaTime * ran * rotSpeed, 0));
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < arms.Length; i++)
        {
            if (i == 0)
            {
                arms[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                arms[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        while (body.transform.rotation != Quaternion.Euler(Vector3.zero)) {
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, transform.rotation, Time.deltaTime * 20f);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up), Time.deltaTime * 10f);

            Vector3 dist1 = body.transform.rotation.eulerAngles - transform.rotation.eulerAngles;
            

            if (Quaternion.Angle(body.transform.rotation, transform.rotation) < 1f) {
                body.transform.rotation = transform.rotation;
                break;
            } 

            yield return new WaitForEndOfFrame();
        }

        print("Spin done");
    }


    public IEnumerator SprayAndPray() {



        for (int i = 0; i < arms.Length; i++)
        {
            if (i == 0)
            {
                arms[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                arms[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        while (spray) {

            arms[0].transform.localRotation = Quaternion.Euler(arms[0].transform.rotation.eulerAngles.x,  50f * Mathf.Sin(Time.time * 2f), arms[0].transform.rotation.eulerAngles.z);

            arms[1].transform.localRotation = Quaternion.Euler(arms[1].transform.rotation.eulerAngles.x, -50f * Mathf.Sin(Time.time * 2f), arms[1].transform.rotation.eulerAngles.z);


            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up), Time.deltaTime * 6f);

            //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator WeakSpot(float weaknessTime) {
        ready = false;

        float weakTime = weaknessTime;

        img.gameObject.SetActive(true);

        while (weakTime > 0) {

            img.transform.localScale -= imgScaleStart / (1 / Time.deltaTime) * (1 / weaknessTime);

            weakTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        img.gameObject.SetActive(false);

        img.transform.localScale = imgScaleStart;

        ready = true;
    }
}

class WeakState : State
{ 

    Boss boss;

    public override void Enter()
    {
        boss = owner.GetComponent<Boss>();

        boss.StartCoroutine(boss.WeakSpot(3f));
        boss.count = 0;
    }

    public override void Exit()
    {
        
    }

    public override void Think()
    {

        if (boss.ready) {
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                owner.ChangeState(new SprayAndPray());
            }
            else
            {
                owner.ChangeState(new SpinShot());
            }
        }
        
    }
}

class SprayAndPray : State
{
    Boss boss;

    float timeToSprayAndPray = 10f;

    float startTime;

    Coroutine coroutine;

    public override void Enter()
    {
        boss = owner.GetComponent<Boss>();
        boss.shoot = true;
        boss.spray = true;

        boss.StartCoroutine(boss.SprayAndPray());
        coroutine = boss.StartCoroutine(boss.Shoot(40, 10));

        startTime = Time.time;
    }

    public override void Exit()
    {

        boss.StopCoroutine(coroutine);
        boss.shoot = false;
        boss.spray = false;

        boss.count++;
    }

    public override void Think()
    {
        if (Time.time - startTime > timeToSprayAndPray)
        {
            if (boss.count > 0)
            {
                owner.ChangeState(new WeakState());
            }
            else {
                owner.ChangeState(new SpinShot());
            }
        }
    }
}

class SpinShot : State
{
    Boss boss;

    float timeToSpin = 10f;

    float startTime;

    Coroutine coroutine;

    public override void Enter()
    {
        boss = owner.GetComponent<Boss>();
        boss.shoot = true;
        boss.spin = true;
        coroutine = boss.StartCoroutine(boss.Shoot(35, 25));
        boss.StartCoroutine(boss.Spin());
        startTime = Time.time;
    }

    public override void Think()
    {
        if (Time.time - startTime > timeToSpin) {
            if (boss.count > 0)
            {
                owner.ChangeState(new WeakState());
            }
            else
            {
                owner.ChangeState(new SprayAndPray());
            }
        }
    }

    public override void Exit()
    {
        boss.StopCoroutine(coroutine);
        boss.shoot = false;
        boss.spin = false;

        boss.count++;
    }

}