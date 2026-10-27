using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaticOverlay : MonoBehaviour
{
    public RawImage staticImage;

    public int textureSize = 512;
    public float fadeDuration = 8f;
    public float targetOpacity = 1f;
    public float noiseUpdateInterval = 0.05f;

    private Texture2D noiseTexture;
    private Color32[] noisePixels;
    private bool started;

    private void Awake()
    {
        if (staticImage == null)
            staticImage = GetComponent<RawImage>();

        noiseTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.wrapMode = TextureWrapMode.Repeat;

        noisePixels = new Color32[textureSize * textureSize];

        staticImage.texture = noiseTexture;

        SetOpacity(0f);
        GenerateNoise();
    }


    private void SetOpacity(float opacity)
    {
        Color color = staticImage.color;
        color.a = opacity;
        staticImage.color = color;
    }

    private void GenerateNoise()
    {
        for (int i = 0; i < noisePixels.Length; i++)
        {
            byte brightness = (byte)Random.Range(0, 256);

            noisePixels[i] = new Color32(
                brightness,
                brightness,
                brightness,
                255
            );
        }

        noiseTexture.SetPixels32(noisePixels);
        noiseTexture.Apply(false);
    }

    public void StartStatic()
    {
        if (started)
            return;

        started = true;
        StartCoroutine(StaticSequence());
    }

    private IEnumerator StaticSequence()
    {
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetOpacity(Mathf.Lerp(0f, targetOpacity, t));

            GenerateNoise();

            yield return new WaitForSeconds(noiseUpdateInterval);
        }
    }
}
