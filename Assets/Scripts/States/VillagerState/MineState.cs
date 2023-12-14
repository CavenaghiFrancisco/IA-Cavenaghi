using IA.FSM.Entities.Villager;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Map;
using MinerSimulator.Utils.Pathfinder;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Villager
{
    public class MineState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            float speed = stateParameters.GetFloat(1);
            GameObject target = stateParameters.GetGameObject(2);
            int resources = stateParameters.GetInt(3);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);
            int mined = stateParameters.GetInt(6);
            VoronoiController voronoi = stateParameters.GetVoronoi(7);

            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                Debug.Log(resources);
                if (target != null)
                {
                    if(target.GetComponent<Mine>().CanContinue())
                    {
                        resources = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                    
                if (MapGenerator.Instance.MinesAvailable.Count <= 0 || VillagerAdmin.Instance.Emergency)
                {
                    Transition((int)Flags.OnHaveEnoughResources);
                    return;
                }

                if (target == null)
                {
                    voronoi.SetVoronoi(MapGenerator.Instance.MinesAvailable);
                    target = voronoi.GetMineCloser(transform.position).gameObject;
                    stateParameters.SetGameObject(2, target);
                    travelPositions.Clear();
                    travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.VILLAGER);
                    stateParameters.SetVectorList(5, travelPositions);
                }

                if (Vector3.Distance(transform.position, target.transform.position) > 0.6f)
                {
                    Transition((int)Flags.OnMineDestroyed);
                }

                if (mined < 3 && Vector3.Distance(transform.position, target.transform.position) < 0.5f)
                {
                    resources += target.GetComponent<Mine>().Take(1);
                    mined += 1;

                    if (mined >= 3 && target.GetComponent<Mine>().CanTakeFood())
                    {
                        mined = 0;
                    }

                    stateParameters.SetInt(6, mined);
                    stateParameters.SetInt(3, resources);
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
