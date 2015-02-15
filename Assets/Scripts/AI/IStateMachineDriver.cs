using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IStateMachineDriver
{
	/// <summary>
	/// Adds a state implementation (duplicate entries are overriden)
	/// </summary>
	/// <param name="stateName">State name.</param>
	/// <param name="implementation">Implementation.</param>
	void AddStateImplementation(string stateName, IStateImplementation implementation);

	/// <summary>
	/// Overrides the initial state from the state machine definition with the value here.
	/// </summary>
	/// <param name="initialStateName">Initial state name.</param>
	void OverrideInitialState(string initialStateName);

	/// <summary>
	/// Adds an action to the action queue.
	/// </summary>
	/// <param name="action">The action to add</param>
	void AddAction(string action);

	/// <summary>
	/// Adds an action to the action queue which is delayed by the specified time.
	/// </summary>
	/// <param name="action">The action to add</param>
	/// <param name="delayTime">The time at which to insert it</param>
	void AddDelayedAction(string action, float delayTime);

	/// <summary>
	/// Add an action that may have arbitrary parameters associated with it.
	/// </summary>
	/// <param name="action">The action to apply</param>
	/// <param name="args">The arguments associated with action (action specific)</param>
	void AddAction(string action, params object[] args);
	
	/// <summary>
	/// Adds an action to the action queue which is delayed by the specified time.
	/// </summary>
	/// <param name="action">The action to add</param>
	/// <param name="delayTime">The time at which to insert it</param>
	void AddDelayedAction(string action, float delayTime, params object[] args);

	/// <summary>
	/// Adds a priority action (at the head of the queue but behind other priority actions)
	/// </summary>
	/// <param name="action">The action type of add</param>
	void AddPriorityAction(string action);

	/// <summary>
	/// Adds a priority action (at the head of the queue but behind other priority actions)
	/// </summary>
	/// <param name="action">The action to apply</param>
	/// <param name="args">The arguments associated with action (action specific)</param>
	void AddPriorityAction(string action, params object[] args);

	/// <summary>
	/// Retrieve the current state of the State Machine
	/// </summary>
	State currentState { get; }
}
