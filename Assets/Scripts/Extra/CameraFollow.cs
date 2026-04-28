using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float followSpeed = 8f;
    public float yOffset = 1f;
    private Transform target;

    [Header("Cam Shake")]
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    public static CameraFollow Instance;

    private void Awake()
    {
        Instance = this;
        Debug.Log("Camera instance: " + CameraFollow.Instance);
    }

    public void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void LateUpdate()
    {

        if (target == null) 
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + yOffset, -10f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition + shakeOffset, followSpeed * Time.deltaTime);

        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0f;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}