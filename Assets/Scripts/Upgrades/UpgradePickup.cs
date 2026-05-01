using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public UpgradeConfig upgradeConfig;
    public float pickupRadius = 2f;

    private float baseY;
    private static Transform robot;
    private static UpgradeInventory inventory;

    void Start()
    {
        baseY = transform.position.y;

        if (robot == null)
        {
            var ctrl = FindFirstObjectByType<RobotController>();
            if (ctrl != null)
            {
                robot = ctrl.transform;
                inventory = ctrl.GetComponent<UpgradeInventory>();
            }
        }
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

        if (robot == null || inventory == null) return;

        float dist = Vector3.Distance(transform.position, robot.position);
        if (dist < pickupRadius && inventory.TryAddUpgrade(upgradeConfig))
        {
            Destroy(gameObject);
        }
    }
}
