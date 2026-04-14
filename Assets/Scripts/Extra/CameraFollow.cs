using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 8f;
    public float yOffset = 1f;

    private Transform target;

    public void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void LateUpdate()
    {

        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x,target.position.y + yOffset, -10f);

        transform.position = Vector3.Lerp( transform.position,desiredPosition,followSpeed * Time.deltaTime);
    }
}