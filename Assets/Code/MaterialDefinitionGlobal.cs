using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material Definition", menuName = "Material Definition", order = 0)]
public class MaterialDefinitionGlobal : ScriptableObject
{
    [SerializeField]
    private MaterialKind _kind;
    public MaterialKind Kind => _kind;

    [SerializeField]
    private Material[] _materials;
    public Material[] Materials => _materials;
}
