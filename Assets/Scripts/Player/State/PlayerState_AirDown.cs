using System.Collections;
using UnityEngine;
public class PlayerState_AirDown : PlayerStateBase
{
    private Vector3 currentHight = Vector3.zero;
    public override void Enter()
    {
        currentHight.y = player.jumpHight;
        player.PlayAnimation("JumpLoop");
    }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v);

        player.CharacterController.Move(player.Model.transform.TransformDirection(moveDir) * player.walkSpeed * Time.deltaTime);
        player.CharacterController.Move(currentHight * Time.deltaTime);
        currentHight.y += player.gravity * Time.deltaTime;

        if (player.CharacterController.isGrounded)
        {
            player.PlayAnimation("JumpEnd");
        }

        if (CheckAnimation("JumpEnd", out float time) && time >= 0.9f)
        {
            player.ChangeState(PlayerState.Idle);
        }
    }
}