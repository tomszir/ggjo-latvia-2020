using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public ItemData[] items;
    public GameObject[] inWorldPrefabs;
    public ItemData[] equips;

    public GameObject inventoryItemPrefab;

    public static Dictionary<string, int> equipSlotNames = new Dictionary<string, int>{
      { "headgear",         0 },
      { "bodygear",         1 },
      { "weapon",           2 },
      { "secondary_weapon", 3 },
    };

    private ItemManager itemManager;

    private GameObject itemGrid;
    private GameObject equipGrid;

    void Start()
    {
      itemGrid = GameObject.Find("ItemGrid");
      equipGrid = GameObject.Find("EquipGrid");

      itemManager = GameObject
        .FindGameObjectWithTag("GameController")
        .GetComponent<ItemManager>();

      items  = new ItemData[6];
      inWorldPrefabs  = new GameObject[6];
      equips = new ItemData[4];
    }

    public ItemData GetItem(int slot) 
    {
      return items[slot];
    }

    public bool HasSpace() { 
      for (int i = 0; i < 6; i++) {
          if (items[i] == null) {
            return true;
          } 
      }

      return false;
    }

    public void AddItem(ItemData item, GameObject real) {
      for (int i = 0; i < 6; i++) {
          if (items[i] == null) {
            items[i] = item;
            inWorldPrefabs[i] = real;
            break;
          }
      }
    }

    public void Equip(string slot, ItemData item) 
    {
      if (equipSlotNames[slot] != null) {
        Equip(equipSlotNames[slot], item);
      }
    }

    public void Equip(int slot, ItemData item) 
    {
      equips[slot] = item;
    }

    void Unequip(int equipSlot) 
    {

    }
}
