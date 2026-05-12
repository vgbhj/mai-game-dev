using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeSlotUI : MonoBehaviour
{
    public Image[] slotIcons;
    public TMP_Text[] slotLabels;
    public Button[] removeButtons;
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;
    public Color emptyColor = new Color(1, 1, 1, 0.2f);
    public Color benefitColor = new Color(0.2f, 0.9f, 0.3f, 1f);
    public Color penaltyColor = new Color(0.9f, 0.2f, 0.2f, 1f);
    public Color neutralColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    public float outlineWidth = 3f;

    private UpgradeInventory inventory;
    private EnergySystem energy;
    private Outline[] slotOutlines;

    private static readonly string[] BiomeNames = { "Песок", "Лёд", "Камень", "Болото" };

    void Start()
    {
        inventory = FindFirstObjectByType<UpgradeInventory>();
        energy = FindFirstObjectByType<EnergySystem>();

        if (inventory == null || energy == null) return;

        inventory.OnInventoryChanged += Refresh;
        energy.OnBiomeChanged += _ => UpdateColors();

        slotOutlines = new Outline[slotIcons.Length];
        for (int i = 0; i < slotIcons.Length; i++)
        {
            slotIcons[i].raycastTarget = false;
            var outline = slotIcons[i].gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(outlineWidth, outlineWidth);
            outline.effectColor = emptyColor;
            slotOutlines[i] = outline;
        }

        for (int i = 0; i < slotLabels.Length; i++)
            if (slotLabels[i] != null)
                slotLabels[i].raycastTarget = false;

        for (int i = 0; i < removeButtons.Length; i++)
        {
            int index = i;
            removeButtons[i].onClick.AddListener(() => RemoveAt(index));

            var hover = removeButtons[i].gameObject.AddComponent<SlotHoverHandler>();
            hover.Init(this, index);
        }

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < inventory.ActiveUpgrades.Count)
            {
                var upgrade = inventory.ActiveUpgrades[i];

                if (upgrade.icon != null)
                {
                    slotIcons[i].sprite = upgrade.icon;
                    slotIcons[i].color = Color.white;
                }
                else
                {
                    slotIcons[i].sprite = null;
                    slotIcons[i].color = new Color(1, 1, 1, 0.5f);
                }

                if (i < slotLabels.Length && slotLabels[i] != null)
                {
                    slotLabels[i].text = !string.IsNullOrEmpty(upgrade.upgradeName)
                        ? upgrade.upgradeName
                        : upgrade.name;
                    slotLabels[i].enabled = true;
                }

                removeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = emptyColor;

                if (i < slotLabels.Length && slotLabels[i] != null)
                {
                    slotLabels[i].text = "";
                    slotLabels[i].enabled = false;
                }

                removeButtons[i].gameObject.SetActive(false);

                if (slotOutlines != null && i < slotOutlines.Length)
                    slotOutlines[i].effectColor = emptyColor;
            }
        }

        UpdateColors();
    }

    void UpdateColors()
    {
        if (slotOutlines == null || energy == null) return;

        BiomeType current = energy.CurrentBiome;

        for (int i = 0; i < slotOutlines.Length; i++)
        {
            if (i >= inventory.ActiveUpgrades.Count)
            {
                slotOutlines[i].effectColor = emptyColor;
                continue;
            }

            float mult = GetMultiplierForBiome(inventory.ActiveUpgrades[i], current);
            if (mult < 1f)
                slotOutlines[i].effectColor = benefitColor;
            else if (mult > 1f)
                slotOutlines[i].effectColor = penaltyColor;
            else
                slotOutlines[i].effectColor = neutralColor;
        }
    }

    float GetMultiplierForBiome(UpgradeConfig upgrade, BiomeType biome)
    {
        foreach (var mod in upgrade.modifiers)
            if (mod.biomeType == biome)
                return mod.energyMultiplier;
        return 1f;
    }

    public void ShowTooltip(int index)
    {
        if (tooltipPanel == null || tooltipText == null) return;
        if (index >= inventory.ActiveUpgrades.Count) return;

        var upgrade = inventory.ActiveUpgrades[index];
        string name = !string.IsNullOrEmpty(upgrade.upgradeName) ? upgrade.upgradeName : upgrade.name;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"<b>{name}</b>");

        foreach (var mod in upgrade.modifiers)
        {
            int biomeIdx = (int)mod.biomeType;
            string biomeName = biomeIdx < BiomeNames.Length ? BiomeNames[biomeIdx] : mod.biomeType.ToString();

            string color;
            string sign;
            int percent = Mathf.RoundToInt((mod.energyMultiplier - 1f) * 100f);

            if (mod.energyMultiplier < 1f)
            {
                color = "#33EE55";
                sign = "";
            }
            else if (mod.energyMultiplier > 1f)
            {
                color = "#EE3333";
                sign = "+";
            }
            else
            {
                color = "#AAAAAA";
                sign = "";
            }

            sb.AppendLine($"{biomeName}: <color={color}>{sign}{percent}%</color>");
        }

        tooltipText.text = sb.ToString().TrimEnd();
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void RemoveAt(int index)
    {
        if (index < inventory.ActiveUpgrades.Count)
            inventory.RemoveUpgrade(inventory.ActiveUpgrades[index]);
    }
}

public class SlotHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UpgradeSlotUI slotUI;
    private int slotIndex;

    public void Init(UpgradeSlotUI ui, int index)
    {
        slotUI = ui;
        slotIndex = index;
    }

    public void OnPointerEnter(PointerEventData eventData) => slotUI.ShowTooltip(slotIndex);
    public void OnPointerExit(PointerEventData eventData) => slotUI.HideTooltip();
}
