using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public float triggerRadius = 5f;

    private Transform robot;
    private GameManager gm;

    void Start()
    {
        var ctrl = FindFirstObjectByType<RobotController>();
        if (ctrl != null) robot = ctrl.transform;
        gm = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (robot == null || gm == null) return;

        float dx = transform.position.x - robot.position.x;
        float dz = transform.position.z - robot.position.z;
        if (dx * dx + dz * dz < triggerRadius * triggerRadius)
            gm.OnWin();
    }
}
