using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GuardPatrolStep {
  public int index = 0;
  public float minWaitTime = 1.0f;
  public float maxWaitTime = 2.0f;
  public bool canWait = false;
}

public class GuardController : Glitchable {
  public List<Vector2> points = new List<Vector2>();
  public List<GuardPatrolStep> sequence = new List<GuardPatrolStep>();
  public float speed = 3.5f;
  public float minDistance = 0.01f;
  public float minPlayerDistance = 30.0f;
  public GameObject shootParticles;

  protected int nextPoint = 0;
  protected Rigidbody2D rb;
  protected Animator anim;
  protected Vector3 initialScale = new Vector3(1, 1, 1);
  protected float? waitStartTime = null;
  protected float? waitTime = null;

  protected string state = "patroling";

  public void Start() {
    this.rb = this.GetComponent<Rigidbody2D>();
    this.initialScale = this.transform.localScale;
    this.anim = this.GetComponent<Animator>();
  }

  public void FixedUpdate() {
    GameObject player = GameObject.FindWithTag("Player");

    if (this.state == "patroling") {
      if (player) {
        float playerDist = player.transform.position.x - this.transform.position.x;

        if (Mathf.Abs(playerDist) < this.minPlayerDistance) {
          if (Mathf.Sign(playerDist) == Mathf.Sign(this.transform.localScale.x)) {
            this.state = "attacking";
            this.anim.SetTrigger("Fire");
          }
        }
      }

      if (this.waitStartTime.HasValue && this.waitTime.HasValue) {
        if ((Time.time - this.waitStartTime.GetValueOrDefault()) * this.getTime() < this.waitTime.GetValueOrDefault()) {
          return;
        }

        this.waitStartTime = null;
        this.waitTime = null;
      }

      GuardPatrolStep step = this.sequence[this.nextPoint % this.sequence.Count];
      Vector2 nextPointVec = this.points[step.index];
      float dist = nextPointVec.x - this.transform.position.x;

      if (Mathf.Abs(dist) < this.minDistance) {
        nextPoint = (nextPoint + 1) % this.sequence.Count;

        if (step.canWait) {
          float amount = Random.Range(step.minWaitTime, step.maxWaitTime);
          Debug.Log("Waiting");
          Debug.Log(amount);

          this.waitStartTime = Time.time;
          this.waitTime = amount;
        }

      } else {
        bool right = this.transform.position.x < nextPointVec.x;
        this.transform.localScale = new Vector3(initialScale.x * (right ? 1 : -1), initialScale.y, initialScale.z);
        this.rb.velocity = new Vector2(this.speed * (right ? 1 : -1) * this.getTime(), this.rb.velocity.y);
      }
    }
  }

  public void Fire() {
    GameObject player = GameObject.FindWithTag("Player");
    Transform gunSpot = this.transform.Find("Gun Spot");

    Instantiate(this.shootParticles, gunSpot.position, Quaternion.identity);

    RaycastHit2D hit = Physics2D.Raycast(
      this.transform.position,
      Vector2.right * Mathf.Sign(this.transform.localScale.x),
      Mathf.Infinity,
      LayerMask.GetMask("Obstacles", "Player")
    );

    if (hit) {
      Debug.Log("Hit something");
      Debug.Log(hit.transform.gameObject);
      hit.transform.gameObject.SendMessage(
        "Hit",
        new object[] { hit, this.transform.gameObject },
        SendMessageOptions.DontRequireReceiver
      );
    }
  }

  public void OnDrawGizmos() {
    Gizmos.color = Color.red;

    foreach(Vector2 p in this.points) {
      Gizmos.DrawWireSphere(new Vector3(p.x, p.y, 0), 0.5f);
    }
  }
}
