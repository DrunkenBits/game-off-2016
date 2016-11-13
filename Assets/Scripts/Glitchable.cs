using UnityEngine;
using System.Collections;

public class Glitchable : MonoBehaviour {
  protected float time = 1.0f;

  public void setTime(float time) {
    this.time = time;
    this.gameObject.SendMessage("OnLocalTimeChanged", time);
  }

  public float getTime() {
    return this.time;
  }
}
