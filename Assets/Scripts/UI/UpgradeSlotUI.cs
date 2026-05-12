using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSlotUI : MonoBehaviour
{
    public Image[] slotIcons;
    public TMP_Text[] slotLabels;
    public Button[] removeButtons;
    public Color filledColor = new Color(0.2f, 0.8f, 0.4f, 1f);
    public Color emptyColor = new Color(1, 1, 1, 0.2f);

    private UpgradeInventory inventory;

    void Start()
    {
        inventory = FindFirstObjectByType<UpgradeInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("UpgradeSlotUI: UpgradeInventory not found");
            return;
        }
        inventory.OnInventoryChanged += Refresh;

        for (int i = 0; i < slotIcons.Length; i++)
            slotIcons[i].raycastTarget = false;
        for (int i = 0; i < slotLabels.Length; i++)
            if (slotLabels[i] != null)
                slotLabels[i].raycastTarget = false;

        for (int i = 0; i < removeButtons.Length; i++)
        {
            int index = i;
            removeButtons[i].onClick.AddListener(() => RemoveAt(index));
        }

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
                    slotIcons[i].color = filledColor;
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
            }
        }
    }

    void RemoveAt(int index)
    {
        if (index < inventory.ActiveUpgrades.Count)
        {
            inventory.RemoveUpgrade(inventory.ActiveUpgrades[index]);
        }
    }
}
