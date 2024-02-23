using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public interface IStateMachineOwner { }
public class StateMachine
{
    private IStateMachineOwner owner; // 利用多态的特点来表明当前这个状态机是谁的
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();
    private StateBase currentState;
    public StateBase CurrentState { get => currentState; }

    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    public void ChangeState<T>() where T : StateBase, new()
    {
        if (currentState != null)
        {
            currentState.Exit();
            MonoManager.instance.RemoveUpdateListener(currentState.Update);
        }

        currentState = CheckState<T>(); // 判断新状态是否在池子里，不在就创建一个放进去
        currentState.Enter();
        MonoManager.instance.AddUpdateListener(currentState.Update);
    }

    private StateBase CheckState<T>() where T : StateBase, new()
    {
        if (!stateDic.TryGetValue(typeof(T), out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDic.Add(typeof(T), state);
        }
        return state;
    }
}