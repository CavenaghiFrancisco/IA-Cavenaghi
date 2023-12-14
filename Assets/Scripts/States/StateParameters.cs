using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States
{
    public class StateParameters
    {
        public object[] Parameters { get; set; }

        public Transform GetTransform(int index)
        {
            return GetParameter<Transform>(index, "Transform");
        }

        public float GetFloat(int index)
        {
            return GetParameter<float>(index, "Float");
        }

        public int GetInt(int index)
        {
            return GetParameter<int>(index, "Int");
        }

        public GameObject GetGameObject(int index)
        {
            return GetParameter<GameObject>(index, "GameObject");
        }

        public List<Vector3> GetVectorList(int index)
        {
            return GetParameter<List<Vector3>>(index, "VectorList");
        }

        public VoronoiController GetVoronoi(int index)
        {
            return GetParameter<VoronoiController>(index, "VoronoiController");
        }

        public T GetParameter<T>(int index, string paramName)
        {
            if (this == null || index < 0 || index >= this.Parameters.Length)
            {
                throw new ArgumentException($"Invalid parameters or index for {paramName}");
            }

            return (T)this.Parameters[index];
        }

        public void SetTransform(int index, Transform value)
        {
            SetParameter(index, value, "Transform");
        }

        public void SetFloat(int index, float value)
        {
            SetParameter(index, value, "Float");
        }

        public void SetInt(int index, int value)
        {
            SetParameter(index, value, "Int");
        }

        public void SetGameObject(int index, GameObject value)
        {
            SetParameter(index, value, "GameObject");
        }

        public void SetVectorList(int index, List<Vector3> value)
        {
            SetParameter(index, value, "VectorList");
        }

        protected void SetVoronoi(int index, VoronoiController value)
        {
            SetParameter(index, value, "VoronoiController");
        }

        private void SetParameter<T>(int index, T value, string paramName)
        {
            if (this == null || index < 0 || index >= this.Parameters.Length)
            {
                throw new ArgumentException($"Invalid parameters or index for {paramName}");
            }

            this.Parameters[index] = value;
        }
    }
}
