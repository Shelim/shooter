using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialDefinitions : MonoBehaviour
{
    private MaterialDefinition[] _definitions;
    private Dictionary<Material, MaterialKind> _kinds;

    private void Scan()
    {
        if (_definitions != null && _kinds != null) return;

        var children = GetComponentsInChildren<MaterialDefinition>();
        _definitions = new MaterialDefinition[Enum.GetValues(typeof(MaterialKind)).Length];
        _kinds = new Dictionary<Material, MaterialKind>();
        foreach (var child in children)
        {
            _definitions[(int)child.Kind] = child;
            foreach (var material in child.DefinitionGlobal.Materials)
            {
                _kinds[material] = child.Kind;
            }
        }
    }

    public void Hit(Material material, Vector3 point, Vector3 normal)
    {
        Scan();

        if (_kinds.TryGetValue(material, out var kind))
        {
            var def = _definitions[(int)kind];
            if (def != null)
            {
                def.Hit(point, normal);
            }
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
