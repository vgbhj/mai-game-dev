using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishArrow : MonoBehaviour
{
    public RectTransform arrowImage;
    public TMP_Text distanceText;
    public Transform robot;
    public Transform finishPoint;

    void Update()
    {
        if (robot == null || finishPoint == null) return;

        Vector3 dir = finishPoint.position - robot.position;
        float dist = dir.magnitude;
        dir.y = 0;

        float angle = -Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        Camera cam = Camera.main;
        if (cam != null)
            angle -= cam.transform.eulerAngles.y;

        arrowImage.localRotation = Quaternion.Euler(0, 0, -angle);
        distanceText.text = $"{Mathf.RoundToInt(dist)}m";
    }
}
