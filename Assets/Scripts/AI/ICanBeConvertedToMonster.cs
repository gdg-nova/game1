using UnityEngine;
using System.Collections;

public interface ICanBeConvertedToMonster {

	/// <summary>
	/// Converts the character to a monster. Will take Mana to do so otherwise doesn't do anything.
	/// </summary>
	void ConvertToMonster();
}
