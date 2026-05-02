using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BiomeIndicator : MonoBehaviour
{
    public TMP_Text biomeText;
    public Image biomeIcon;

    private EnergySystem energy;

    void Start()
    {
        energy = FindFirstObjectByType<EnergySystem>();
        energy.OnBiomeChanged += OnBiomeChanged;
    }

    void OnBiomeChanged(BiomeType biome)
    {
        biomeText.text = biome.ToString();
    }
}
