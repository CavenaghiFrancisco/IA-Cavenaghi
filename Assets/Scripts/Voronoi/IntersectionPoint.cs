using UnityEngine;

public class IntersectionPoint
{
    private Vector2 position = Vector2.zero;
    private float angle = 0f;

    public Vector2 Position { get => position; }
    public float Angle { get => angle; set => angle = value; }

    public IntersectionPoint(Vector2 position)
    {
        this.position = position;

        angle = 0f;
    }
}
