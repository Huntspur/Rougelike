using UnityEngine;

public class ExitDoor : MonoBehaviour
{

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null && Boss.IsDefeated)
            {
                GameManager.Instance.DungeonCleared();
                GameManager.Instance.GenerateNewDungeon();
            }
        }
    }
}