using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration) 
    {
        StartCoroutine(DoHitStop(duration));
    }

    private IEnumerator DoHitStop(float duration) 
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1f;
    }
}

