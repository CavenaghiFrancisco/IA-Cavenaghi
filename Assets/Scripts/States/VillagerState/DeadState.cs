using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Villager
{
    public class DeadState : State
    {
        public override List<Action> GetBehaviours(StateParameters parameters)
        {
            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                Debug.Log("F");
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
