using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyBar : MonoBehaviour
{
    public Image fillImage;
    public TMP_Text percentText;
    public TMP_Text drainText;
    public Gradient colorGradient;

    private EnergySystem energy;

    void Start()
    {
        energy = FindFirstObjectByType<EnergySystem>();
        energy.OnEnergyChanged += UpdateBar;
        UpdateBar(1f);
    }

    void UpdateBar(float normalized)
    {
        fillImage.fillAmount = normalized;
        fillImage.color = colorGradient.Evaluate(normalized);
        percentText.text = $"{Mathf.CeilToInt(normalized * 100)}%";
    }

    void Update()
    {
        if (energy != null)
            drainText.text = $"-{energy.DrainRate:F1}/s";
    }
}
