using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string itemId = "cheese";

    private ItemData item;
    private ItemManager itemManager;
    private PlayerInventory playerInventory;

    private bool pickedUp = false;

    void Start() {
      itemManager = GameObject
        .FindGameObjectWithTag("GameController")
        .GetComponent<ItemManager>();

      playerInventory = GameObject
        .FindGameObjectWithTag("Player")
        .GetComponent<PlayerInventory>();

      item = itemManager.GetItem(itemId);

      if (item == null) {
        gameObject.SetActive(false);
        return;
      }

      transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item.icon;
    }

    public void Take(Transform target) 
    {
        if (!pickedUp && playerInventory.HasSpace()) {
            // GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>().health += .5f;
            playerInventory.AddItem(item, gameObject);
            gameObject.SetActive(false);
            pickedUp = true;
        }
        
        // transform.parent = target;
        // transform.position = new Vector3(target.position.x, target.position.y, -5f);
        // transform.rotation = target.rotation;

        /*
        if (GetComponent<Weapon>()) {
            GetComponent<Weapon>().enabled = true;
            
            if (target.GetComponent<PlayerMove>()) {
              target.GetComponent<PlayerMove>().weapon = GetComponent<Weapon>();
            }
            
            tag = target.tag;
            gameObject.layer = 0;
            transform.SetSiblingIndex(0);

            target.Find("Fist").gameObject.SetActive(false);
        }*/
    }
}
