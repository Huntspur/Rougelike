using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("Prefabs")]
    public GameObject bloodHitPrefab;
    public GameObject dustPrefab;
    public GameObject damageNumber;
    private void Awake()
    {
        Instance = this;
    }

    public void PlayBloodHit(Vector2 position)
    {
        if (bloodHitPrefab == null) 
        {
            return;
        }
        Instantiate(bloodHitPrefab, position, Quaternion.identity);
    }

    public void PlayDust(Vector2 position)
    {
        if (dustPrefab == null)
        { 
            return; 
        }
        Instantiate(dustPrefab, position, Quaternion.identity);
    }

    public void PlayDamageNumber(int damage, Vector2 position, bool isPlayer = false)
    {
        if (damageNumber == null) 
        {
            return;
        
        }
        var obj = Instantiate(damageNumber, position + Vector2.up * 0.5f, Quaternion.identity);
        DamageNumber dn = obj.GetComponent<DamageNumber>();

        if (dn != null)
        {
            Color color = isPlayer ? Color.red : Color.yellow;
            dn.Init(damage, color);
        }
    }
}
