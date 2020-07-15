using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroySingleton<T> : Singleton<T> where T : DontDestroySingleton<T>
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
