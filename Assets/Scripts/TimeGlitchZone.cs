using UnityEngine;
using System.Collections;

public class TimeGlitchZone : MonoBehaviour {
  public float timeValue = 0.5f;

  public void OnTriggerEnter2D(Collider2D col) {
    Glitchable g = col.gameObject.GetComponent<Glitchable>();
    g.setTime(this.timeValue);
  }

  public void OnTriggerExit2D(Collider2D col) {
    Glitchable g = col.gameObject.GetComponent<Glitchable>();
    g.setTime(1.0f);
  }
}
