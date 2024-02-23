using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState_Idle : PlayerStateBase
{
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
    } 

    public override void Enter()
    {
        player.PlayAnimation("Idle");
    }

    public override void Update()
    {
        player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if ((h != 0 || v != 0) && !player.isDialog)
        {
            player.ChangeState(PlayerState.Walk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(PlayerState.Jump);
        }
    }
}
