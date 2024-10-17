using UnityEngine;

public class ResetTrigger : StateMachineBehaviour
{
    [SerializeField] private string _triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerName);
    }
}
