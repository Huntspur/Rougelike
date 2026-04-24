using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private PlayerInventory inventory;
    public int i;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    public void DropItem() 
    {
        foreach (Transform child in transform) 
        {
            child.GetComponent<WeaponButton>().SpawnDroppedItem(); 
            GameObject.Destroy(child.gameObject);        
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount <= 0) 
        {
            inventory.isFull[i] = false;
        }
    }
}
