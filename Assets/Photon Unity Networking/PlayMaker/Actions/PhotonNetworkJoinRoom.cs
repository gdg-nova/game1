// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("Join room with given title. Global Photon Event 'JOINED ROOM' will occur, or 'CREATED ROOM' if createIfnotExists was true and processed. If no such room exists and createIfnotExists is set to false, An Photon Error Event will occur 'FAILED TO JOIN ROOM'")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W914")]
	public class PhotonNetworkJoinRoom : FsmStateAction
	{
		[Tooltip("The room Name")]
		public FsmString roomName;
		
		[Tooltip("If true, the server will attempt to create a room, as if CreateRoom was called instead.")]
		public FsmBool createIfNotExists;
		
		public override void Reset()
		{
			roomName  = null;
			createIfNotExists = false;
		}

		public override void OnEnter()
		{
			PhotonNetwork.JoinRoom(roomName.Value,createIfNotExists.Value);
			
			Finish();
		}

	}
}