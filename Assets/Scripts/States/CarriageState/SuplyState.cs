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
            Transform transform = stateParameters.GetTransform(0);
            float speed = stateParameters.GetFloat(1);
            GameObject target = stateParameters.GetGameObject(2);
            int food = stateParameters.GetInt(3);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);
            VoronoiController voronoi = stateParameters.GetVoronoi(6);

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (target == null)
                {
                    Mine workedMine = voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable, false);
                    if (workedMine != null)
                    {
                        target = workedMine.gameObject;
                        stateParameters.SetGameObject(2, target);
                    }
                }
                else
                {
                    Mine mine = MapGenerator.Instance.GetMostWorkedMine();
                    if(mine.Mined > 0)
                    {
                        target = mine.gameObject;
                        stateParameters.SetGameObject(2, target);
                    }
                }
                

                if (VillagerAdmin.Instance.Emergency || VoronoiController.workdMines.Count <= 0 || target == null)
                {
                    Transition((int)Flags.OnNeedToReturnHome);
                    return;
                }

                if (travelPositions == null || travelPositions.Count <= 0)
                {
                    travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.CARRIAGE);
                    stateParameters.SetVectorList(5, travelPositions);
                }

                if (travelPositions.Count > 0)
                    transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, target.transform.position) < 1.1f)
                {
                    Mine mine = target.GetComponent<Mine>();
                    mine.SuplyFood(food);
                    stateParameters.SetInt(3, 0);
                    stateParameters.SetInt(7, mine.GetMinedResources());
                    Transition((int)Flags.OnNeedToReturnHome);
                }

                if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.SetVectorList(5, travelPositions);
                }
            });

            behaviours.Add(() => Debug.Log("SUPPLY"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            GameObject target = stateParameters.GetGameObject(2);
            VoronoiController voronoi = stateParameters.GetVoronoi(6);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (target == null)
                {
                    Mine workedMine = voronoi.GetWorkedMineCloser(transform.position, MapGenerator.Instance.MinesAvailable, false);
                    if (workedMine != null)
                    {
                        target = workedMine.gameObject;
                        stateParameters.SetGameObject(2, target);
                    }
                }

                if (target == null)
                {
                    return;
                }

                travelPositions = PathFinder.FindPath(transform.position, target.transform.position, PawnType.CARRIAGE);
                stateParameters.SetVectorList(5, travelPositions);
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
