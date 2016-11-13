using UnityEngine;
using System.Collections;

public class BulletController : Glitchable {
  public float speed = 500.0f;

  protected Rigidbody2D rb;

  public void Start() {
    this.rb = this.GetComponent<Rigidbody2D>();
    this.rb.velocity = new Vector2(this.speed, this.rb.velocity.y);
  }

  public void OnLocalTimeChanged(float time) {
    Debug.Log("LOcal time changed");

    this.rb.velocity = new Vector2(
      this.speed * time,
      this.rb.velocity.y
    );

    this.rb.gravityScale = time * time;
  }

  public void OnTriggerEnter2D(Collider2D col) {
    Debug.Log("COLLIDED");

    if (col.gameObject.tag == "Player") {
      Debug.Log("COLLIDED WITH PLAYER");
    }
  }
}
