using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IStateImplementation<Ty>
{
	void OnStateEntry(Ty userObj, string entryAction, object[] parameters);

	void Tick(Ty userObj);

	void OnStateExit(Ty userObj, string exitAction, object[] parameters);
}
