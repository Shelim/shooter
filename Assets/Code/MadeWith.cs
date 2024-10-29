using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadeWith : MonoBehaviour
{
    [SerializeField]
    private MaterialKind _kind;
    public MaterialKind Kind => _kind;
}
