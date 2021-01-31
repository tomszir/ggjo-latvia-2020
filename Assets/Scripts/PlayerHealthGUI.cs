using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthGUI : MonoBehaviour
{
    public int height = 6;
    public int width = 50;

    public Color healthColor = Color.green;
    public Color backgroundColor = Color.black;

    public Vector2 nativeSize = new Vector2(640, 480);
    
    private PlayerMove player;

    private GUIStyle guiStyle;
    private Texture2D healthTexture;
    private Texture2D backgroundTexture;

    void Start()
    {
        player = GetComponent<PlayerMove>();

        guiStyle = new GUIStyle();

        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, backgroundColor);
        backgroundTexture.Apply();
        
        healthTexture = new Texture2D(1, 1);
        healthTexture.SetPixel(0, 0, healthColor);
        healthTexture.Apply();
    }

    void OnGUI() 
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
        Vector2 pos = new Vector2(
            screenPos.x / Screen.width  * nativeSize.x, 
            screenPos.y / Screen.height * nativeSize.y);

        guiStyle.normal.background = backgroundTexture;

        float x = pos.x - width / 2;
        float y = nativeSize.y - pos.y - 45;

        Vector3 scale = new Vector3(
            Screen.width  / nativeSize.x, 
            Screen.height / nativeSize.y, 1.0f);
        
        GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, scale);

        GUI.Box(
            new Rect(x, y, width, height), 
            GUIContent.none, guiStyle);
        
        guiStyle.normal.background = healthTexture;

        GUI.Box(
            new Rect(x + 1, y + 1, player.GetHealthPercentage() * (width - 2), height - 2), 
            GUIContent.none, guiStyle);
    }
}
