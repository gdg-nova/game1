using UnityEngine;
using System.Collections;

public interface ICanBeInfluenced {

	/// <summary>
	/// Causes the character to be influenced by a movement indicated (x- and z- axis only).
	/// This isn't a requirement to move in that direction explicitly, some variation is allowed but 
	/// it should "generally" be in that direction and if the character is currently stationary it
	/// should start moving, and if currently seeking a target, it will move in a different direction
	/// (even temporarily avoiding attacking a target).
	/// </summary>
	void BeInfluenced(Vector3 directionOfInfluence);
}
