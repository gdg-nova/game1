using UnityEngine;
using System.Collections;

public class PlayerSettings : MonoBehaviour {
	
	public SpellBase[] AvailableSpells;

	public SpellBase ActiveSpell { get; set; }

	void Start()
	{
		if (AvailableSpells.Length > 0)
		{
			ActiveSpell = AvailableSpells[0];
		}
		else
		{
			ActiveSpell = new SpellBase();
		}
	}
}
