using UnityEngine;
using System.Collections;

public class CustomTimeApply : MonoBehaviour {
  protected float time = 1.0f;

  protected Rigidbody2D rb;

  public void Awake() {
    rb = GetComponent<Rigidbody2D>();
  }

  public void setTime(float time) {
    this.time = time;
  }

  public float getTime() {
    return this.time;
  }

  public void FixedUpdate() {
    rb.velocity *= time;
  }
}
