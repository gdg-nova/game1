using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

public class Transition : ICloneable
{
	[XmlAttribute(AttributeName="action")]
	public string action { get; set; }

	[XmlAttribute(AttributeName="weight")]
	public double weight { get; set; }

	[XmlAttribute(AttributeName = "nextstate")]
	public string stateId { get; set; }

	public object Clone()
	{
	    Transition t = new Transition();
	    t.action = this.action;
	    t.weight = this.weight;
	    t.stateId = this.stateId;
	    return t;
	}
}
