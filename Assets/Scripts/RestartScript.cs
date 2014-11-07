using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class RestartScript : MonoBehaviour
{
	private GUISkin skin;
	public GameObject[] musicSources;
	public GameObject[] soundSources;

	void Start()
	{
		skin = Resources.Load("GUISkin") as GUISkin;

		SetMusicOnOff(globalEvents.CurrentMusicOnOffState);
		SetSoundOnOff(globalEvents.CurrentSoundOnOffState);

		globalEvents.MusicOnOff += (object sender, globalEvents.MusicOrSoundOnOffEventArgs e) => SetMusicOnOff(e.OnOrOff);
		globalEvents.SoundOnOff += (object sender, globalEvents.MusicOrSoundOnOffEventArgs e) => SetSoundOnOff(e.OnOrOff);
	}

	void SetMusicOnOff(bool musicOnOff)
	{
		foreach(GameObject source in musicSources)
		{
			AudioSource[] sources = source.GetComponents<AudioSource>();
			foreach(AudioSource aSource in sources)
			{
				aSource.mute = !musicOnOff;
			}
		}
	}
	
	void SetSoundOnOff(bool soundOnOff)
	{
		foreach(GameObject source in soundSources)
		{
			AudioSource[] sources = source.GetComponents<AudioSource>();
			foreach(AudioSource aSource in sources)
			{
				aSource.mute = !soundOnOff;
			}
		}
	}

	// Removed the Level Select as now part of the camStats U/I
}