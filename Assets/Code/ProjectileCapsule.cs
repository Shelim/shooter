using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCapsule : MonoBehaviour
{
    [SerializeField]
    private float _speed = 50.0f;
    [SerializeField]
    private float _lifetime = 5.0f;
    [SerializeField]
    private LayerMask _layerMask;

    private Pool _pool;
    private float _timeToLive;

    public void Initialize(Pool pool)
    {
        _pool = pool;
        _timeToLive = _lifetime;
    }

    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
        _timeToLive -= Time.deltaTime;
        if (_timeToLive <= 0.0f)
        {
            _pool.Return(gameObject);
            return;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _pool.Return(gameObject);
    }
}
