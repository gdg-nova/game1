using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IStateImplementation
{
	void OnStateEntry(string entryAction, object[] parameters);

	void Tick();

	void OnStateExit(string exitAction, object[] parameters);
}
