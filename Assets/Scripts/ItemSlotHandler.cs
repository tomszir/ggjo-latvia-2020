using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData activeItem;
    public string hotkey;

    private Image itemImage;
    private Transform spriteObject;
    private Transform hotkeyObject;

    private RectTransform rect;

    private Vector2 initialPos;
    private GameObject draggedItem;
    
    private Transform player;
    private ItemManager itemManager;
    private PlayerInventory inventory;

    public GameObject weaponPrefab;
    public GameObject bowProjectile;
    public GameObject magicProjectile;
    public GameObject bottleProjectile;
    public GameObject slingProjectile;

    private Transform itemTooltip;

    void Start() {
      spriteObject = transform.GetChild(0);
      hotkeyObject = transform.GetChild(1);

      itemManager = GameObject
        .FindGameObjectWithTag("GameController")
        .GetComponent<ItemManager>();
      itemTooltip = itemManager.itemTooltip;
      player = GameObject.FindGameObjectWithTag("Player").transform;
      inventory = player.GetComponent<PlayerInventory>();

      itemImage = spriteObject.GetComponent<Image>();
      rect = spriteObject.GetComponent<RectTransform>();

      if (hotkey != "") {
        hotkeyObject.GetChild(0).GetComponent<TextMeshProUGUI>().text = hotkey;
      } else {
        hotkeyObject.gameObject.SetActive(false);
      }
    }

    void Update() {
      if (transform.parent.name == "ItemGrid") {
        activeItem = inventory.items[transform.GetSiblingIndex()];
      } else {
        activeItem = inventory.equips[transform.GetSiblingIndex()];
      }

        if (transform.Find("Placeholder")) {
          if (activeItem != null) {
              transform.Find("Placeholder").gameObject.SetActive(false);
          } else {
              transform.Find("Placeholder").gameObject.SetActive(true);
          }
        }

      itemImage.sprite = activeItem != null ? activeItem.icon : null;

      if (hotkey != "") {
        if (Input.GetKeyDown(hotkey)) {
          if (activeItem != null) {
            if (activeItem.type == ItemData.Type.CONSUMABLE) {
              if (activeItem.stats.ContainsKey(ItemData.Modifier.HEALTH_GIVE)) {
                float[] modifier = activeItem.stats[ItemData.Modifier.HEALTH_GIVE];
                PlayerMove playerMove = player.GetComponent<PlayerMove>();

                playerMove.health = Mathf.Max(1, Mathf.Min(playerMove.maxHealth, playerMove.health + playerMove.maxHealth * modifier[1] + modifier[0]));
                inventory.items[transform.GetSiblingIndex()] = null;
              }
            }  
          }
        }
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (activeItem == null) {
        return;
      }

      if (itemManager.currentDraggedItem != null) {
        return;
      }

      draggedItem = new GameObject("DraggedItemIcon");

      itemManager.currentDraggedItem = activeItem;
      itemManager.currentDraggedItemSlot = transform.GetSiblingIndex();
      itemManager.currentDraggedItemType = transform.parent.name;

      draggedItem.transform.SetParent(GameObject.Find("Canvas").transform, false);
      draggedItem.transform.SetAsLastSibling();

      draggedItem.AddComponent<CanvasGroup>();
      draggedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;

      Image image = draggedItem.AddComponent<Image>();
      image.sprite = itemImage.sprite;
      image.rectTransform.sizeDelta = itemImage.rectTransform.sizeDelta;

      Color tempColor = image.color;
      tempColor.a = 0.8f;
      image.color = tempColor;
    }

    public void OnDrag(PointerEventData eventData) 
    { 
      if (activeItem == null) {
        return;
      }

      if (draggedItem) {
        Vector2 currentMousePosition = eventData.position;
        draggedItem.GetComponent<RectTransform>().position = currentMousePosition;
      }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
      if (draggedItem) {
        spriteObject.SetParent(transform);
        Destroy(draggedItem);
        draggedItem = null;
      }

      if (itemManager.currentDraggedItem != null) {
        Vector3 newDropPos = new Vector3(player.position.x, player.position.y + 0.25f, -5f);
        GameObject droppedItem = Instantiate(weaponPrefab, newDropPos, Quaternion.identity);

        droppedItem.SetActive(true);
        droppedItem.GetComponent<Pickup>().itemId = activeItem.id;

        if (itemManager.currentDraggedItemType == "ItemGrid") {
          inventory.items[itemManager.currentDraggedItemSlot] = null;
        }

        itemManager.currentDraggedItem = null;
        itemManager.currentDraggedItemType = null;
      }
    }

     public void OnDrop(PointerEventData data)
     {  
        ItemData dragItem = itemManager.currentDraggedItem;
        int dragSlot = itemManager.currentDraggedItemSlot;
        string dragType = itemManager.currentDraggedItemType;

        if (dragItem == null) return;

        itemManager.currentDraggedItem = null;
        itemManager.currentDraggedItemType = null;

        if (data.pointerDrag != null) {
            string type = transform.parent.name;
            int slot = transform.GetSiblingIndex();
        
            // The item is getting dropped into an item slot
            if (type == "ItemGrid") {
              ItemData itemInSlot = inventory.items[slot];

              if (itemInSlot == null) {
                if (dragType == "ItemGrid") {
                  inventory.items[dragSlot] = null;
                } else {
                  if (dragItem.type == ItemData.Type.WEAPON) {
                    if (slot == 3) {
                      OnPrimaryWeaponUnequip();
                    }
                    if (slot == 2) {
                      OnSecondaryWeaponUnequip();
                    }
                  }
                  inventory.equips[dragSlot] = null;
                }
              } else {
                if (dragType == "ItemGrid") {
                  inventory.items[dragSlot] = itemInSlot;
                } else {
                  if (itemInSlot.type != dragItem.type) {
                    return;
                  }
                  
                  if (itemInSlot.type == ItemData.Type.WEAPON) {
                    if (slot == 3) {
                      OnPrimaryWeaponEquip(itemInSlot);
                    }
                    if (slot == 2) {
                      OnSecondaryWeaponEquip(itemInSlot);
                    }
                  }

                  inventory.equips[dragSlot] = itemInSlot;
                }
              }

              inventory.items[slot] = dragItem;
            } 
            // The item is getting dropped into an equippable slot
            else {
              ItemData itemInSlot = inventory.equips[slot];

              if (dragItem.type != ItemData.Type.HEADGEAR &&
                  dragItem.type != ItemData.Type.BODYGEAR && 
                  dragItem.type != ItemData.Type.WEAPON) {
                return;
              }

              if (dragItem.type == ItemData.Type.HEADGEAR && slot != 0) {
                return;
              }

              if (dragItem.type == ItemData.Type.BODYGEAR && slot != 1) {
                return;
              }

              if (dragItem.type == ItemData.Type.WEAPON && (slot != 2 && slot != 3)) {
                return;
              }

              if (itemInSlot == null) {
                if (dragType == "ItemGrid") {
                  inventory.items[dragSlot] = null;
                } else {
                  if (dragItem.type == ItemData.Type.WEAPON) {
                    if (slot == 3) {
                      OnPrimaryWeaponUnequip();
                    }
                    if (slot == 2) {
                      OnSecondaryWeaponUnequip();
                    }
                  }
                  inventory.equips[dragSlot] = null;
                }
              } else {  
                if (itemInSlot.type != dragItem.type) {
                  return;
                }

                if (dragType == "ItemGrid") {
                  inventory.items[dragSlot] = itemInSlot;
                } else {
                  if (itemInSlot.type == ItemData.Type.WEAPON) {
                    if (dragSlot == 2) {
                      OnPrimaryWeaponUnequip();
                      OnPrimaryWeaponEquip(itemInSlot);
                    }
                    if (dragSlot == 3) {
                      OnSecondaryWeaponUnequip();
                      OnSecondaryWeaponEquip(itemInSlot);
                    }
                  }

                  inventory.equips[dragSlot] = itemInSlot;
                }
              }
              
              if (dragItem.type == ItemData.Type.WEAPON) {
                if (slot == 2) {
                  OnPrimaryWeaponUnequip();
                  OnPrimaryWeaponEquip(dragItem);
                }
                if (slot == 3) {
                  OnSecondaryWeaponUnequip();
                  OnSecondaryWeaponEquip(dragItem);
                }
              }

              inventory.equips[slot] = dragItem;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (activeItem != null) {
        itemTooltip.gameObject.SetActive(true);

        Vector2 mousePos = Input.mousePosition;
        RectTransform tooltipRect = itemTooltip.GetComponent<RectTransform>();

        TextMeshProUGUI nameText = itemTooltip.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = itemTooltip.GetChild(1).GetComponent<TextMeshProUGUI>();

        nameText.text = activeItem.title;
        descText.text = activeItem.description;

        float padding = 6f;

        Vector2 bgSize = new Vector2(
          Mathf.Min(400, Mathf.Max(nameText.preferredWidth, descText.preferredWidth) + padding * 2f),
          nameText.preferredHeight + descText.preferredHeight + padding * 3f);

        descText.GetComponent<RectTransform>().offsetMax = 
          new Vector2(
            descText.GetComponent<RectTransform>().offsetMax.x, 
            -(nameText.preferredHeight + padding * 2f));

        tooltipRect.sizeDelta = bgSize;
      }
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
      if (activeItem != null) {
        itemTooltip.gameObject.SetActive(false);

        TextMeshProUGUI nameText = itemTooltip.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = itemTooltip.GetChild(1).GetComponent<TextMeshProUGUI>();

        nameText.text = "";
        descText.text = "";
      }
    }

    void OnPrimaryWeaponEquip(ItemData data) {
      itemManager.equippedWeapon = Instantiate(weaponPrefab, new Vector3(0,0,0), Quaternion.identity);
      
      GameObject weaponObject = itemManager.equippedWeapon;
      Transform target = player.transform;

      weaponObject.transform.parent = target;
      weaponObject.transform.position = new Vector3(target.position.x, target.position.y, -5f);
      weaponObject.transform.rotation = target.rotation;

      weaponObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = data.icon;

      Weapon weapon = weaponObject.transform.GetComponent<Weapon>();
      Pickup pickup = weaponObject.transform.GetComponent<Pickup>();

      pickup.itemId = data.id;
      weapon.enabled = true;

      if (data.id == "bow" || data.id == "magic_wand" || data.id == "bottle" || data.id == "sling") {
        weapon.ranged = true;
      }

      if (data.id == "bow") {
        weapon.projectile = bowProjectile;
      } else if (data.id == "magic_wand") {
        weapon.projectile = magicProjectile;
      } else if (data.id == "bottle") {
        weapon.projectile = bottleProjectile;
      } else if (data.id == "sling") {
        weapon.projectile = slingProjectile;
      }
 
      if (target.GetComponent<PlayerMove>()) {
        target.GetComponent<PlayerMove>().weapon = weapon;
      }
            
      weaponObject.tag = target.tag;
      weaponObject.layer = 0;
      weaponObject.transform.SetSiblingIndex(0);

      target.Find("Fist").gameObject.SetActive(false);
    }

    void OnSecondaryWeaponEquip(ItemData data) {
      Debug.Log("Secondary Equip!");
    }

    void OnPrimaryWeaponUnequip() {
      DestroyImmediate(itemManager.equippedWeapon);
      itemManager.equippedWeapon = null;

      Transform fist = player.transform.Find("Fist");

      fist.gameObject.SetActive(true);
      player.transform.GetComponent<PlayerMove>().weapon = fist.GetComponent<Weapon>();
    }

    void OnSecondaryWeaponUnequip() {
      Debug.Log("Secondary Unequip!");
    }
}
