using UnityEngine;
using System.Collections;

public class StateImplementationHelper<T> {

	public StateImplementationHelper(T userObj)
	{
		m_userObj = userObj;
	}

	protected T m_userObj;
}
