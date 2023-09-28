using System;
using System.Collections.Generic;

namespace IA.FSM.States
{
    public abstract class State
    {
        public Action<int> SetFlag;
        public abstract List<Action> GetOnEnterBehaviours(StateParameters parameters);
        public abstract List<Action> GetBehaviours(StateParameters parameters);
        public abstract List<Action> GetOnExitBehaviours(StateParameters parameters);
        public abstract void Transition(int flag);
    }
}