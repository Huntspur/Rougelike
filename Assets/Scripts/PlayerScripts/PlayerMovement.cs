using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2f;
    [SerializeField]
    private bool facingRight;

    private Animator anim;
    private Rigidbody2D rb;

    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        facingRight = true;
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();
  

        if (input.x != 0) 
        {
            Flip();
        }

        if (input != Vector2.zero)
        {
            anim.SetBool("isMoving", true);
        }
        else 
        {
            anim.SetBool("isMoving", false);
        }


    }
   

    void Flip()
    {
        if (input.x > 0 && !facingRight || input.x < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector2 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void LateUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
