using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
  public enum Type
  {
      HEADGEAR,
      BODYGEAR,
      WEAPON,
      ITEM,
      CONSUMABLE
  }

  public static class Modifier 
  {
    public static string DAMAGE = "DAMAGE";
    public static string ATTACK_SPEED = "ATTACK_SPEED";
    public static string MOVE_SPEED = "MOVE_SPEED";
    public static string HEALTH = "HEALTH";
    public static string HEALTH_GIVE = "HEALTH_GIVE";
  }

  public static class Attribute 
  {
    public static string SOUND_PATH = "SOUND_PATH";
  }

  public string id;
  public string title;
  public string description;

  public Type type;

  public Sprite icon;
  public Dictionary<string, float[]> stats = new Dictionary<string, float[]>();
  public Dictionary<string, string> attributes = new Dictionary<string, string>();

  public ItemData(
      Type type, string id, string title, string description, Dictionary<string, float[]> stats, Dictionary<string, string> attributes) {
      this.id = id;
      this.type = type;
      this.title = title;
      this.description = description;
      this.icon = Resources.Load<Sprite>("Sprites/Items/" + id);
      this.stats = stats;
      this.attributes = attributes;
  }

  public ItemData(ItemData item) {
      this.id = item.id;
      this.type = item.type;
      this.title = item.title;
      this.description = item.description;
      this.icon = Resources.Load<Sprite>("Sprites/Items/" + item.id);
      this.stats = item.stats;
      this.attributes = item.attributes;
  }
}
