using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    public Animator Animator { get => animator; }


    // ¸úÔË¶¯
    protected Action<Vector3, Quaternion> rootMotionAction;
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }
    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }

    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
}
