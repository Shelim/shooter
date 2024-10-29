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
    private MaterialDefinitions _materialDefinitions;
    private float _timeToLive;

    public void Initialize(Pool pool, MaterialDefinitions materialDefinitions)
    {
        _materialDefinitions = materialDefinitions;
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
        var madeWith = other.GetComponentInParent<MadeWith>();
        if (madeWith != null)
        {
            _materialDefinitions.Hit(madeWith.Kind, transform.position, -transform.forward);
        }

        _pool.Return(gameObject);
    }
}
