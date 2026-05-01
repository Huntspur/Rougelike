using UnityEngine;

public class FootstepParticle : MonoBehaviour
{
    public float stepInterval = 0.3f;
    private float stepTimer = 0f;
    private Rigidbody2D rb;
    public Transform feet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = stepInterval;
                ParticleManager.Instance.PlayDust(feet.position);
                AudioManager.Instance.PlayWithVariation(AudioManager.Instance.footstep);
            }
        }
    }
}