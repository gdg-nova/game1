using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineDriver : MonoBehaviour, IStateMachineDriver
{
	/// <summary>
	/// The definition of the state machine for this component as an XML file.
	/// </summary>
	public TextAsset stateMachineDefinitionAsXmlFile;

	public bool isDead { get; set; }

	class ActionTime
	{
		public string action;
		public float time;
		public object[] parameters;

		public ActionTime(string action, float time, object[] parameters)
		{
			this.action = action;
			this.time = time;
			this.parameters = parameters;
		}

		public Action AsAction()
		{
			return new Action( action, parameters );
		}
	}

	List<ActionTime> m_pendingActions = new List<ActionTime>();

	StateMachine m_stateMachine = null;
	State m_currentState = null;
    IStateImplementation m_currentImpl = null;
	bool m_initializationCompleted = false;

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

	// Internal maps which are populated before the state machine starts ticking.
	Dictionary<State, IStateImplementation> m_mapStateToResponder = new Dictionary<State, IStateImplementation>();
	Dictionary<string, State> m_mapNameToState = new Dictionary<string,State>();

	// This should be pre-populated with configuration information.
	Dictionary<string, IStateImplementation> m_mapStateImplementations = new Dictionary<string, IStateImplementation>();

	// This should be pre-populated with the initial state (if different from what is in the state machine definition)
	string m_overridenInitialState = null;

	private void AddActionTime(string action, float timeDelta, object[] parameters)
	{
		float time = Time.time + timeDelta;
		ActionTime at = new ActionTime(action, time, parameters);

		if (m_pendingActions.Count == 0)
		{
			m_pendingActions.Add(at);
		}
		else
		{
			int i = 0;
			for(; i < m_pendingActions.Count; i++)
			{
				if (m_pendingActions[i].time > time)
					break;
			}
			m_pendingActions.Insert(i, at);
		}
	}

	/// <summary>
	/// Read the State Machine definition from storage.
	/// </summary>
	void InitializeSM()
	{
		if (stateMachineDefinitionAsXmlFile == null)
		{
			Debug.LogError ("State Machine not defined for " + this.name);
		}
		m_stateMachine = StateMachineSerializationHelper.FromStringData(
			stateMachineDefinitionAsXmlFile.name, 
			stateMachineDefinitionAsXmlFile.text);
	}

	/// <summary>
	/// This should be called after all initialization is completed. This generates all the internal
	/// maps which should be done after call configuration is completed.
	/// </summary>
	void InitializeSMMaps()
	{
		//Debug.Log ("Count of implemented states: " + m_mapStateImplementations.Count);
		foreach(string stateId in m_mapStateImplementations.Keys)
		{
			State state = m_stateMachine.states.Find(s => s.id == stateId);
			if (state == null)
			{
				throw new ArgumentException(String.Format("State {0} not found in StateMachine definition", stateId));
			}
			m_mapStateToResponder.Add(state, m_mapStateImplementations[stateId]);
		}
		foreach (State state in m_stateMachine.states)
		{
			m_mapNameToState.Add(state.id, state);
		}
	}

	/// <summary>
	/// Updates the initial state based on what has been selected.
	/// </summary>
	void InitializeInitialState()
	{
		if (m_overridenInitialState != null)
		{
			if (m_mapNameToState.ContainsKey(m_overridenInitialState))
			{
				currentState = m_mapNameToState[m_overridenInitialState];
			}
			else
			{
				Debug.LogError("Initial state override not found for " + this.name);
			}
		}
		if (currentState == null)
		{
			if (!m_mapNameToState.ContainsKey(m_stateMachine.initialState))
			{
				throw new Exception("Initial State " + m_stateMachine.initialState + " not found for" + this.name);
			}
			currentState = m_mapNameToState[m_stateMachine.initialState];
		}
		if (currentImplementation != null)
		{
			//Debug.Log ("Initial state entry");
			currentImplementation.OnStateEntry("init", null);
		}
	}

	void Awake()
	{
		InitializeSM();
	}

	void Update()
	{
		if (!m_initializationCompleted)
		{
			InitializeSMMaps();
			InitializeInitialState();
			m_initializationCompleted = true;
		}

		if (m_pendingActions.Count > 0 && m_pendingActions[0].time < Time.time)
		{
			//Debug.Log ("Adding delayed action: " + m_pendingActions[0].action);
			m_normalPriorityactions.Add( m_pendingActions[0].AsAction() );
			m_pendingActions.RemoveAt (0);
		}

		// Tick along...
		Tick ();
	}

	void Tick()
	{
	    if (currentState == null)
	    {
			Debug.LogError("No state for statemachine for obj:" + this.name);
	        return;
	    }

		//Debug.Log ("Current state:" + currentState.id);
	    // 1) Do an update round on the current state
        if (currentImplementation != null)
	        currentImplementation.Tick();
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
	        System.Random rnd = new System.Random();
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

	    try
	    {
	        if (currentImplementation != null) 
	        {
	            currentImplementation.OnStateExit(currentAction.action, currentAction.parameters);
	        }
	    }
	    catch(Exception e)
	    {
			Debug.LogError(e.ToString());
	    }
		//Debug.Log ("New state for: " + this.name + " is: " + newState.id);
	    currentState = newState;
	    try
	    {
	        if (currentImplementation != null)
	        {
                //UnityEngine.Debug.Log("Calling StateEntry for " + currentImplementation.ToString());
	            currentImplementation.OnStateEntry(currentAction.action, currentAction.parameters);
	        }
	    }
	    catch (Exception e)
	    {
			Debug.LogError(e.ToString());
	    }
	    return true;
	}

	public void AddStateImplementation(string stateName, IStateImplementation implementation)
	{
		if (implementation != null && stateName != null)
		{
			if (m_mapStateImplementations.ContainsKey(stateName))
			{
				m_mapStateImplementations[stateName] = implementation;
			}
			else
			{
				m_mapStateImplementations.Add ( stateName, implementation);
			}
		}
	}
	
	public void OverrideInitialState(string initialStateName)
	{
		if (!String.IsNullOrEmpty(initialStateName))
		{
			m_overridenInitialState = initialStateName;
		}
	}

	public void AddAction(string action)
	{
	    m_normalPriorityactions.Add(new Action(action, null));
	}

	public void AddDelayedAction(string action, float timeDelay)
	{
		AddActionTime ( action, timeDelay, null );
	}

	public void AddAction(string action, params object[] args)
	{
	    m_normalPriorityactions.Add(new Action(action, args));
	}

	public void AddDelayedAction(string action, float timeDelay, params object[] args)
	{
		AddActionTime ( action, timeDelay, args );
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
			if (m_currentState.id == "die")
				isDead = true;
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

    private IStateImplementation currentImplementation
    {
        get { return m_currentImpl; }
    }
}
