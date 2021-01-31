using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    SpriteRenderer sprite;
    public GameObject slash;
    public GameObject projectile;
    GameObject S;
    Transform SChild;
    SceneManager scene;
    public float angle;
    public Vector3 focus;
    public bool attacking = false;
    public bool playerWeapon = false;
    public bool ranged;
    public int ammo = 10;
    public bool held = false;
    GameObject home;

    void Start()
    {
        if(ranged) {
            home = Instantiate(new GameObject(name), transform.position, transform.rotation, scene.projectiles);
            for(int i = 0; i < ammo; i++) {
                GameObject shot = Instantiate(projectile, transform.position, transform.rotation, home.transform);
            }
        }
    }

    void OnEnable() {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if(transform.parent) {
            held = true;
            if(transform.parent.tag == "Player") playerWeapon = true;
        }
        
        if (held) {
            if(transform.parent.childCount < 2) S = Instantiate(slash, transform.position, transform.rotation, transform.parent);
            else S = transform.parent.Find("Slash").gameObject;
            S.GetComponent<DeleteSelf>().Disable();
            SChild = S.transform.GetChild(0);
            GetComponent<Pickup>().enabled = false;
        }else {
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Pickup>().enabled = true;
            this.enabled = false;
        }

        scene = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
    }
    void Update() {
        if (held) {
            if(playerWeapon) focus = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else focus = scene.player.position;

            angle = Mathf.Atan2(focus.y - transform.position.y, focus.x - transform.position.x) * Mathf.Rad2Deg - 40;

            if(angle > 10 && angle < 80) {
                transform.position = new Vector3(transform.position.x, transform.position.y, -3f);
            }else {
                transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
            }

            if(angle > 60 || angle < -135)
            {
                sprite.flipY = true;
                transform.rotation = Quaternion.Euler(0, 0, angle + 80);
            }else {
                sprite.flipY = false;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public void Attack(float time) {
        if(!attacking) {
            Invoke("Cooldown", time);
            if(ranged) {
                for(int i = 0; i < home.transform.childCount; i++) {
                    Transform shot = home.transform.GetChild(i);
                    if(!shot.gameObject.activeInHierarchy) {
                        shot.position = transform.position;
                        shot.rotation = Quaternion.Euler(0, 0, angle - 50);
                        shot.gameObject.SetActive(true);
                        attacking = true;
                        return;
                    }
                }
                Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, angle - 50), home.transform);
            }else {
                if (S) {
                    if(angle > 60 || angle < -135) {
                        S.transform.rotation = Quaternion.Euler(0, 0, angle + 20);
                        SChild.GetComponent<SpriteRenderer>().flipY = true;
                    }else {
                        S.transform.rotation = transform.rotation;
                        SChild.GetComponent<SpriteRenderer>().flipY = false;
                    }

                    S.GetComponent<DeleteSelf>().Kill(0.1f);
                }
            }
        }
        attacking = true;
    }

    void Cooldown() {
        attacking = false;
    }
}
