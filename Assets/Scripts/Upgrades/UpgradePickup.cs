using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public UpgradeConfig upgradeConfig;

    private float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 60f * Time.deltaTime);
        float bob = Mathf.Sin(Time.time * 2f) * 0.3f;
        transform.position = new Vector3(
            transform.position.x,
            baseY + bob,
            transform.position.z
        );
    }

    void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<UpgradeInventory>();
        if (inventory == null) return;

        if (inventory.TryAddUpgrade(upgradeConfig))
        {
            Destroy(gameObject);
        }
    }
}
