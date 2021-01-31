using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;
    SpriteRenderer sprite;
    public Weapon weapon;
    private float horizontal;
    private float vertical;
    private Vector2 knockback;
    public float moveLimiter = 0.7f;
    public float runSpeed = 1f;
    public float attackSpeed = 0.25f;
    public float health = 3f;
    public float maxHealth;
    public float knockbackResistance = 30f;
    SceneManager scene;
    AudioSource audio;
    public AudioClip[] clips;
    ParticleSystem particle;

    private PlayerInventory inventory;

    void Start ()
    {
        maxHealth = health;
        inventory = GetComponent<PlayerInventory>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        weapon = transform.GetChild(0).GetComponent<Weapon>();
        scene = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
        audio = GetComponent<AudioSource>();
        clips = new AudioClip[4];
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Movement values
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);

        float angle = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg - 40;

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

        if(angle > 60 || angle < -135) {
            sprite.flipX = true;
        } else {
            sprite.flipX = false;
        }

        if(Input.GetButtonDown("Primary Attack")) {
            if(!weapon.attacking) {
                clips = new AudioClip[4];
                int clipCount = 0;
                if (inventory.equips[2] != null && inventory.equips[2].attributes.ContainsKey("HIT_SOUND_1")) {
                    clips[clipCount] = Resources.Load<AudioClip>(inventory.equips[2].attributes["HIT_SOUND_1"]);
                    clipCount++;
                }
                if (inventory.equips[2] != null && inventory.equips[2].attributes.ContainsKey("HIT_SOUND_2")) {
                    clips[clipCount] = Resources.Load<AudioClip>(inventory.equips[2].attributes["HIT_SOUND_2"]);
                    clipCount++;
                }
                if (inventory.equips[2] != null && inventory.equips[2].attributes.ContainsKey("HIT_SOUND_3")) {
                    clips[clipCount] = Resources.Load<AudioClip>(inventory.equips[2].attributes["HIT_SOUND_3"]);
                    clipCount++;
                }
                if (inventory.equips[2] != null && inventory.equips[2].attributes.ContainsKey("HIT_SOUND_4")) {
                    clips[clipCount] = Resources.Load<AudioClip>(inventory.equips[2].attributes["HIT_SOUND_4"]);
                    clipCount++;
                }
                if(clipCount > 0) audio.PlayOneShot(clips[Random.Range(0, clipCount)]);
            }
            weapon.Attack(GetAttackSpeed());
        }
    }

    void FixedUpdate()
    {
        if (horizontal != 0 || vertical != 0) {
            anim.SetBool("Walking", true);

            if (horizontal != 0 && vertical != 0) {
                // limit movement speed diagonally
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }
        }else {
            anim.SetBool("Walking", false);
        }

        body.velocity = (body.velocity + new Vector2(horizontal * GetRunSpeed(), vertical * GetRunSpeed()))/2 - knockback;
        knockback = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != tag) {
            if(col.tag == "Enemy"){
                particle.Play();
                audio.Play();
                health -= .5f;
                knockback = (col.transform.position-transform.position) * Mathf.Clamp(100-knockbackResistance, 0, 200);
                if(health <= 0) {
                    scene.Died();
                    gameObject.SetActive(false);
                }
            }else if(col.tag == "Item") {
                col.GetComponent<Pickup>().Take(transform);
            }
        }
    }

    public float GetAttackSpeed() {
      float fMod = 0f;
      float pMod = 1f;

      for (int i = 0; i < 4; i++) {
        ItemData item = inventory.equips[i];

        if (item != null) {
          if (item.stats.ContainsKey(ItemData.Modifier.ATTACK_SPEED)) {
            float[] modifier = item.stats[ItemData.Modifier.ATTACK_SPEED];
            
            if (modifier != null) {
              fMod += modifier[0];
              pMod += modifier[1];
            }
          }
        }
      }

      return (attackSpeed + fMod) * pMod;
    }

    public float GetRunSpeed() {
      float fMod = 0f;
      float pMod = 1f;

      for (int i = 0; i < 4; i++) {
        ItemData item = inventory.equips[i];

        if (item != null) {
          if (item.stats.ContainsKey(ItemData.Modifier.MOVE_SPEED)) {
            float[] modifier = item.stats[ItemData.Modifier.MOVE_SPEED];
            
            if (modifier != null) {
              fMod += modifier[0];
              pMod += modifier[1];
            }
          }
        }
      }

      return (runSpeed + fMod) * pMod;
    }

    public float GetHealthPercentage() {
      return health / maxHealth;
    }
}
