using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2f;

    private Animator anim;
    private Rigidbody2D rb;

    private Vector2 input;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();

        if (input != Vector2.zero)
        {
            anim.SetBool("isMoving", true);
        }
        else 
        {
            anim.SetBool("isMoving", false);
        }
    }

    private void LateUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
