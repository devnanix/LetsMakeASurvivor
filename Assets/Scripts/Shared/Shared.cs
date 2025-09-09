using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Shared<T> : ScriptableObject, ISerializationCallbackReceiver
{
    public T defaultValue;
    public UnityAction<T> OnChange;

    [NonSerialized]
    private T _shared;
    public T shared
    {
        get
        {
            return _shared;
        }
        set
        {
            _shared = value;
            OnChange?.Invoke(value);
        }
    }

    public void OnAfterDeserialize()
    {
        _shared = defaultValue;
    }

    public void OnBeforeSerialize()
    {
    }
}
