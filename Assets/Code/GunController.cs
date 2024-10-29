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
    private Pool _muzzleFlashes;

    [SerializeField]
    private MaterialDefinitions _materialDefinitions;

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
                var muzzleFlash = _muzzleFlashes.Grab(_hardPoint.transform);
                StartCoroutine(ReturnMuzzleFlash(muzzleFlash));
                //FireProjectileLinecast();
                //FireProjectileCapsule();
                FireHitscan();
            }
        }
    }
    private IEnumerator ReturnMuzzleFlash(GameObject muzzleFlash)
    {
        yield return new WaitForSeconds(0.375f * 60.0f / _roundsPerMinute);
        _muzzleFlashes.Return(muzzleFlash);
    }

    [SerializeField]
    private Pool _projectileLinecasts;

    [SerializeField, Min(0.0f), Tooltip("In degrees")]
    private float _spread = 3.0f;

    private void FireProjectileLinecast()
    {
        var projectile = _projectileLinecasts.Grab(_hardPoint.transform);
        projectile.transform.SetParent(null, true);
        projectile.GetComponent<ProjectileLinecast>().Initialize(_projectileLinecasts, _materialDefinitions);
        var random = Random.insideUnitCircle;
        projectile.transform.Rotate(_spread * random.x, _spread * random.y, 0.0f);
    }

    [SerializeField]
    private Pool _projectileCapsules;

    private void FireProjectileCapsule()
    {
        var projectile = _projectileCapsules.Grab(_hardPoint.transform);
        projectile.transform.SetParent(null, true);
        projectile.GetComponent<ProjectileCapsule>().Initialize(_projectileCapsules, _materialDefinitions);
        var random = Random.insideUnitCircle;
        projectile.transform.Rotate(_spread * random.x, _spread * random.y, 0.0f);
    }

    [SerializeField]
    private LayerMask _layerMaskHitScan;

    private void FireHitscan()
    {
        var random = Random.insideUnitCircle;
        var rotation = Quaternion.Euler(_spread * random.x, _spread * random.y, 0.0f);
        var forward = rotation * Vector3.forward;
        var ray = new Ray(_hardPoint.transform.position, _hardPoint.transform.TransformDirection(forward));
        if (Physics.Raycast(ray, out var hit, 1000.0f, _layerMaskHitScan, QueryTriggerInteraction.Ignore))
        {
            var madeWith = hit.collider.GetComponentInParent<MadeWith>();
            if (madeWith != null)
            {
                _materialDefinitions.Hit(madeWith.Kind, hit.point, hit.normal);
            }
        }
    }

}
