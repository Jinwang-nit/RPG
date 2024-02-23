using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : SingletonMono<MonoManager>
{
    private Action updateAction;

    public void AddUpdateListener(Action action)
    {
        updateAction += action;
    }

    public void RemoveUpdateListener(Action action)
    {  
        updateAction -= action;
    }

    private void Update()
    {
        updateAction?.Invoke();
    }
}
