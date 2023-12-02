using IA.FSM.Entities.Carriage;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Map;
using MinerSimulator.Utils.Pathfinder;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Carriage
{
    public class SuplyState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            int food = Convert.ToInt32(stateParameters.Parameters[3]);
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;
            VoronoiController voronoi = stateParameters.Parameters[6] as VoronoiController;

            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                if (Target == null)
                {
                    if (voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable,true) != null)
                    {
                        Target = voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable,false).transform.gameObject;
                        stateParameters.Parameters[2] = Target;
                    }
                }
                if (VillagerAdmin.Instance.Emergency || VoronoiController.workdMines.Count <= 0)
                {
                    Transition((int)Flags.OnNeedToReturnHome);
                    return;
                }
                if(Target == null)
                {
                    return;
                }
                if (travelPositions == null || travelPositions.Count <= 0)
                {
                    stateParameters.Parameters[5] = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.CARRIAGE);
                    travelPositions = stateParameters.Parameters[5] as List<Vector3>;
                }

                if (travelPositions.Count > 0)
                    transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, Target.transform.position) < 1.1f)
                {
                    Target.GetComponent<Mine>().SuplyFood(food);
                    stateParameters.Parameters[3] = 0;
                    Transition((int)Flags.OnNeedToReturnHome);
                }
                if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.Parameters[5] = travelPositions;
                }

            });

            behabiours.Add(() => Debug.Log("SUPLY"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;
            VoronoiController voronoi = stateParameters.Parameters[6] as VoronoiController;
            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (Target == null)
                {
                    if (voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable,true) != null)
                    {
                        Target = voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable,false).transform.gameObject;
                    }
                }
                if (Target == null)
                {
                    return;
                }
                travelPositions = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.CARRIAGE);
                stateParameters.Parameters[5] = travelPositions;
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
