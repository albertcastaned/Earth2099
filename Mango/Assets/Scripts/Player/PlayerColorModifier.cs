using UnityEngine;
using System.Linq;
using Photon.Pun;
using UnityEngine.Rendering;
using System.Text;

public class PlayerColorModifier : MonoBehaviourPun
{
    public Texture2D defaultTexture;
    public GameObject parent;
    public TrailRenderer trailRenderer;
    Renderer[] renderers;
    private Texture2D texture;
    private Texture2D emissionTexture;
    private Color visorColor;
    private const int cellSize = cellWidth * cellHeight;
    private float visorDarken = 0.5f;
    private const int cellWidth = 128;
    private const int cellHeight = 128;

    private const float visorEmissionIntensity = 15f;
    private const float trailEmissionIntensity = 50f;

    // Start is called before the first frame update
    void Start()
    {

        if (photonView.IsMine)
        {
            texture = new Texture2D(512, 512);
            texture.SetPixels(defaultTexture.GetPixels());
            texture.Apply();

            renderers = parent.GetComponentsInChildren<Renderer>();

            AttemptLoadPrefs();

            foreach (Renderer renderer in renderers)
            {
                renderer.material.mainTexture = texture;
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", visorColor * visorEmissionIntensity);
                renderer.material.SetTexture("_EmissionMap", emissionTexture);
            }

            byte[] bytes = texture.EncodeToPNG();
            photonView.RPC(nameof(UpdateColor), RpcTarget.OthersBuffered, bytes, visorColor.r, visorColor.g, visorColor.b);
        }
    }
    [PunRPC]
    void UpdateColor(byte[] bytes, float r, float g, float b)
    {
        Texture2D newTex = new Texture2D(defaultTexture.width, defaultTexture.height);

        if(texture == null)
        {
            texture = new Texture2D(512, 512);
            texture.SetPixels(defaultTexture.GetPixels());
            texture.Apply();
        }

        if (!newTex.LoadImage(bytes) || newTex == null)
            Debug.LogError("Error loading material texture");

        texture.SetPixels(newTex.GetPixels());
        texture.Apply();
        if(renderers == null)
        {
            renderers = parent.GetComponentsInChildren<Renderer>();
        }
        visorColor = new Color(r, g, b);
        Color32[] blackColors = Enumerable.Repeat((Color32)Color.black, cellSize).ToArray();

        // Set emisison texture
        emissionTexture = new Texture2D(512, 512);
        emissionTexture.SetPixels32(0, 0, cellWidth, cellHeight, blackColors, 0);
        Color darkenVisor = visorColor;
        darkenVisor.r -= visorDarken;
        darkenVisor.g -= visorDarken;
        darkenVisor.b -= visorDarken;

        Color32[] auxColors = Enumerable.Repeat((Color32)darkenVisor, cellSize).ToArray();

        // Set emisison texture darker
        emissionTexture = new Texture2D(512, 512);
        emissionTexture.SetPixels(new Color[512 * 512]);
        emissionTexture.Apply();
        emissionTexture.SetPixels32(1 * cellWidth, 1 * cellHeight, cellWidth, cellHeight, auxColors, 0);
        emissionTexture.Apply();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.mainTexture = texture;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", visorColor * visorEmissionIntensity);
            renderer.material.SetTexture("_EmissionMap", emissionTexture);
        }
        Color mainColor = newTex.GetPixel(cellWidth * 0, cellHeight * 3);
        trailRenderer.material.SetColor("_EmissionColor", mainColor * trailEmissionIntensity);
        trailRenderer.material.color = new Color(mainColor.r, mainColor.b, mainColor.g, 0.1f);

    }

    public void AttemptLoadPrefs()
    {

        Color color;
        Color32[] colors;
        Vector2Int[] positions;

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.MainBody1.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(0, 3), new Vector2Int(3, 3) };
            FillCells(positions, colors);
            trailRenderer.material.SetColor("_EmissionColor", color * trailEmissionIntensity);
            trailRenderer.material.color = new Color(color.r, color.b, color.g, 0.1f);

        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.MainBody2.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(1, 3) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.MainBody3.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(2, 3) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.Secondary1.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(0, 2), new Vector2Int(3, 1) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.Secondary2.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(1, 2) };
            FillCells(positions, colors);
        }
        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.Secondary3.ToString()), out color))
        {
            colors = Enumerable.Repeat((Color32)color, cellSize).ToArray();
            positions = new Vector2Int[] { new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(3, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
            FillCells(positions, colors);
        }

        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(ColorController.ColorChangeType.Visor.ToString()), out color))
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

            // Set emisison texture
            emissionTexture = new Texture2D(512, 512);
            emissionTexture.SetPixels(new Color[512 * 512]);
            emissionTexture.Apply();
            emissionTexture.SetPixels32(1 * cellWidth, 1 * cellHeight, cellWidth, cellHeight, auxColors, 0);
            emissionTexture.Apply();

        }

        texture.Apply();
    }


    void FillCells(Vector2Int[] positions, Color32[] color)
    {
        foreach (Vector2Int pos in positions)
            texture.SetPixels32(pos.x * cellWidth, pos.y * cellHeight, cellWidth, cellHeight, color, 0);
    }


}
