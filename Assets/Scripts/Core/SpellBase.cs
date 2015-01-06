using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SpellBase : MonoBehaviour {

	public enum eSpellType
	{
		kCreateMonster = 0,
		kMagic = 1,
	};
	
	public class SpellActivatedEventArgs : System.EventArgs
	{
		public SpellActivatedEventArgs()
		{
		}
	}

	public delegate void SpellActivatedHandler(SpellBase spell, SpellActivatedEventArgs e);

	public event SpellActivatedHandler SpellActivated;

	protected void OnSpellActivated(SpellActivatedEventArgs e)
	{
		if (SpellActivated != null)
		{
			SpellActivated(this, e);
		}
	}

	// This prefab is instantiated when the spell is available. It should be a "Toggle" button
	// to allow the spell controller to specify the appropriate details.
	public Toggle guiToggleButtonPrefab = null;

	public void SetToggleState(bool toggleState)
	{
		//Debug.Log ("State is " + toggleState);
		if (toggleState)
		{
			OnSpellActivated(new SpellActivatedEventArgs());
		}
	}

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
