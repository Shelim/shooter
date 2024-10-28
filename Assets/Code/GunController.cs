using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private float _roundsPerMinute = 600.0f;

    [SerializeField]
    private GameObject _hardPoint;

    [SerializeField]
    private GameObject _muzzleFlash;

    private float _delay;

    private void Start()
    {
        _delay = 0.0f;
    }

    private void Update()
    {
        if (_delay > 0.0f)
        {
            _delay -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            if (_delay <= 0.0f)
            {
                _delay = 60.0f / _roundsPerMinute;
                var muzzleFlash = Instantiate(_muzzleFlash, _hardPoint.transform);
                Destroy(muzzleFlash, 0.375f * 60.0f / _roundsPerMinute);
            }
        }
    }
}
