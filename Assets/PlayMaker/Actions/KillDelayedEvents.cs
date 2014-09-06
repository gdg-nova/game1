// (c) copyright Hutong Games, LLC 2010-2012. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Note("Kill all queued delayed events.")]
    [Tooltip("Kill all queued delayed events. Delayed events are 'fire and forget', but sometimes this can cause problems.")]
    public class KillDelayedEvents : FsmStateAction
    {
        public override void OnEnter()
        {
            Fsm.KillDelayedEvents();
            
            Finish();
        }
    }
}
