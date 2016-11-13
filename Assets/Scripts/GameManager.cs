using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
  protected float timeMult = 1.0f;

  public void setTimeMult(float time) {
    this.timeMult = timeMult;
  }

  public float getTimeMult() {
    return this.timeMult;
  }
}
