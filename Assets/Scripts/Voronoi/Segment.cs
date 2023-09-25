using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    private Vector2 origin = Vector2.zero;
    private Vector2 final = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private Vector2 mediatrix = Vector2.zero;

    private List<Vector2> intersections = null;

    public Vector2 Origin { get => origin; }
    public Vector2 Final { get => final; }
    public Vector2 Mediatrix { get => mediatrix; }
    public Vector2 Direction { get => direction; }
    public List<Vector2> Intersections { get => intersections; }

    public Segment(Vector2 origin, Vector2 final)
    {
        this.origin = origin;
        this.final = final;

        mediatrix = new Vector2((origin.x + final.x) / 2, (origin.y + final.y) / 2);
        direction = Vector2.Perpendicular(new Vector2(final.x - origin.x, final.y - origin.y));

        intersections = new List<Vector2>();
    }

    public void Draw()
    {
        Gizmos.DrawLine(origin, final);
    }
}
