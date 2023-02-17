using UnityEngine;

namespace _AnimatorScripts {
    public class ResetOnStateEnter : StateMachineBehaviour {
        public string state = "State";
        public int locomotionState = 1;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(state, locomotionState);
        }
    }
}