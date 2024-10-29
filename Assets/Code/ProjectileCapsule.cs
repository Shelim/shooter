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
    [SerializeField]
    private float _followCrosshair = 0.125f;

    private Pool _pool;
    private MaterialDefinitions _materialDefinitions;
    private float _timeToLive;

    private Ray _targetRay;

    public void Initialize(Pool pool, MaterialDefinitions materialDefinitions, Ray ray)
    {
        _materialDefinitions = materialDefinitions;
        _pool = pool;
        _timeToLive = _lifetime;
        _targetRay = ray;
    }

    private void Update()
    {
        var positionBase = transform.position + transform.forward * _speed * Time.deltaTime;
        _targetRay.origin += _targetRay.direction * _speed * Time.deltaTime;
        transform.position = Vector3.Lerp(positionBase, _targetRay.origin, Time.deltaTime / _followCrosshair);

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
