using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // GameObject 배열에다가 프리팹들 받아 놓기. 꺼낼 때는?
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>(); // 프리팹들 넣어놓을 곳
    Dictionary<string, Pool> _pool;
    Transform _root;

    protected override void Init()
    {
        base.Init();
        _pool = new Dictionary<string, Pool>();
        if (_root == null)
        {
            _root = gameObject.transform;
            _isDestroyOnLoad = false;
        }
    }

    public void CreatePool(GameObject original, int count = 10)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    public void Enqueue(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (!_pool.ContainsKey(name))
        {
            GameObject.Destroy(poolable.gameObject);
            Debug.Log("풀이 존재하지 않음");
            return;
        }        

        _pool[name].Enqueue(poolable);
    }

    public Poolable Dequeue(GameObject original, Transform parent = null)
    {
        if (!_pool.ContainsKey(original.name))
            CreatePool(original);

        return _pool[original.name].Dequeue(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (!_pool.ContainsKey(name))
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
