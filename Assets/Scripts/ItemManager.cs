using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
  public List<ItemData> items;

  public ItemData currentDraggedItem;
  public int currentDraggedItemSlot;
  public string currentDraggedItemType;

  public GameObject equippedWeapon;
  public Transform itemTooltip;

  void Awake()
  {
    BuildItems();
    itemTooltip = GameObject.Find("ItemTooltip").transform;
    itemTooltip.gameObject.SetActive(false);
  }

  void BuildItems()
  {
    items = new List<ItemData>() {
      new ItemData(
        ItemData.Type.CONSUMABLE, 
        "cheese", 
        "Cheese", 
        "It stinks... But it's better than nothing.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.HEALTH_GIVE, new float[2] { 0.0f, .25f } }
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.CONSUMABLE, 
        "rotten_egg", 
        "Rotten Egg", 
        "DO NOT EAT THIS! Don't say I didn't warn you...", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.HEALTH_GIVE, new float[2] { -100.0f, 0f } }
        },
        new Dictionary<string, string>()),
        
      new ItemData(
        ItemData.Type.HEADGEAR, 
        "magic_hat", 
        "Magic Hat", 
        "It makes you able to fly! (on the ground)", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, .5f } },
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.HEADGEAR, 
        "trash_lid", 
        "Trash Lid", 
        "Who's to say that this isn't a hat! Protects your noggin.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.15f } },
          { ItemData.Modifier.HEALTH, new float[2] { 0.0f, .25f } }
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.BODYGEAR, 
        "cardboard_box", 
        "Cardboard Box", 
        "It's better than being naked!", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.1f } },
          { ItemData.Modifier.HEALTH, new float[2] { 0.0f, .25f } }
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.BODYGEAR, 
        "old_coat", 
        "Old Coat", 
        "Probably belonged to a grandma once! Keeps you warm and well protected.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.15f } },
          { ItemData.Modifier.HEALTH, new float[2] { 0.0f, .5f } },
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.BODYGEAR, 
        "sweater", 
        "Ugly Christmas sweater", 
        "Why was this in the trash!? It's awesome!", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, .15f } },
          { ItemData.Modifier.HEALTH, new float[2] { 0.0f, .15f } },
        },
        new Dictionary<string, string>()),

      new ItemData(
        ItemData.Type.WEAPON, 
        "chair_leg", 
        "Broken Chair Leg",  
        "Probably from a broken chair... An average weapon at best.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.DAMAGE, new float[2] { .5f, 0.0f } },
          { ItemData.Modifier.ATTACK_SPEED, new float[2] { 0.5f, 0.0f } },
        },
        new Dictionary<string, string>() {
          { "HIT_SOUND_1", "Sounds/ChairBonk/Explosion_02" },
          { "HIT_SOUND_2", "Sounds/ChairBonk/Explosion_03" },
          { "HIT_SOUND_3", "Sounds/ChairBonk/Explosion_04" }
        }),

      new ItemData(
        ItemData.Type.WEAPON, 
        "bow", 
        "Bow",  
        "Shoot arrows at a fast pace, dealing fabulous damage.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.DAMAGE, new float[2] { .7f, 0.0f } },
          { ItemData.Modifier.ATTACK_SPEED, new float[2] { .3f, 0.0f } },
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.3f } },
        },
        new Dictionary<string, string>() {
          { "HIT_SOUND_1", "Sounds/BowShot/Menu_Navigate_03" }
        }),

      new ItemData(
        ItemData.Type.WEAPON, 
        "magic_wand", 
        "Magic Chicken Staff",  
        "Shoots flaming chicken legs! Slow to attack, slow to move, but you deal a lot of damage.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.DAMAGE, new float[2] { 2.0f, 0.0f } },
          { ItemData.Modifier.ATTACK_SPEED, new float[2] { 1.25f, 0.0f } },
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.5f } },
        },
        new Dictionary<string, string>() {
          { "HIT_SOUND_1", "Sounds/MagicWandHit/Shoot_00" },
          { "HIT_SOUND_2", "Sounds/MagicWandHit/Shoot_01" },
          { "HIT_SOUND_3", "Sounds/MagicWandHit/Shoot_02" },
          { "HIT_SOUND_4", "Sounds/MagicWandHit/Shoot_03" }
        }),

      new ItemData(
        ItemData.Type.WEAPON, 
        "sling", 
        "Sling",  
        "Shoots small bits of rocks! Seems harmless until you get hit by one.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.DAMAGE, new float[2] { .45f, 0.0f } },
          { ItemData.Modifier.ATTACK_SPEED, new float[2] { .3f, 0.0f } },
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.3f } },
        },
        new Dictionary<string, string>() {
          { "HIT_SOUND_1", "Sounds/Sling/Hit_00" }
        }),

      new ItemData(
        ItemData.Type.WEAPON, 
        "bottle", 
        "Bottle",  
        "Throw bottles at people.. probably won't shatter though. Does average damage.", 
        new Dictionary<string, float[]>() {
          { ItemData.Modifier.DAMAGE, new float[2] { .25f, 0.0f } },
          { ItemData.Modifier.ATTACK_SPEED, new float[2] { .4f, 0.0f } },
          { ItemData.Modifier.MOVE_SPEED, new float[2] { 0.0f, -.1f } },
        },
        new Dictionary<string, string>() {
          { "HIT_SOUND_1", "Sounds/Bottle/Pickup_00" },
          { "HIT_SOUND_2", "Sounds/Bottle/Pickup_04" }
        }),
    };
  }

  public ItemData GetItem(string id) {
    return items.Find(item => item.id == id);
  }
}
