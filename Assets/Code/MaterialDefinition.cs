using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDefinition : MonoBehaviour
{
    [SerializeField]
    private MaterialKind _kind;
    public MaterialKind Kind => _kind;

    [SerializeField]
    private MaterialDefinitionGlobal _definitionGlobal;
    public MaterialDefinitionGlobal DefinitionGlobal => _definitionGlobal;

    [SerializeField]
    private Pool _hitPool;
    [SerializeField]
    private float _hitTimeToLive;

    public void Hit(Vector3 point, Vector3 normal)
    {
        var hit = _hitPool.Grab(transform);
        hit.transform.position = point;
        hit.transform.rotation = Quaternion.LookRotation(normal);
        StartCoroutine(ReturnHit(hit));
    }
    private IEnumerator ReturnHit(GameObject hit)
    {
        yield return new WaitForSeconds(_hitTimeToLive);
        _hitPool.Return(hit);
    }
}
