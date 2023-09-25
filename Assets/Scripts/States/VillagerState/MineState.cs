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
            int mined = Convert.ToInt32(stateParameters.Parameters[7]);


            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                if(mined < 3)
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
                    stateParameters.Parameters[7] = mined;
                    stateParameters.Parameters[3] = resources;
                    if (resources >= 15)
                        Transition((int)Flags.OnHaveEnoughResources);
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
