using System.Collections.Generic;

using UnityEngine;

public class VoronoiController : MonoBehaviour
{
    [SerializeField] private bool drawSegments = false;

    private List<Limit> limits = null;
    private List<Sector> sectors = null;
    List<Mine> workdMines = new List<Mine>();

    private void OnDrawGizmos()
    {
        Draw();
    }

    public void Awake()
    {
        sectors = new List<Sector>();

        InitLimits();
    }

    public void SetVoronoi(List<Mine> mines)
    {
        sectors.Clear();
        if (mines.Count == 0) return;

        for (int i = 0; i < mines.Count; i++)
        {
            sectors.Add(new Sector(mines[i]));
        }

        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].AddSegmentLimits(limits);
        }

        for (int i = 0; i < mines.Count; i++)
        {
            for (int j = 0; j < mines.Count; j++)
            {
                if (i == j) continue;

                sectors[i].AddSegment(new Vector2(mines[i].transform.position.x, mines[i].transform.position.z), new Vector2(mines[j].transform.position.x, mines[j].transform.position.z));
            }
        }

        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].SetIntersections();
        }
    }

    public Mine GetMineCloser(Vector3 minerPos)
    {
        if (sectors != null)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                if (sectors[i].CheckPointInSector(minerPos))
                {
                    return sectors[i].Mine;
                }
            }
        }

        return null;
    }

    public Mine GetWorkedMineCloser(Vector3 carriagePos, List<Mine> mines, bool setVoronoi)
    {
        if (setVoronoi)
        {
            workdMines = new List<Mine>();

            foreach (Mine mine in mines)
            {
                if (mine.Worked)
                {
                    workdMines.Add(mine);
                }
            }

            SetVoronoi(workdMines);
        }

        if (sectors != null)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                if (sectors[i].CheckPointInSector(carriagePos))
                {
                    return sectors[i].Mine;
                }
            }
        }

        return null;
    }

    private void InitLimits()
    {
        limits = new List<Limit>();


        limits.Add(new Limit(new Vector2(0,0),DIRECTION.LEFT));
        limits.Add(new Limit(new Vector2(0f, AdminOfGame.GetMap().SizeY * AdminOfGame.GetMap().SpaceBetweenY / 2f) , DIRECTION.UP));
        limits.Add(new Limit(new Vector2(AdminOfGame.GetMap().SizeX * AdminOfGame.GetMap().SpaceBetweenX / 2f, AdminOfGame.GetMap().SizeY * AdminOfGame.GetMap().SpaceBetweenY / 2f) , DIRECTION.RIGHT));
        limits.Add(new Limit(new Vector2(AdminOfGame.GetMap().SizeX * AdminOfGame.GetMap().SpaceBetweenX / 2f, 0f) , DIRECTION.DOWN));
    }
    

    private void Draw()
    {
        if (sectors == null) return;

        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].DrawSector();

            if (drawSegments)
            {
                sectors[i].DrawSegments();
            }
        }
    }
}