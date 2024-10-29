using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudCrosshair : MonoBehaviour
{
    public float Spread { get; set; }

    [SerializeField]
    private float _sizeMin = 32.0f;
    [SerializeField]
    private float _sizeMax = 128.0f;

    private RectTransform _rect;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        var size = Mathf.Lerp(_sizeMin, _sizeMax, Spread);

        _rect.offsetMin = new Vector2(-size * 0.5f, -size * 0.5f);
        _rect.offsetMax = new Vector2(size * 0.5f, size * 0.5f);
    }
}
