using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SpellBase : MonoBehaviour {

	public enum eSpellType
	{
		kCreateMonster = 0,
		kMagic = 1,
	};

	// This prefab is instantiated when the spell is available. It should be a "Toggle" button
	// to allow the spell controller to specify the appropriate details.
	public Toggle guiToggleButtonPrefab = null;

	// Returns the type of the spell which can determine how the GUI is rendered
	// or other game logic.
	virtual public eSpellType type { get { return eSpellType.kMagic; } }
	
	// Base class for a "Spell" class which is used to implement a spell
	virtual public void StartTouch(SceneControl.TouchEvent args)
	{
	}
	
	virtual public void MoveTouch(SceneControl.TouchEvent args)
	{
	}
	
	virtual public void EndTouch(SceneControl.TouchEvent args)
	{
	}
	
	virtual public void CancelTouch(SceneControl.TouchEvent args)
	{
	}
	
}
