using UnityEngine;
using System.Collections;

public class globalEvents : MonoBehaviour
{

		static public bool	CurrentMusicOnOffState = true;
		static public bool	CurrentSoundOnOffState = true;

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
}
