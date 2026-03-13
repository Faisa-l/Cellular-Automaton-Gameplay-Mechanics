using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// If a ScriptableObject needs to be reinitalised to an original state at runtime, it should derive from this class.
/// </summary>
public abstract class RuntimeScriptableObject : ScriptableObject
{
    static readonly List<RuntimeScriptableObject> instances = new();

    private void OnEnable()
    {
        instances.Add(this);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    /// <summary>
    /// Reset this object to a starting state.
    /// </summary>
    protected abstract void ResetInstance();

    // Will reset the instances of all objects when the game starts loading
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetInstances()
    {
        foreach (var instance in instances)
        {
            instance.ResetInstance();
        }
    }
}