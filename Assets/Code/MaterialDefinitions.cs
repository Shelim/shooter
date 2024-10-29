using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialDefinitions : MonoBehaviour
{
    private MaterialDefinition[] _definitions;

    private void Scan()
    {
        if (_definitions != null) return;

        var children = GetComponentsInChildren<MaterialDefinition>();
        _definitions = new MaterialDefinition[Enum.GetValues(typeof(MaterialKind)).Length];
        foreach (var child in children)
        {
            _definitions[(int)child.Kind] = child;
        }
    }

    public void Hit(MaterialKind kind, Vector3 point, Vector3 normal)
    {
        Scan();

        var def = _definitions[(int)kind];
        if (def != null)
        {
            def.Hit(point, normal);
        }
    }
}
