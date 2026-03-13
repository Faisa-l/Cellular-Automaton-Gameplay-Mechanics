using UnityEngine;
using System;

/// <summary>
/// An object which contains a single value. Other scripts can reference this value and subscribe to events when the value updates.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ValueObject<T> : RuntimeScriptableObject
{
    [SerializeField]
    T value;

    [SerializeField]
    T initialValue;

    /// <summary>
    /// The value held in the object.
    /// </summary>
    public T Value 
    { 
        get => value;
        set 
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        } 
    }

    /// <summary>
    /// Invoked whenever the value receives a change, returning the new value.
    /// </summary>
    /// <remarks> This may fire even if setting the value does not actually change the value it holds. </remarks>
    public event Action<T> OnValueChanged;

    protected override void ResetInstance() => value = initialValue;

}
