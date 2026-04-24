using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private PlayerInventory inventory;
    public GameObject itemButton;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (inventory == null) { return; }

        if (collision.CompareTag("Player")) 
        {
            for (int i = 0; i < inventory.slots.Length; i++) 
            {
                if (inventory.isFull[i] == false) 
                {
                    //add item

                    inventory.isFull[i] = true;
                    Instantiate(itemButton, inventory.slots[i].transform, false);
                    Destroy(gameObject);
                    break;
                }
            }     
        }
    }
}
