using UnityEngine;
using System;
using System.Collections;

public class PlayerSettings : MonoBehaviour {
	
	public SpellBase[] AvailableSpells;
	public float	   defaultMana = 3.0f;

	public PlayerSettings()
	{
		playerMana = defaultMana;
	}

	public SpellBase ActiveSpell { get; set; }

	private int playerScore = 0;
	public int PlayerScore
	{
		get { return playerScore; }
		set { playerScore = value; }
	}

	private float playerMana = 3.0f;
	public float PlayerMana
	{
		get { return playerMana; }
		set { playerMana = value; }
	}
	
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
