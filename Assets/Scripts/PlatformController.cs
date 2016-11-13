using UnityEngine;
using System.Collections;

public class PlatformController : Glitchable {
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = false;

    public float moveForce = 5f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public Transform groundCheck;

    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;
    private bool dirty = false;

    // Use this for initialization
    void Awake ()
    {
        //anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update ()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        //anim.SetFloat("Speed", Mathf.Abs(h));

        // if (h * rb2d.velocity.x < maxSpeed && Mathf.Abs(h) > 0)
        //     rb2d.velocity = (Vector2.right * 1 * Mathf.Sign(h) * moveForce);
				//
        // if (Mathf.Abs (rb2d.velocity.x) > maxSpeed)
        //     rb2d.velocity = new Vector2(Mathf.Sign (rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

				if (h > 0) {
					rb2d.velocity = new Vector2(moveForce, rb2d.velocity.y);
          dirty = true;
				} else if (h < 0) {
					rb2d.velocity = new Vector2(-moveForce, rb2d.velocity.y);
          dirty = true;
				} else {
					rb2d.velocity = new Vector2(0, rb2d.velocity.y);
				}

        if (h > 0 && !facingRight)
            Flip ();
        else if (h < 0 && facingRight)
            Flip ();

        rb2d.gravityScale = 10.0f * this.getTime() * this.getTime();

        if (jump)
        {
            //anim.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0f, jumpForce * this.getTime()));
            //rb2d.velocity += Vector2.up * 70.0f * this.getTime();
            jump = false;
            dirty = true;
        }

        if (dirty) {
          dirty = false;
          rb2d.velocity = new Vector2(rb2d.velocity.x * this.getTime(), rb2d.velocity.y);
        }
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
