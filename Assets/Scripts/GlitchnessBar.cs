using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlitchnessBar : MonoBehaviour {
  Rect rect;
  Texture2D texture;
  int barHeight = 10;
  int barWidth;
  Color barColor = new Color(1f, 0.6f, 0);
  int maxGlitchness = 100;
  float barX, barY;

  [Range(0, 100)]
  public float currentGlitchness;

  void Start() {
    currentGlitchness = maxGlitchness;
    barWidth = maxGlitchness;

    barX = Screen.width - barWidth - (barWidth / 3);
    barY = barHeight + (barWidth / 5);

    rect = new Rect(barX, barY, barWidth, barHeight);

    texture = new Texture2D(1, 1);
    texture.SetPixel(0, 0, barColor);
    texture.Apply();
  }

  void OnGUI() {
    float ratio = currentGlitchness / maxGlitchness;

    rect.width = ratio * barWidth;

    GUI.DrawTexture(rect, texture);
  }
}
