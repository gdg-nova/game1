// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("Connect to Photon Network: \n" +
		"Connect to the configured Photon server: Reads NetworkingPeer.serverSettingsAssetPath and connects to cloud or your own server. \n" +
		"Uses: Connect(string serverAddress, int port, string uniqueGameID)")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W902")]
	public class PhotonNetworkConnectUsingSettings : FsmStateAction
	{
		[Tooltip("The gameVersion")]
		public FsmString gameVersion;
		
		[Tooltip("If True, will check the cloud for the best server to connect the user. If False, will connect using the server addressed define in the settings.")]
		public FsmBool connectToBestServer;
		
		
		public override void Reset()
		{
			gameVersion  = "1.0";
			connectToBestServer = true;
		}

		public override void OnEnter()
		{
			if (connectToBestServer.Value)
			{
				#if !(UNITY_WINRT || UNITY_WP8 || UNITY_PS3 || UNITY_WIIU)
					PhotonNetwork.ConnectToBestCloudServer(gameVersion.Value);
				#else
					PhotonNetwork.ConnectUsingSettings(gameVersion.Value);
				#endif
			}else{
				PhotonNetwork.ConnectUsingSettings(gameVersion.Value);
			}
			
			// reset authentication failure properties.
			PlayMakerPhotonProxy.lastAuthenticationDebugMessage = string.Empty;
			PlayMakerPhotonProxy.lastAuthenticationFailed=false;

			Finish();
		}
		
		public override string ErrorCheck()
		{
			if (connectToBestServer.Value)
			{
				#if !(UNITY_WINRT || UNITY_WP8 || UNITY_PS3 || UNITY_WIIU)
					return "";
				#else
					return "Connect to Best Server is not available on this platform, the normal connection protocol will be used instead.";
				#endif	
			}
			return "";
		}

	}
}