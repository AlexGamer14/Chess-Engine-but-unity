using UnityEngine;

public class DrawArrowUI : MonoBehaviour
{
    public RectTransform arrowImage; // Drag your UI Image here
    public RectTransform startPoint; // Start anchor
    public RectTransform endPoint;   // End anchor
    public float thickness = 5f;     // Width of the arrow

    void Update()
    {
        Vector2 start = startPoint.position;
        Vector2 end = endPoint.position;
        Vector2 dir = end - start;
        float distance = dir.magnitude;

        arrowImage.sizeDelta = new Vector2(thickness, distance);
        arrowImage.position = start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowImage.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust based on pivot
    }
}