using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlotUI : MonoBehaviour
{
    public Image[] slotIcons;
    public Button[] removeButtons;

    private UpgradeInventory inventory;

    void Start()
    {
        inventory = FindFirstObjectByType<UpgradeInventory>();
        inventory.OnInventoryChanged += Refresh;

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
                slotIcons[i].sprite = inventory.ActiveUpgrades[i].icon;
                slotIcons[i].color = Color.white;
                removeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = new Color(1, 1, 1, 0.2f);
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
