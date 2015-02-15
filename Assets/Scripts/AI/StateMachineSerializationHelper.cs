using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class StateMachineSerializationHelper
{
	static XmlSerializer ms_stateMachineSerializer = null;

	// Keep a static map of files, this avoids deserializing every time.
	// Note: StateMachine should support Clone() operation
	static Dictionary<string, StateMachine> m_mapCachedStateMachines = new Dictionary<string, StateMachine>();

    static StateMachineSerializationHelper()
	{
	    ms_stateMachineSerializer = new XmlSerializer(typeof(StateMachine));
	}

	static public StateMachine FromFile(string filePath)
	{
	    if (ms_stateMachineSerializer == null) return null;

	    if (m_mapCachedStateMachines.ContainsKey(filePath))
	    {
	        return m_mapCachedStateMachines[filePath].Clone() as StateMachine;
	    }

	    using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
	    {
	        StateMachine sm = (StateMachine)ms_stateMachineSerializer.Deserialize(sr);
	        m_mapCachedStateMachines.Add(filePath, sm);
	        return sm.Clone() as StateMachine;
	    }
	}

    static public StateMachine FromUnityResources(string pathInResourceFolder)
    {
        if (ms_stateMachineSerializer == null) return null;

        if (m_mapCachedStateMachines.ContainsKey(pathInResourceFolder))
        {
            return m_mapCachedStateMachines[pathInResourceFolder].Clone() as StateMachine;
        }

        TextAsset textAsset = (TextAsset)Resources.Load(pathInResourceFolder);  
        using (System.IO.StringReader sr = new System.IO.StringReader(textAsset.text))
        {

            StateMachine sm = (StateMachine)ms_stateMachineSerializer.Deserialize(sr);
            m_mapCachedStateMachines.Add(pathInResourceFolder, sm);
            return sm.Clone() as StateMachine;
        }
    }

    static public StateMachine FromStringData(string uniqueIdentifiedForStatemachine, string xmlDataForStatemachine)
    {
        if (ms_stateMachineSerializer == null) return null;

        if (m_mapCachedStateMachines.ContainsKey(uniqueIdentifiedForStatemachine))
        {
            return m_mapCachedStateMachines[uniqueIdentifiedForStatemachine].Clone() as StateMachine;
        }

        using (System.IO.StringReader sr = new System.IO.StringReader(xmlDataForStatemachine))
        {
            StateMachine sm = (StateMachine)ms_stateMachineSerializer.Deserialize(sr);
            m_mapCachedStateMachines.Add(uniqueIdentifiedForStatemachine, sm);
            return sm.Clone() as StateMachine;
        }
    }
}
