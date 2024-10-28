using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private List<GameObject> _poolCache;

    [SerializeField]
    private int _initialCount;
    [SerializeField]
    private GameObject _matrix;


    private void Seed()
    {
        if (_poolCache != null) return;

        _poolCache = new List<GameObject>();
        for (int i = 0; i < _initialCount; i++)
        {
            GameObject item = Instantiate(_matrix, transform);
            item.SetActive(false);
            _poolCache.Add(item);
        }
    }

    private void Awake()
    {
        Seed();
    }

    public GameObject Grab(Transform parent)
    {
        Seed();
        if (_poolCache.Count == 0)
        {
            GameObject item = Instantiate(_matrix, parent);
            return item;
        }
        else
        {
            var item = _poolCache[_poolCache.Count - 1];
            _poolCache.RemoveAt(_poolCache.Count - 1);
            item.SetActive(true);
            item.transform.SetParent(parent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            return item;
        }
    }
    public void Return(GameObject item)
    {
        item.SetActive(false);
        item.transform.SetParent(transform);
        _poolCache.Add(item);
    }
}
