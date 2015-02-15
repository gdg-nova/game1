using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

[XmlRoot(ElementName="statemachine")]
public class StateMachine : ICloneable
{
	List<State> m_states = new List<State>();

	[XmlAttribute(AttributeName = "initialstate")]
	public string initialState { get; set; }

	[XmlElement(ElementName="state")]
	public List<State> states { get { return m_states; } }

	public object Clone()
	{
	    StateMachine sm = new StateMachine();
	    sm.initialState = this.initialState;
	    foreach(State s in this.states)
	    {
	        sm.states.Add((State)s.Clone());
	    }
	    return sm;
	}
}
