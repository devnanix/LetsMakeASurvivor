using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPools : MonoBehaviour
{
    public static ObjectPools Instance;
    private Dictionary<IPoolable, ObjectPool<IPoolable>> pools = new Dictionary<IPoolable, ObjectPool<IPoolable>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Get
    public IPoolable Get(IPoolable prefab)
    {
        FindOrCreatePool(prefab);
        return pools[prefab].Get();
    }

    // FineOrCreatePool
    public ObjectPool<IPoolable> FindOrCreatePool(IPoolable prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            if(prefab is Object unityObject)
            {
                pools.Add(prefab,
                    new ObjectPool<IPoolable>(
                        createFunc: () => { IPoolable item = (IPoolable)Instantiate(unityObject, transform); item.pool = pools[prefab]; return item; },
                        actionOnGet: (IPoolable item) => { item.Load(); },
                        actionOnRelease: (IPoolable item) => { item.Unload(); },
                        actionOnDestroy: (IPoolable item) => { Destroy(item.gameObject); },
                        collectionCheck: true,
                        defaultCapacity: 16
                    )
                );
            }
        }
        return pools[prefab];
    }
}
