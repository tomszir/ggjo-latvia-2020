using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Rigidbody2D body;
    public float despawnTime = 1f;
    DeleteSelf ds;
    Collider2D collider;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        ds = GetComponent<DeleteSelf>();
        collider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        if(body) {
            body.velocity = transform.up * speed;
            collider.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != tag) {
            if(col.tag == "Enemy" || col.tag == "Wall"){
                ds.Kill(despawnTime);
                body.velocity = Vector3.zero;
                collider.enabled = false;
            }
        }
    }
}
