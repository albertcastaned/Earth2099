using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ColorController : MonoBehaviour
{
    public PlayerColorModifier playerColor;
    public Button mainBodyButton;
    public Button mainBody2Button;
    public Button mainBody3Button;
    public Button secondaryButton;
    public Button secondary2Button;
    public Button secondary3Button;
    public Button visorButton;

    public Texture2D defaultTexture;

    private float visorDarken = 0.5f;
    private const int cellWidth = 128;
    private const int cellHeight = 128;

    Renderer[] renderers;

    private Texture2D texture;
    private Color visorColor;
    public Color defaultVisorColor;

    private const int cellSize = cellWidth * cellHeight;

    public enum ColorChangeType
    {
        MainBody1,
        MainBody2,
        MainBody3,
        Secondary1,
        Secondary2,
        Secondary3,
        Visor
    }


    public void AttemptLoadPrefs()
    {

        Color color;
        Color32[] colors;
        Vector2Int[] positions;

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.MainBody1.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(0, 3), new Vector2Int(3, 3) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.MainBody2.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(1, 3) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.MainBody3.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(2, 3) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.Secondary1.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(0, 2), new Vector2Int(3, 1) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.Secondary2.ToString()), out color))
        { 
        colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
        positions = new Vector2Int[] { new Vector2Int(1, 2) };
        FillCells(positions, colors);
        }
        if(ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.Secondary3.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(3, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorChangeType.Visor.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(2, 1) };
            FillCells(positions, colors);

            Color darkenVisor = color;
            darkenVisor.r -= visorDarken;
            darkenVisor.g -= visorDarken;
            darkenVisor.b -= visorDarken;

            Color32[] auxColors = Enumerable.Repeat((Color32)darkenVisor, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(1, 1) };
            FillCells(positions, auxColors);
            visorColor = color;

            foreach (Renderer renderer in renderers)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", visorColor * 2.388657f);
            }
        }


        texture.Apply();
    }

    void FillCells(Vector2Int[] positions, Color32[] color)
    {
        foreach (Vector2Int pos in positions)
            texture.SetPixels32(pos.x * cellWidth, pos.y * cellHeight, cellWidth, cellHeight, color, 0);
    }

    private ColorChangeType currentColorChangeType;


    public void ResetToDefault()
    {
        texture.SetPixels(defaultTexture.GetPixels());
        texture.Apply();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.mainTexture = texture;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", defaultVisorColor);
        }
        PlayerPrefs.SetInt("IsDefaultColor", 1);
        foreach(var type in Enum.GetValues(typeof(ColorChangeType)))
        {
            PlayerPrefs.DeleteKey(type.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        texture = new Texture2D(defaultTexture.width, defaultTexture.height);
        texture.SetPixels(defaultTexture.GetPixels());
        texture.Apply();
        AttemptLoadPrefs();
        
        foreach (Renderer renderer in renderers)
        {
            renderer.material.mainTexture = texture;
        }

        currentColorChangeType = ColorChangeType.MainBody1;

        mainBodyButton.onClick.AddListener(() => currentColorChangeType = ColorChangeType.MainBody1);
        mainBody2Button.onClick.AddListener(() => currentColorChangeType = ColorChangeType.MainBody2);
        mainBody3Button.onClick.AddListener(() => currentColorChangeType = ColorChangeType.MainBody3);

        secondaryButton.onClick.AddListener(() => currentColorChangeType = ColorChangeType.Secondary1);
        secondary2Button.onClick.AddListener(() => currentColorChangeType = ColorChangeType.Secondary2);
        secondary3Button.onClick.AddListener(() => currentColorChangeType = ColorChangeType.Secondary3);
        visorButton.onClick.AddListener(() => currentColorChangeType = ColorChangeType.Visor);

    }
    public void SetColorCell(Color color)
    {
        if (texture == null)
            return;
        Debug.Log("Setting new colors");
        Color32[] colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
        Vector2Int[] positions;
        switch (currentColorChangeType)
        {
            case ColorChangeType.MainBody1:
                positions = new Vector2Int[] { new Vector2Int(0, 3), new Vector2Int(3, 3) };
                FillCells(positions, colors);

                break;
            case ColorChangeType.MainBody2:
                positions = new Vector2Int[] { new Vector2Int(1, 3) };
                FillCells(positions, colors);
                break;
            case ColorChangeType.MainBody3:
                positions = new Vector2Int[] { new Vector2Int(2, 3) };
                FillCells(positions, colors);
                break;
            case ColorChangeType.Secondary1:
                positions = new Vector2Int[] { new Vector2Int(0, 2), new Vector2Int(3, 1) };
                FillCells(positions, colors);
                break;
            case ColorChangeType.Secondary2:
                positions = new Vector2Int[] { new Vector2Int(1, 2) };
                FillCells(positions, colors);
                break;
            case ColorChangeType.Secondary3:
                positions = new Vector2Int[] { new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(3, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                FillCells(positions, colors);

                break;
            case ColorChangeType.Visor:
                positions = new Vector2Int[] { new Vector2Int(2, 1) };
                FillCells(positions, colors);
                Color darkenVisor = color;
                darkenVisor.r -= visorDarken;
                darkenVisor.g -= visorDarken;
                darkenVisor.b -= visorDarken;
                Color32[] auxColors = Enumerable.Repeat((Color32)darkenVisor, cellSize).ToArray();
                positions = new Vector2Int[] { new Vector2Int(1, 1) };
                FillCells(positions, auxColors);
                visorColor = color;
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", visorColor);
                }
                break;
        }
        texture.Apply();

        PlayerPrefs.DeleteKey("IsDefaultColor");
        SaveColorToPrefs(currentColorChangeType.ToString(), $"#{ColorUtility.ToHtmlStringRGB(color)}");
    }

    void SaveColorToPrefs(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
    }

    public void Save()
    {
        Debug.Log("Color prefs saved");
        PlayerPrefs.SetInt("ColorSaved", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Launcher");
    }
}
