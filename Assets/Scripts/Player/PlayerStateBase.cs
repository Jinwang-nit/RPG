using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateBase
{
    protected PlayerController player;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = (PlayerController) owner;
    }

    public bool CheckAnimation(string animationName, out float time)
    {
        AnimatorStateInfo currentInfo = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
        time = currentInfo.normalizedTime;

        return currentInfo.IsName(animationName);
    }
}
