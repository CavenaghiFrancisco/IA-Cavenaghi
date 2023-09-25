using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminOfGame : MonoBehaviour
{
    private static MapGenerator map;
    // Start is called before the first frame update
    void Start()
    {
       map = GetComponent<MapGenerator>(); 
    }

    public static MapGenerator GetMap()
    {
        return map;
    }
}
