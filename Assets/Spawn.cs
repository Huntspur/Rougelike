using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject SpawnPrefab;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SpawnDroppedItem() 
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.y + 1);
        Instantiate(SpawnPrefab, playerPos, Quaternion.identity);
    }
}
