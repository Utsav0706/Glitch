using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Color color = Color.white;
    public float size = 10f;
    public float thickness = 2f;
    public float gap = 4f;
    public bool hideWhileAiming = true;
    public string aimButton = "Aim";

    void OnGUI()
    {
        if (hideWhileAiming && Input.GetAxisRaw(aimButton) != 0f) return;

        float cx = Screen.width * 0.5f;
        float cy = Screen.height * 0.5f;

        Color prev = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(new Rect(cx - thickness * 0.5f, cy - gap - size, thickness, size), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx - thickness * 0.5f, cy + gap, thickness, size), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx - gap - size, cy - thickness * 0.5f, size, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx + gap, cy - thickness * 0.5f, size, thickness), Texture2D.whiteTexture);

        GUI.color = prev;
    }
}
