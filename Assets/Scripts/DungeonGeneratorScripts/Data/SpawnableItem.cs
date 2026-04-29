using UnityEngine;


[CreateAssetMenu(menuName = "Dungeon/SpawnableItem")]
public class SpawnableItem : ScriptableObject
{
    public GameObject prefab;
    [Range(0f, 1f)] public float spawnChance = 0.1f;
    public int maxPerRoom = 3;
}