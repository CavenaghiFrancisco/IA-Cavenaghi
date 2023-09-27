using MinerSimulator.Utils.Pathfinder;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Villager
{
    public class CollectState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;
            VoronoiController voronoi = stateParameters.Parameters[7] as VoronoiController;

            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                if (AdminOfGame.GetMap().MinesAvailable.Count <= 0 || VillagerAdmin.Emergency)
                {
                    Transition((int)Flags.OnEmergency);
                    return;
                }

                if (Target == null)
                {
                    voronoi.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                    Target = voronoi.GetMineCloser(transform.position).transform.gameObject;
                    stateParameters.Parameters[2] = Target;
                    travelPositions = new List<Vector3>();
                    travelPositions = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.VILLAGER);
                    stateParameters.Parameters[5] = travelPositions;
                }
                if (travelPositions == null || travelPositions.Count <= 0)
                {
                    travelPositions = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.VILLAGER);
                    stateParameters.Parameters[5] = travelPositions;
                }
                transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, Target.transform.position) < 0.5f)
                {
                    Transition((int)Flags.OnNearTarget);
                }
                if (Vector3.Distance(transform.position, travelPositions[0]) < 0.1f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.Parameters[5] = travelPositions;
                }
                
            });

            behabiours.Add(() => Debug.Log("COLLECT"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            VoronoiController voronoi = stateParameters.Parameters[7] as VoronoiController;
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;


            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                if (Target == null && AdminOfGame.GetMap().MinesAvailable.Count > 0)
                {
                    voronoi.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                    Target = voronoi.GetMineCloser(transform.position).transform.gameObject;
                    stateParameters.Parameters[2] = Target;
                    travelPositions = new List<Vector3>();
                    travelPositions = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.VILLAGER);
                    stateParameters.Parameters[5] = travelPositions;
                }
                if(Target != null)
                stateParameters.Parameters[5] = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.VILLAGER);
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
