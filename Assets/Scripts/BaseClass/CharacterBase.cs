using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour, IStateMachineOwner
{
    [SerializeField] protected ModelBase model;
    public ModelBase Model { get => model; }

    [SerializeField] protected CharacterController characterController;
    public CharacterController CharacterController { get => characterController;}
    protected StateMachine stateMachine;
    public float gravity = -9.8f;

    public void Init()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(this);
    }

    private string currentAnimationName;
    public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.25f)
    {
        if (currentAnimationName == animationName) return;
        currentAnimationName = animationName;
        model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }
}
