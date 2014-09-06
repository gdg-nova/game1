// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("Defines Authentication values to use for connection ( using PhotonNetworkConnectUsingSettings or PhotonNetworkConnectManually).\n" +
		"Failed Custom Authentication will fire a global Photon event 'CUSTOM AUTHENTICATION FAILED' event.")]
	//[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W922")]
	public class PhotonNetworkSetAuthenticationValues : FsmStateAction
	{
		[Tooltip("The type of custom authentication provider that should be used. Set to 'None' to turn off.")]
		public CustomAuthenticationType authenticationType;
		
		[Tooltip("Name or other end-user ID used in custom authentication service.")]
		[RequiredField]
		public FsmString authName;
		
		[Tooltip("Token provided by authentication service to be used on initial 'login' to Photon.")]
		[RequiredField]
		public FsmString authToken;
		
		[Tooltip("Sets the data to be passed-on to the auth service via POST. Empty string will set AuthPostData to null.")]
		public FsmString authPostData;
		
		public override void Reset()
		{
			authenticationType = CustomAuthenticationType.Custom;
			authName = null;
			authToken = null;
			authPostData = new FsmString(){UseVariable=true};
		}

		public override void OnEnter()
		{
			PhotonNetwork.AuthValues = new AuthenticationValues();
			
			PhotonNetwork.AuthValues.AuthType = authenticationType;
			
            PhotonNetwork.AuthValues.SetAuthParameters(authName.Value, authToken.Value);
			
			PhotonNetwork.AuthValues.SetAuthPostData(authPostData.Value);
			
			Finish();
		}

	}
}