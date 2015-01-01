using UnityEngine;
using System;
using System.Collections;

public class globalEvents : MonoBehaviour
{

	static public bool	CurrentMusicOnOffState = false;
	static public bool	CurrentSoundOnOffState = false;

	public class MusicOrSoundOnOffEventArgs : System.EventArgs
	{
		public MusicOrSoundOnOffEventArgs (bool onOrOff)
		{
			OnOrOff = onOrOff;
		}
		public bool OnOrOff { get; private set; }
	}

	public delegate void MusicOrSoundOnOffHandler (object sender,MusicOrSoundOnOffEventArgs e);

	static public event MusicOrSoundOnOffHandler MusicOnOff;

	static public void OnMusicOnOff (object sender, bool onOrOff)
	{
		CurrentMusicOnOffState = onOrOff;
		if (MusicOnOff != null) {
			MusicOnOff (sender, new MusicOrSoundOnOffEventArgs (onOrOff));
		}
	}

	static public event MusicOrSoundOnOffHandler SoundOnOff;
	static public void OnSoundOnOff (object sender, bool onOrOff)
	{
		CurrentSoundOnOffState = onOrOff;
		if (SoundOnOff != null) {
			SoundOnOff (sender, new MusicOrSoundOnOffEventArgs (onOrOff));
		}
	}

	public class GameObjectHitEventArgs : EventArgs
	{
		public GameObjectHitEventArgs(GameObject objHit)
		{
			objectHit = objHit;
		}

		public GameObject objectHit { get; set; }
	}
	public delegate void GameObjectHitEventHandler(object sender, GameObjectHitEventArgs e);

	static public event GameObjectHitEventHandler PlayerObjectHit;
	static public void OnPlayerObjectHit( object sender, GameObject playerObjectHit )
	{
		if (PlayerObjectHit != null)
		{
			PlayerObjectHit(sender, new GameObjectHitEventArgs(playerObjectHit) );
		}
	}

	static public event GameObjectHitEventHandler EnemyElementHit;
	static public void OnEnemyElementsHit( object sender, GameObject objectHit )
	{
		if (EnemyElementHit != null)
		{
			EnemyElementHit(sender, new GameObjectHitEventArgs(objectHit) );
		}
	}

}
