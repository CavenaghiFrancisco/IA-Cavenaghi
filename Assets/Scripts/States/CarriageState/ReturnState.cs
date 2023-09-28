using IA.FSM.Carriage;
using MinerSimulator.Admins;
using MinerSimulator.Utils.Pathfinder;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Carriage
{
    public class ReturnState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            float food = Convert.ToSingle(stateParameters.Parameters[3]);
            GameObject home = stateParameters.Parameters[4] as GameObject;
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;
            VoronoiController voronoi = stateParameters.Parameters[6] as VoronoiController;

            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                if(travelPositions.Count <= 0 || travelPositions == null)
                {
                    travelPositions = PathFinder.FindPath(transform.position, home.transform.position, PawnType.CARRIAGE);
                    stateParameters.Parameters[5] = travelPositions;
                }

                if (travelPositions.Count > 0)
                transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if(food > 0 && VoronoiController.workdMines.Count > 0 && !VillagerAdmin.Emergency)
                {
                    Transition((int)Flags.OnSuplyMode);
                }

                if (Vector3.Distance(transform.position, home.transform.position) < 1.1f)
                {
                    food = 10;
                    stateParameters.Parameters[3] = food;
                    if (VoronoiController.workdMines.Count <= 0 || VillagerAdmin.Emergency)
                    {
                        if(VillagerAdmin.Emergency)
                            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                        return;
                    }
                    transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    if(VoronoiController.workdMines.Count > 0)
                    Transition((int)Flags.OnSuplyMode);
                }
                if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.Parameters[5] = travelPositions;
                }
            }
            );

            behabiours.Add(() => Debug.Log("RETURN"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            GameObject home = stateParameters.Parameters[4] as GameObject;
            Transform transform = stateParameters.Parameters[0] as Transform;
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;

            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                travelPositions = PathFinder.FindPath(transform.position, home.transform.position, PawnType.CARRIAGE);
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