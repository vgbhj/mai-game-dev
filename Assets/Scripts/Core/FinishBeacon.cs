using UnityEngine;

public class FinishBeacon : MonoBehaviour
{
    public float pillarHeight = 30f;
    public float pillarRadius = 1.5f;
    public Color beaconColor = new Color(0f, 1f, 0.5f, 1f);
    public float lightRange = 50f;
    public float pulseSpeed = 2f;

    private Light beaconLight;
    private Material pillarMaterial;

    void Start()
    {
        var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.transform.SetParent(transform);
        pillar.transform.localPosition = new Vector3(0, pillarHeight / 2f, 0);
        pillar.transform.localScale = new Vector3(pillarRadius, pillarHeight / 2f, pillarRadius);
        Destroy(pillar.GetComponent<Collider>());

        pillarMaterial = CreateEmissiveMaterial(beaconColor * 0.5f, beaconColor * 3f);
        pillar.GetComponent<Renderer>().material = pillarMaterial;

        var lightObj = new GameObject("BeaconLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = new Vector3(0, pillarHeight + 2f, 0);
        beaconLight = lightObj.AddComponent<Light>();
        beaconLight.type = LightType.Point;
        beaconLight.color = beaconColor;
        beaconLight.range = lightRange;
        beaconLight.intensity = 5f;

        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.transform.SetParent(transform);
        ring.transform.localPosition = Vector3.zero;
        ring.transform.localScale = new Vector3(5f, 0.1f, 5f);
        Destroy(ring.GetComponent<Collider>());

        var ringMat = CreateEmissiveMaterial(beaconColor * 0.3f, beaconColor * 2f);
        ring.GetComponent<Renderer>().material = ringMat;
    }

    Material CreateEmissiveMaterial(Color baseColor, Color emissionColor)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", baseColor);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissionColor);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        return mat;
    }

    void Update()
    {
        if (beaconLight != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.4f;
            beaconLight.intensity = 5f * pulse;
        }
    }
}
