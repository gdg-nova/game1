using UnityEngine;
using System.Collections;

public interface ICanBeStruckDown {

	/// <summary>
	/// Causes the character to be struck down temporarily, they may become scared or change behavior after getting up.
	/// </summary>
	void StrikeDown();
}
