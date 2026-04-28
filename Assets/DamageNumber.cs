using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public float lifetime = 0.8f;
    public float floatSpeed = 1.5f;
    public float fadeSpeed = 2f;
    private TextMeshPro tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    public void Init(int damage, Color color)
    {
        tmp.text = damage.ToString();
        tmp.color = color;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Color startColor = tmp.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            transform.position = startPos + Vector3.up * floatSpeed * t;

            tmp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
