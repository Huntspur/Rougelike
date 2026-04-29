using UnityEngine;

public class MinionTracker : MonoBehaviour
{
    public Boss boss;

    private void OnDestroy()
    {
        if (boss != null) 
        {
            boss.MinionDied();

        }
    }
}