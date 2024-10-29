using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileLinecast : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer _trailRenderer;
    [SerializeField]
    private float _speed = 50.0f;
    [SerializeField]
    private float _lifetime = 5.0f;
    [SerializeField]
    private LayerMask _layerMask;

    private Pool _pool;
    private MaterialDefinitions _materialDefinitions;
    private float _timeToLive;

    private void Reset()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Initialize(Pool pool, MaterialDefinitions materialDefinitions)
    {
        _materialDefinitions = materialDefinitions;
        _pool = pool;
        _timeToLive = _lifetime;
        if (_trailRenderer)
            _trailRenderer.Clear();
    }

    private void Update()
    {
        var positionNext = transform.position + transform.forward * _speed * Time.deltaTime;
        if (Physics.Linecast(transform.position, positionNext, out var hit, _layerMask, QueryTriggerInteraction.Ignore))
        {
            _pool.Return(gameObject);
            var madeWith = hit.collider.GetComponentInParent<MadeWith>();
            if (madeWith != null)
            {
                _materialDefinitions.Hit(madeWith.Kind, hit.point, hit.normal);
            }
            return;
        }
        _timeToLive -= Time.deltaTime;
        if (_timeToLive <= 0.0f)
        {
            _pool.Return(gameObject);
            return;
        }
        transform.position = positionNext;
    }
}
