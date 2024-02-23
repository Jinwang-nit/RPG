using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Move : PlayerStateBase
{
    private float runTransition = 0;

    public override void Enter()
    {
        player.PlayAnimation("Move");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            player.ChangeState(PlayerState.Idle);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                runTransition = Mathf.Clamp(runTransition + Time.deltaTime * player.walkToRunSpeed, 0, 1);
            }
            else
            {
                runTransition = Mathf.Clamp(runTransition - Time.deltaTime * player.walkToRunSpeed, 0, 1);
            }

            player.Model.Animator.SetFloat("Move", runTransition);
            player.Model.Animator.speed = Mathf.Lerp(player.walkSpeed, player.runSpeed, runTransition);

            Vector3 input = new Vector3(h, 0, v);
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            player.ChangeState(PlayerState.Jump);
        }
    }

    public override void Exit()
    {
        runTransition = 0;
        player.Model.ClearRootMotionAction();
        player.Model.Animator.speed = 1;
    }


    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}
