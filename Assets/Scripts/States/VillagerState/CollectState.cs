using IA.FSM.Entities.Villager;
using MinerSimulator.Admins;
using MinerSimulator.Map;
using MinerSimulator.Utils.Pathfinder;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Villager
{
    public class CollectState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            float speed = stateParameters.GetFloat(1);
            GameObject target = stateParameters.GetGameObject(2);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);
            VoronoiController voronoi = stateParameters.GetVoronoi(7);

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (MapGenerator.Instance.MinesAvailable.Count <= 0 || VillagerAdmin.Instance.Emergency)
                {
                    Transition((int)Flags.OnEmergency);
                    return;
                }

                if (target == null)
                {
                    voronoi.SetVoronoi(MapGenerator.Instance.MinesAvailable);
                    target = voronoi.GetMineCloser(transform.position).transform.gameObject;
                    stateParameters.SetGameObject(2, target);
                    travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.VILLAGER);
                    stateParameters.SetVectorList(5, travelPositions);
                }

                if (travelPositions == null || travelPositions.Count <= 0)
                {
                    travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.VILLAGER);
                    stateParameters.SetVectorList(5, travelPositions);
                }

                if (travelPositions.Count > 0)
                    transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
                {
                    Transition((int)Flags.OnNearTarget);
                }

                if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.SetVectorList(5, travelPositions);
                }
            });

            behaviours.Add(() => Debug.Log("COLLECT"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            GameObject target = stateParameters.GetGameObject(2);
            VoronoiController voronoi = stateParameters.GetVoronoi(7);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                if (target == null && MapGenerator.Instance.MinesAvailable.Count > 0)
                {
                    voronoi.SetVoronoi(MapGenerator.Instance.MinesAvailable);
                    target = voronoi.GetMineCloser(transform.position).transform.gameObject;
                    stateParameters.SetGameObject(2, target);
                    travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.VILLAGER);
                    stateParameters.SetVectorList(5, travelPositions);
                }

                if (target != null)
                    stateParameters.SetVectorList(5, PathFinder.FindPath(transform.position, target.transform.position, PawnType.VILLAGER));
            });

            return behaviours;
        }

        public override List<Action> GetOnExitBehaviours(StateParameters stateParameters)
        {
            return new List<Action>();
        }

        public override void Transition(int flag)
        {
            SetFlag?.Invoke(flag);
        }
    }
}
