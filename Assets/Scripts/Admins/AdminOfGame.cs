using MinerSimulator.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinerSimulator.Admins
{
    public class AdminOfGame : MonoBehaviour
    {
        private static MapGenerator map;

        void Start()
        {
           map = GetComponent<MapGenerator>(); 
        }

        public static MapGenerator GetMap()
        {
            return map;
        }
    }
}
