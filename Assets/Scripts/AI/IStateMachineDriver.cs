using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IStateMachineDriver
{
	/// <summary>
	/// Call this to perform a single operation on the statemachine.
	/// </summary>
	void Tick();

	/// <summary>
	/// Adds an action to the action queue.
	/// </summary>
	/// <param name="action">The action to add</param>
	void AddAction(string action);

	/// <summary>
	/// Add an action that may have arbitrary parameters associated with it.
	/// </summary>
	/// <param name="action">The action to apply</param>
	/// <param name="args">The arguments associated with action (action specific)</param>
	void AddAction(string action, params object[] args);

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
