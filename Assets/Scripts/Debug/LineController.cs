using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Debug line drawing
public class LineController : MonoBehaviour
{
    public Sprite sprite;
    private LineRenderer lineRenderer;
    private List<Vector2> points;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new List<Vector2>();
    }

    private void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    public void AddLine(List<Vector2> points)
    {
        lineRenderer.positionCount = points.Count;
        this.points = points;
    }

    public void RemoveLine()
    {
        if (points != null && points.Count > 0)
        {
            lineRenderer.positionCount = 0;
            points = new List<Vector2>();
        }
    }
}
