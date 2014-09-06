// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("True if the PhotonView was loaded with the scene (game object) or instantiated with InstantiateSceneObject." +
		"\n Scene objects are not owned by a particular player but belong to the scene. " +
		"Thus they don't get destroyed when their creator leaves the game and the current Master Client can control them (whoever that is)." +
		" The ownerIs is 0 (player IDs are 1 and up). \n A PhotonView component is required on the gameObject")]
	//[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W918")]
	public class PhotonViewGetIsSceneView : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(PhotonView))]
		[Tooltip("The Game Object with the PhotonView attached.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the Photon network view is a scene view.")]
		public FsmBool isSceneView;
		
		[Tooltip("Send this event if the Photon network view is a scene view")]
		public FsmEvent isSceneViewEvent;
		
		[Tooltip("Send this event if the Photon network view is NOT a scene view")]
		public FsmEvent isNotSceneViewEvent;
		
		private PhotonView _networkView;
		
		private void _getNetworkView()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) 
			{
				return;
			}
			
			_networkView =  go.GetComponent<PhotonView>();
		}
		
		public override void Reset()
		{
			gameObject = null;
			isSceneView = null;
			isSceneViewEvent = null;
			isNotSceneViewEvent = null;
		}

		public override void OnEnter()
		{
			_getNetworkView();
			
			checkIsSceneView();
			
			Finish();
		}
		
		void checkIsSceneView()
		{
			if (_networkView ==null)
			{
				return;	
			}
			bool _isSceneView = _networkView.isSceneView;
			isSceneView.Value = _isSceneView;
			
			if (_isSceneView )
			{
				if (isSceneViewEvent!=null)
				{
					Fsm.Event(isSceneViewEvent);
				}
			}
			else if (isNotSceneViewEvent!=null)
			{
				Fsm.Event(isNotSceneViewEvent);
			}
		}

	}
}