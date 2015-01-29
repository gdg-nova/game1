using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StateMachineDriver<T> : IStateMachineDriver
{
	StateMachine m_stateMachine = null;
	State m_currentState = null;
    IStateImplementation<T> m_currentImpl = null;

	class Action
	{
	    internal Action(string action, object[] parameters)
	    {
	        this.action = action;
	        this.parameters = parameters;
	    }

	    public string action { get; private set; }
	    public object[] parameters { get; private set; }
	}

	private List<Action> m_highPriorityActions = new List<Action>();
	private List<Action> m_normalPriorityactions = new List<Action>();

	Dictionary<State, IStateImplementation<T>> m_mapStateToResponder = new Dictionary<State, IStateImplementation<T>>();
	Dictionary<string, State> m_mapNameToState = new Dictionary<string,State>();

	T m_userObj = default(T);

	public StateMachineDriver(StateMachine sm, Dictionary<string, IStateImplementation<T>> implementations, T userObj)
	{
	    if (sm == null)
	    {
	        throw new ArgumentNullException("StateMachine");
	    }
	    if (implementations == null)
	    {
	        throw new ArgumentNullException("implementations");
	    }
	    m_stateMachine = sm;
	    m_userObj = userObj;
	    foreach(string stateId in implementations.Keys)
	    {
	        State state = sm.states.Find(s => s.id == stateId);
	        if (state == null)
	        {
	            throw new ArgumentException(String.Format("State {0} not found in StateMachine definition", stateId));
	        }
	        m_mapStateToResponder.Add(state, implementations[stateId]);
	    }
        foreach (State state in m_stateMachine.states)
        {
            m_mapNameToState.Add(state.id, state);
        }
        if (!m_mapNameToState.ContainsKey(m_stateMachine.initialState))
        {
            throw new Exception(String.Format("State {0} which is defined as the initial state is not found in the state definition", m_stateMachine.initialState));
        }
        currentState = m_mapNameToState[m_stateMachine.initialState];
	}

	public void Tick()
	{
	    if (currentState == null)
	    {
	        return;
	    }
	    // 1) Do an update round on the current state
        if (currentImplementation != null)
	        currentImplementation.Tick(m_userObj);
	    // 2) Check for high priority transitions out of current state
	    if (!TestStateTransition(m_highPriorityActions))
	    // 3) Check for normal priority transitions out of current state
	        TestStateTransition(m_normalPriorityactions);
	}

	bool TestStateTransition(List<Action> actionsList)
	{
	    int foundActionIndex = 0;
	    List<int> possibleTransitions = null;
	    for(int i = 0; i < actionsList.Count; i++)
	    {
	        Action action = actionsList[i];
	        for(int j = 0; j < currentState.transitions.Count; j++ )
	        {
	            if (currentState.transitions[j].action == action.action)
	            {
	                if (possibleTransitions == null) possibleTransitions = new List<int>(); // Only create the list if we have a possible transition.
	                possibleTransitions.Add(j);
	            }
	        }
	        if (possibleTransitions != null)
	        {
	            foundActionIndex = i;
	            break;
	        }
	    }
	    if (possibleTransitions == null) return false;

	    Action currentAction = actionsList[foundActionIndex];
	    actionsList.RemoveAt(foundActionIndex);

	    Transition selectedTransition = null;
	    if (possibleTransitions.Count > 1)
	    {
	        double total = 0.0;
	        foreach(int i in possibleTransitions)
	        {
	            total += currentState.transitions[possibleTransitions[i]].weight;
	        }
	        Random rnd = new Random();
	        double choice = rnd.NextDouble() * total;
	        foreach(int i in possibleTransitions)
	        {
	            choice -= currentState.transitions[possibleTransitions[i]].weight;
	            if (choice <= 0.00001)
	            {
	                selectedTransition = currentState.transitions[possibleTransitions[i]];
	                break;
	            }
	        }
	    }
	    else
	    {
	        selectedTransition = currentState.transitions[possibleTransitions[0]];
	    }
	    State newState = m_mapNameToState[selectedTransition.stateId];
	    if (newState == currentState) return false;

	    try
	    {
	        if (currentImplementation != null) 
	        {
	            currentImplementation.OnStateExit(m_userObj, currentAction.action, currentAction.parameters);
	        }
	    }
	    catch(Exception)
	    {
	    }
	    currentState = newState;
	    try
	    {
	        if (currentImplementation != null)
	        {
                UnityEngine.Debug.Log("Calling StateEntry for " + currentImplementation.ToString());
	            currentImplementation.OnStateEntry(m_userObj, currentAction.action, currentAction.parameters);
	        }
	    }
	    catch (Exception)
	    {
	    }
	    return true;
	}

	public void AddAction(string action)
	{
	    m_normalPriorityactions.Add(new Action(action, null));
	}

	public void AddAction(string action, params object[] args)
	{
	    m_normalPriorityactions.Add(new Action(action, args));
	}

	public void AddPriorityAction(string action)
	{
	    m_highPriorityActions.Add(new Action(action, null));
	}

	public void AddPriorityAction(string action, params object[] args)
	{
	    m_highPriorityActions.Add(new Action(action, args));
	}

	public State currentState
	{
	    get { return m_currentState; }
	    private set 
        {
            if (m_currentState == value) return;
            m_currentState = value;
            if (m_mapStateToResponder.ContainsKey(m_currentState))
            {
                m_currentImpl = m_mapStateToResponder[currentState];
            }
            else
            {
                m_currentImpl = null;
            }
        }
	}

    private IStateImplementation<T> currentImplementation
    {
        get { return m_currentImpl; }
    }
}
