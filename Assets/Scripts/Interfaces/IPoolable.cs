using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    [HideInInspector] public ObjectPool<IPoolable> pool { get; set; }
    GameObject gameObject { get; }

    public void Load()
    {
        gameObject.SetActive(true);
    }

    public void Unload()
    {
        gameObject.SetActive(false);
    }

    public void Release()
    {
        if (gameObject.activeSelf)
        {
            pool ??= ObjectPools.Instance.FindOrCreatePool(this);
            pool.Release(this);
        }
    }
}
