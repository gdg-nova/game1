using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

public class State : ICloneable
{
	[XmlAttribute(AttributeName="id")]
	public string id { get; set; }

	List<Transition> m_transitions = new List<Transition>();

	[XmlElement(ElementName="transition")]
	public List<Transition> transitions { get { return m_transitions; } }

	public object Clone()
	{
	    State s = new State();
	    s.id = this.id;
	    foreach(Transition t in transitions)
	    {
	        s.transitions.Add((Transition)t.Clone());
	    }
	    return s;
	}
}
