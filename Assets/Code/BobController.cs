using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobController : MonoBehaviour
{
    public float WalkingSpeed { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsJumping { get; set; }

    [SerializeField]
    private float _amountWalking = 0.025f;
    [SerializeField]
    private float _amountJumping = 0.1125f;
    [SerializeField]
    private float _multiplayerLanding = 0.5f;

    [SerializeField]
    private float _speedWalking = 1.5f;
    [SerializeField]
    private float _jumpSpeed = 6.0f;
    [SerializeField]
    private float _landSpeed = 12.0f;

    private float _progressWalking;
    private float _progressJumping;
    private bool _wasInTheAir;
    private float _landStrength;

    private void Update()
    {
        if (IsGrounded)
        {
            _progressWalking += Time.deltaTime * WalkingSpeed * _speedWalking;
            _progressWalking %= 1.0f;
            if (!_wasInTheAir)
            {
                _progressJumping = Mathf.MoveTowards(_progressJumping, 0.0f, Time.deltaTime * _jumpSpeed);
            }
            else
            {
                _progressJumping = _progressJumping - Time.deltaTime * _landSpeed;
                if (_progressJumping <= -_landStrength * _multiplayerLanding)
                {
                    _wasInTheAir = false;
                    _landStrength = 0.0f;
                }
            }
        }
        else
        {
            _wasInTheAir = true;
        }
        if (IsJumping)
        {
            _progressJumping = Mathf.MoveTowards(_progressJumping, 1.0f, Time.deltaTime * _jumpSpeed);
            if (_progressJumping > _landStrength)
            {
                _landStrength = _progressJumping;
            }
        }
        var walkOffset = new Vector3(Mathf.Sin(_progressWalking * Mathf.PI * 2.0f) * _amountWalking,
            Mathf.Sin(_progressWalking * Mathf.PI * 2.0f * 2.0f) * _amountWalking, 0.0f);

        var jumpOffset = new Vector3(0.0f, -Mathf.Sin(_progressJumping * Mathf.PI * 0.5f) * _amountJumping, 0.0f);

        transform.localPosition = walkOffset + jumpOffset;
    }
}
