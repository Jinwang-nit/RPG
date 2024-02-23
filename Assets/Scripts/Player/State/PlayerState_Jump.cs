using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState_Jump : PlayerStateBase
{
    public override void Enter()
    {
        player.PlayAnimation("JumpStart");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        if (CheckAnimation("JumpStart", out float normalizedTime) && normalizedTime >= 0.9f)
        {
            player.ChangeState(PlayerState.AirDown);
        }
    }

    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y *= player.jumpPower;
        player.CharacterController.Move(deltaPosition);
    }
}