using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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

    [SerializeField]
    private HudCrosshair _hudCrosshair;
    [SerializeField]
    private float _spreadCooldown = 0.65f;
    [SerializeField]
    private float _spreadWindup = 0.125f;
    [SerializeField]
    private float _spreadOnFire = 0.25f;

    public bool IsMoving { get; set; }

    private float _delay;
    private float _spreadProgress;

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

        if (IsMoving)
            _spreadProgress = Mathf.MoveTowards(_spreadProgress, 1.0f, Time.deltaTime / _spreadWindup);
        else
            _spreadProgress = Mathf.MoveTowards(_spreadProgress, 0.0f, Time.deltaTime / _spreadCooldown);

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
                _spreadProgress = Mathf.MoveTowards(_spreadProgress, 1.0f, _spreadOnFire);
            }
        }
        _hudCrosshair.Spread = _spreadProgress;
    }
    private IEnumerator ReturnMuzzleFlash(GameObject muzzleFlash)
    {
        yield return new WaitForSeconds(0.375f * 60.0f / _roundsPerMinute);
        _muzzleFlashes.Return(muzzleFlash);
    }

    [SerializeField]
    private Pool _projectileLinecasts;

    [SerializeField, Min(0.0f), Tooltip("In degrees")]
    private float _spreadMin = 3.0f;
    [SerializeField, Min(0.0f), Tooltip("In degrees")]
    private float _spreadMax = 4.5f;

    private void FireProjectileLinecast()
    {
        var projectile = _projectileLinecasts.Grab(_hardPoint.transform);
        projectile.transform.SetParent(null, true);
        projectile.GetComponent<ProjectileLinecast>().Initialize(_projectileLinecasts, _materialDefinitions);
        var random = Random.insideUnitCircle;
        var spread = Mathf.Lerp(_spreadMin, _spreadMax, _spreadProgress);
        projectile.transform.Rotate(spread * random.x, spread * random.y, 0.0f);
    }

    [SerializeField]
    private Pool _projectileCapsules;

    private void FireProjectileCapsule()
    {
        var projectile = _projectileCapsules.Grab(_hardPoint.transform);
        projectile.transform.SetParent(null, true);
        projectile.GetComponent<ProjectileCapsule>().Initialize(_projectileCapsules, _materialDefinitions);
        var random = Random.insideUnitCircle;
        var spread = Mathf.Lerp(_spreadMin, _spreadMax, _spreadProgress);
        projectile.transform.Rotate(spread * random.x, spread * random.y, 0.0f);
    }

    [SerializeField]
    private LayerMask _layerMaskHitScan;

    private void FireHitscan()
    {
        var random = Random.insideUnitCircle;
        var spread = Mathf.Lerp(_spreadMin, _spreadMax, _spreadProgress);
        var rotation = Quaternion.Euler(spread * random.x, spread * random.y, 0.0f);
        var forward = rotation * Vector3.forward;
        var ray = new Ray(_hardPoint.transform.position, _hardPoint.transform.TransformDirection(forward));
        if (Physics.Raycast(ray, out var hit, 1000.0f, _layerMaskHitScan, QueryTriggerInteraction.Ignore))
        {
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
    }

}
