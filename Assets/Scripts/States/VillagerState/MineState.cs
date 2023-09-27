using MinerSimulator.Utils.Pathfinder;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IA.FSM.Villager
{
    public class MineState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            float resources = Convert.ToSingle(stateParameters.Parameters[3]);
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;
            int mined = Convert.ToInt32(stateParameters.Parameters[6]);
            VoronoiController voronoi = stateParameters.Parameters[7] as VoronoiController;


            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                if (resources >= 15 || AdminOfGame.GetMap().MinesAvailable.Count <= 0 || VillagerAdmin.Emergency)
                {
                    Transition((int)Flags.OnHaveEnoughResources);
                    return;
                }
                    
                if (Target == null)
                {
                    voronoi.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                    Target = voronoi.GetMineCloser(transform.position).transform.gameObject;
                    stateParameters.Parameters[2] = Target;
                    travelPositions.Clear();
                    travelPositions = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.VILLAGER);
                    stateParameters.Parameters[5] = travelPositions;
                }
                if(Vector3.Distance(transform.position, Target.transform.position) > 0.6f)
                {
                    Transition((int)Flags.OnMineDestroyed);
                }
                if (mined < 3 && Vector3.Distance(transform.position, Target.transform.position) < 0.5f)
                {
                    resources += Target.GetComponent<Mine>().Take(1);
                    mined += 1;
                    if (mined >= 3)
                    {
                        bool aux = Target.GetComponent<Mine>().CanTakeFood();
                        if (aux)
                        {
                            mined = 0;
                        }
                    }
                    stateParameters.Parameters[6] = mined;
                    stateParameters.Parameters[3] = resources;
                    
                }
                
            });
            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters parameters)
        {
            return new List<Action>();
        }

        public override List<Action> GetOnExitBehaviours(StateParameters parameters)
        {
            return new List<Action>();
        }

        public override void Transition(int flag)
        {
            SetFlag?.Invoke(flag);

        }
    }
}
