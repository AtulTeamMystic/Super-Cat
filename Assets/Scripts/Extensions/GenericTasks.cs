using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTasks : MonoBehaviour
{
    #region DelayDestroy

    public static void StaticDelayDestroy(MonoBehaviour mono, GameObject Go, float delay,
        System.Action OnComplete = null)
    {
        GenericTasks tasks = Go.gameObject.AddComponent<GenericTasks>();
        mono.StartCoroutine(tasks.CoreDelayDestroy(Go.gameObject, delay, OnComplete));
    }

    IEnumerator CoreDelayDestroy(GameObject go, float time, System.Action oncompelete)
    {
        yield return StaticData.GetWait(time);
        oncompelete?.Invoke();
        go.CustomDestroy();
        Destroy(this);
    }

    #endregion

    #region getDelay

    public static void StaticGetDelay(MonoBehaviour mono, float delay, System.Action OnComplete = null)
    {
        GenericTasks tasks = mono.gameObject.AddComponent<GenericTasks>();
        mono.StartCoroutine(tasks.CoreGetDelayDestroy(delay, OnComplete));
    }

    IEnumerator CoreGetDelayDestroy(float time, System.Action oncompelete)
    {
        yield return StaticData.GetWait(time);
        oncompelete?.Invoke();

        Destroy(this);
    }

    #endregion
}