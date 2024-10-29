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
    [SerializeField]
    private float _followCrosshair = 0.125f;

    private Pool _pool;
    private MaterialDefinitions _materialDefinitions;
    private float _timeToLive;

    private Ray _targetRay;

    private void Reset()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Initialize(Pool pool, MaterialDefinitions materialDefinitions, Ray ray)
    {
        _materialDefinitions = materialDefinitions;
        _pool = pool;
        _timeToLive = _lifetime;
        _targetRay = ray;

        if (_trailRenderer)
            _trailRenderer.Clear();
    }

    private void Update()
    {
        var positionBase = transform.position + transform.forward * _speed * Time.deltaTime;
        _targetRay.origin += _targetRay.direction * _speed * Time.deltaTime;
        var positionNext = Vector3.Lerp(positionBase, _targetRay.origin, Time.deltaTime / _followCrosshair);

        if (Physics.Linecast(transform.position, positionNext, out var hit, _layerMask, QueryTriggerInteraction.Ignore))
        {
            _pool.Return(gameObject);
            var madeWith = hit.collider.GetComponentInParent<MadeWith>();
            if (madeWith != null)
            {
                _materialDefinitions.Hit(madeWith.Kind, hit.point, hit.normal);
                return;
            }

            Renderer renderer = hit.collider.GetComponent<Renderer>();
            MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
            if (renderer != null && meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                var idx = hit.triangleIndex;
                for (var i = 0; i < mesh.subMeshCount; i++)
                {
                    var tris = mesh.GetTriangles(i);
                    if (idx < tris.Length)
                    {
                        var material = renderer.sharedMaterials[i];
                        _materialDefinitions.Hit(material, hit.point, hit.normal);
                        return;
                    }
                    idx -= tris.Length;
                }
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
