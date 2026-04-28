using UnityEngine;
[System.Serializable]
public class DropEntry 
{
    public GameObject prefab;
    [Range(0f, 1f)] public float dropChance;
}

public class EnemyDropTable : MonoBehaviour
{
    public DropEntry[] drops;

    public void RollDrops() 
    {
        foreach (var drop in drops) 
        {
            if (Random.value <= drop.dropChance) 
            {
                Instantiate(drop.prefab, transform.position, Quaternion.identity);
            }
        }
    }
}
