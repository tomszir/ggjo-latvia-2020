using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthGUI : MonoBehaviour
{
    public int height = 6;
    float width;

    public Color healthColor = Color.red;
    public Color backgroundColor = Color.black;

    public Vector2 nativeSize = new Vector2(640, 480);
    
    private EnemyMove enemy;

    private GUIStyle guiStyle;
    private Texture2D healthTexture;
    private Texture2D backgroundTexture;

    void Start()
    {
        enemy = GetComponent<EnemyMove>();

        guiStyle = new GUIStyle();

        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, backgroundColor);
        backgroundTexture.Apply();
        
        healthTexture = new Texture2D(1, 1);
        healthTexture.SetPixel(0, 0, healthColor);
        healthTexture.Apply();

        width = enemy.health * 12f;
    }

    void OnGUI() 
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
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
            new Rect(x + 1, y + 1, enemy.GetHealthPercentage() * (width - 2), height - 2), 
            GUIContent.none, guiStyle);
    }
}
