
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DemonCoreAccident : MonoBehaviour
{
    public Transform screwdriver;
    public Transform sphereTop;
    public Animator sphereAnimator;

    public float screwdriverForce = 0.0001f;

    private bool triggered;
    private Vector3 closedPosition;


    public Light coreLight;
    public Renderer[] coreRenderers;

    public Color emissionColor = new Color(0.05f, 0.2f, 1f);
    public float targetLightIntensity = 100f;
    public float targetEmissionIntensity = 10f;
    public float glowDuration = 5f;

    private bool glowStarted;

    public Light directionalLight;
    public float targetDirectionalIntensity = 0.1f;


    

    public Volume globalVolume;
    public float targetBloomIntensity = 100f;
    private Bloom bloom;

private void Start()
    {
        if (sphereTop != null)
        {
            closedPosition = sphereTop.localPosition;
        }
    }

    public void TriggerAccident()
    {
        if (triggered)
            return;

        triggered = true;
        AccidentSequence();
    }

    private void AccidentSequence()
    {
        Debug.Log("TRIGGER ACCIDENT CALLED");
        if (screwdriver != null)
        {
            Rigidbody rb = screwdriver.GetComponent<Rigidbody>();

            if (rb == null)
                rb = screwdriver.gameObject.AddComponent<Rigidbody>();

            rb.AddForce((Vector3.up + Vector3.right).normalized * screwdriverForce, ForceMode.Impulse);

            rb.AddTorque(Random.insideUnitSphere * screwdriverForce * 1000,ForceMode.Impulse);
        }

        if (sphereTop != null)
        {
            sphereAnimator.SetTrigger("Close");
        }

        Debug.Log("death");
    }

    public void StartGlow()
    {
        if (glowStarted)
            return;

        glowStarted = true;
        globalVolume.profile.TryGet(out bloom);
        StartCoroutine(GlowSequence());
    }

    private System.Collections.IEnumerator GlowSequence()
    {
        Material[] materials = new Material[coreRenderers.Length];
        Color[] baseEmissions = new Color[coreRenderers.Length];

        for (int i = 0; i < coreRenderers.Length; i++)
        {
            materials[i] = coreRenderers[i].material;
            materials[i].EnableKeyword("_EMISSION");

            Color original = materials[i].GetColor("_EmissionColor");

            // Preserve the color, but normalize its intensity.
            float maxChannel = Mathf.Max(original.r, original.g, original.b);

            baseEmissions[i] = maxChannel > 0.001f
                ? original / maxChannel
                : Color.cyan;
        }

        float elapsed = 0f;
        float startingDirectionalIntensity = directionalLight.intensity;
        bloom.intensity.value = 0f;

        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / glowDuration);

            coreLight.intensity = Mathf.Lerp(0f, targetLightIntensity, t);

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor(
                    "_EmissionColor",
                    baseEmissions[i] * Mathf.Lerp(0f, targetEmissionIntensity, t)
                );
            }

            directionalLight.intensity = Mathf.Lerp(startingDirectionalIntensity, targetDirectionalIntensity, t);
            bloom.intensity.value = Mathf.Lerp(0f, targetBloomIntensity, t);

            yield return null;
        }
        directionalLight.intensity = targetDirectionalIntensity;
        coreLight.intensity = targetLightIntensity;
        bloom.intensity.value = targetBloomIntensity;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor(
                "_EmissionColor",
                baseEmissions[i] * targetEmissionIntensity
            );
        }
    }
}


