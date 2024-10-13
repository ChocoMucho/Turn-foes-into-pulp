using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    public GameObject Original {  get; private set; }
    public Transform Root { get; set; }

    Queue<Poolable> pool = new Queue<Poolable>();

    public void Init(GameObject original, int count = 10)
    {
        Original = original;
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";

        for(int i = 0; i < count; ++i)
        {
            Enqueue(Create());
        }

    }

    Poolable Create()
    {
        GameObject gameObject = Object.Instantiate<GameObject>(Original);
        gameObject.name = Original.name;
        if (gameObject.GetComponent<Poolable>())
            return gameObject.GetComponent<Poolable>();
        else
        {
            gameObject.AddComponent<Poolable>();
            return gameObject.GetComponent<Poolable>();
        }
    }

    public void Enqueue(Poolable poolable)
    {
        if (poolable == null) return;

        poolable.transform.parent = Root;
        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;

        pool.Enqueue(poolable);
    }

    public Poolable Dequeue(Transform parent)
    {
        Poolable poolable;
        // 풀에 뽑을 요소가 있다면
        if(pool.Count > 0)
            poolable = pool.Dequeue();
        else // 풀에 뽑을 요소가 없다면
            poolable = Create();

        poolable.gameObject.SetActive(true);
        poolable.transform.parent = parent;
        poolable.IsUsing = true;

        return poolable;
    }
}
