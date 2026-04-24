using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    public SOWeaponScripty data;
    private Weapon weaponController;

    public GameObject SpawnPrefab;
    private Transform player;
    void Awake()
    {
        weaponController = GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    public void SpawnDroppedItem()
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.y + 1);
        Instantiate(SpawnPrefab, playerPos, Quaternion.identity);
        weaponController.weaponData = null;
        weaponController.UnEquipWeapon();
    }
    

    public void EquipWeapon() 
    {
        if (weaponController.weaponData == data)
        {
            weaponController.weaponData = null;
            weaponController.UnEquipWeapon();
            return; 
        }

        if (weaponController.weaponData != null)
        {
            weaponController.UnEquipWeapon();
        }

        weaponController.weaponData = data;
        weaponController.EquipWeapon(data);
    }


}
