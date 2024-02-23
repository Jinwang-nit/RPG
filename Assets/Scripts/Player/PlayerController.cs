using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    [Header("��������")]
    public float walkSpeed = 1;
    public float walkToRunSpeed = 1; // ��·�����ܵĹ����ٶ�
    public float runSpeed = 1;
    public float rotateSpeed = 1;
    public float jumpInitSpeed = 3; // �����ĳ�ʼ�ٶ�
    public float jumpPower = 1; // ����������
    public float jumpHight = 5;
    public bool isDialog = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Init();
        ChangeState(PlayerState.Idle);
    }

    public void ChangeState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<PlayerState_Idle>();
                break;
            case PlayerState.Walk:
                stateMachine.ChangeState<PlayerState_Move>();
                break;
            case PlayerState.Jump:
                stateMachine.ChangeState<PlayerState_Jump>();
                break;
            case PlayerState.AirDown:
                stateMachine.ChangeState<PlayerState_AirDown>();
                break;
        }
    }
}
