using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float timeTillDestroy;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyBullet", timeTillDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void DestroyBullet() {
        Destroy(gameObject);
    }
}
