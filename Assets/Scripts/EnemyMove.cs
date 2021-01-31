using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;
    SpriteRenderer sprite;
    Weapon weapon;
    float horizontal;
    float vertical;
    public float moveLimiter = 0.7f;
    public float runSpeed = 1f;
    SceneManager scene;
    float distanceFromTraget;
    public float hitDistance = 0.5f;
    public float attackSpeed = 0.25f;
    public float avoidRange = 0.5f;
    public float sightRange = 1.5f;
    public LayerMask avoidList;
    public LayerMask obsticleList;
    public float health = 3f;
    private float maxHealth;
    public float knockbackResistance;
    Vector2 knockback;
    private PlayerInventory inventory;
    ParticleSystem particle;

    void Start ()
    {
        maxHealth = health;
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        weapon = transform.GetChild(0).GetComponent<Weapon>();
        scene = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
        inventory = scene.player.GetComponent<PlayerInventory>();
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        Vector3 focus = scene.player.position;

        float angle = Mathf.Atan2(focus.y - transform.position.y, focus.x - transform.position.x) * Mathf.Rad2Deg - 40;

        if(angle > 10 && angle < 80) {
            anim.SetBool("Back", true);
            anim.SetBool("Front", false);
        }else if(angle > -165 && angle < -95) {
            anim.SetBool("Front", true);
            anim.SetBool("Back", false);
        } else {
            anim.SetBool("Front", false);
            anim.SetBool("Back", false);
        }

        if (angle > 60 || angle < -135) {
            sprite.flipX = true;
        } else {
            sprite.flipX = false;
        }

        if(distanceFromTraget <= hitDistance) {
            weapon.Attack(attackSpeed);
        }
    }

    void FixedUpdate()
    {
        distanceFromTraget = Vector3.Distance(transform.position, scene.player.position);
        RaycastHit2D canSee = Physics2D.Raycast(transform.position, (scene.player.position-transform.position).normalized, sightRange, obsticleList);

        //if (canSee) Debug.DrawLine(transform.position, canSee.transform.position, Color.yellow, 1f);

        if (distanceFromTraget > hitDistance && canSee && canSee.transform.tag == "Player") {
            Vector3 dir = (scene.player.position-transform.position).normalized;
            horizontal = dir.x;
            vertical = dir.y;

            anim.SetBool("Walking", true);
        }else if (distanceFromTraget < hitDistance - 0.1f && canSee && canSee.transform.tag == "Player") {
            Vector3 dir = (transform.position-scene.player.position).normalized;
            horizontal = dir.x;
            vertical = dir.y;

            anim.SetBool("Walking", true);
        } else {
            horizontal = 0;
            vertical = 0;

            anim.SetBool("Walking", false);
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, avoidRange, avoidList);
        Vector3 avoidDirection = new Vector3(0, 0, 0);
        for(int i = 0; i < targets.Length; i++) {
            float distance = Vector3.Distance(targets[i].transform.position, transform.position);
            if(distance > 0.01f) avoidDirection += (targets[i].transform.position / (distance));
        }
        avoidDirection /= targets.Length;
        if (avoidDirection != Vector3.zero) {
            Vector3 dir = (transform.position - avoidDirection).normalized;
            horizontal += dir.x;
            vertical += dir.y;
        }

        body.velocity = (body.velocity + new Vector2(horizontal * runSpeed, vertical * runSpeed))/2 - knockback;
        knockback = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != tag) {
            if(col.tag == "Player"){
                particle.Play();
                health -= GetDamage();
                knockback = (col.transform.position-transform.position) * Mathf.Clamp(100-knockbackResistance, 0, 200);
                if(health <= 0) {
                    transform.parent = transform.parent.GetChild(0);
                    gameObject.SetActive(false);
                }
            }
        }
    }

        public float GetDamage() {
      float fMod = 0f;
      float pMod = 1f;

      for (int i = 0; i < 4; i++) {
        ItemData item = inventory.equips[i];

        if (item != null) {
          if (item.stats.ContainsKey(ItemData.Modifier.DAMAGE)) {
            float[] modifier = item.stats[ItemData.Modifier.DAMAGE];
            
            if (modifier != null) {
              fMod += modifier[0];
              pMod += modifier[1];
            }
          }
        }
      }

      return (0.5f + fMod) * pMod;
    }

    public float GetHealthPercentage() {
      return health / maxHealth;
    }
}
