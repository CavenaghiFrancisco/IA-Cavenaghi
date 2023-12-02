using UnityEngine;
using System;
using System.Collections.Generic;
using MinerSimulator.Utils.Voronoi;
using IA.FSM.States;

namespace IA.FSM.Entities
{
    public enum States
    {
    }

    public enum Flags
    {
    }

    public abstract class Entity : MonoBehaviour
    {
        protected GameObject target;
        protected GameObject home;
        protected VoronoiController voronoiCalculator;
        protected float speed;
        protected FSM fsm;
        protected List<Vector3> travelPositions;
        protected StateParameters allParameters;

        public GameObject Target { get => target; set => target = value; }
        public GameObject Home { get => home; set => home = value; }
        public float Speed { get => speed; set => speed = value; }

        protected virtual void Start()
        {
            voronoiCalculator = GetComponent<VoronoiController>();
            allParameters = new StateParameters();
            fsm = new FSM(GetEnumLength<States>(), GetEnumLength<Flags>());
        }

        public void Update()
        {
            fsm.Update();
        }

        private int GetEnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}
