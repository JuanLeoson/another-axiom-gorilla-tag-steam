using System;
using System.Collections.Generic;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000CD0 RID: 3280
	public class StateMachine
	{
		// Token: 0x06005177 RID: 20855 RVA: 0x00196410 File Offset: 0x00194610
		public void Tick()
		{
			StateMachine.Transition transition = this.GetTransition();
			if (transition != null)
			{
				this.SetState(transition.To);
			}
			IState currentState = this._currentState;
			if (currentState == null)
			{
				return;
			}
			currentState.Tick();
		}

		// Token: 0x06005178 RID: 20856 RVA: 0x00196444 File Offset: 0x00194644
		public void SetState(IState state)
		{
			if (state == this._currentState)
			{
				return;
			}
			IState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			this._currentState = state;
			this._transitions.TryGetValue(this._currentState.GetType(), out this._currentTransitions);
			if (this._currentTransitions == null)
			{
				this._currentTransitions = StateMachine.EmptyTransitions;
			}
			this._currentState.OnEnter();
		}

		// Token: 0x06005179 RID: 20857 RVA: 0x001964AE File Offset: 0x001946AE
		public IState GetState()
		{
			return this._currentState;
		}

		// Token: 0x0600517A RID: 20858 RVA: 0x001964B8 File Offset: 0x001946B8
		public void AddTransition(IState from, IState to, Func<bool> predicate)
		{
			List<StateMachine.Transition> list;
			if (!this._transitions.TryGetValue(from.GetType(), out list))
			{
				list = new List<StateMachine.Transition>();
				this._transitions[from.GetType()] = list;
			}
			list.Add(new StateMachine.Transition(to, predicate));
		}

		// Token: 0x0600517B RID: 20859 RVA: 0x001964FF File Offset: 0x001946FF
		public void AddAnyTransition(IState state, Func<bool> predicate)
		{
			this._anyTransitions.Add(new StateMachine.Transition(state, predicate));
		}

		// Token: 0x0600517C RID: 20860 RVA: 0x00196514 File Offset: 0x00194714
		private StateMachine.Transition GetTransition()
		{
			foreach (StateMachine.Transition transition in this._anyTransitions)
			{
				if (transition.Condition())
				{
					return transition;
				}
			}
			foreach (StateMachine.Transition transition2 in this._currentTransitions)
			{
				if (transition2.Condition())
				{
					return transition2;
				}
			}
			return null;
		}

		// Token: 0x04005B03 RID: 23299
		private IState _currentState;

		// Token: 0x04005B04 RID: 23300
		private Dictionary<Type, List<StateMachine.Transition>> _transitions = new Dictionary<Type, List<StateMachine.Transition>>();

		// Token: 0x04005B05 RID: 23301
		private List<StateMachine.Transition> _currentTransitions = new List<StateMachine.Transition>();

		// Token: 0x04005B06 RID: 23302
		private List<StateMachine.Transition> _anyTransitions = new List<StateMachine.Transition>();

		// Token: 0x04005B07 RID: 23303
		private static List<StateMachine.Transition> EmptyTransitions = new List<StateMachine.Transition>(0);

		// Token: 0x02000CD1 RID: 3281
		private class Transition
		{
			// Token: 0x17000795 RID: 1941
			// (get) Token: 0x0600517F RID: 20863 RVA: 0x001965F6 File Offset: 0x001947F6
			public Func<bool> Condition { get; }

			// Token: 0x17000796 RID: 1942
			// (get) Token: 0x06005180 RID: 20864 RVA: 0x001965FE File Offset: 0x001947FE
			public IState To { get; }

			// Token: 0x06005181 RID: 20865 RVA: 0x00196606 File Offset: 0x00194806
			public Transition(IState to, Func<bool> condition)
			{
				this.To = to;
				this.Condition = condition;
			}
		}
	}
}
