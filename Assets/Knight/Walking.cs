using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Walking : MonoBehaviour
{
    [SerializeField] private float _jumpSpeed = 5f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minGroundNormalY = 0.65f;
    [SerializeField] private float _shellRadius = 0.01f;

    public Vector2 Direction { get; private set; }

    private Vector2 _velocity = Vector2.zero;
    private Vector2 _targetVelocity;
    private bool _grounded;
    private Vector2 _groundNormal;
    private ContactFilter2D _contactFilter = new ContactFilter2D();
    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;

    private const float _minMoveDistance = 0.001f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        _targetVelocity = new Vector2(Input.GetAxis("Horizontal"), 0) * _speed;

        if (Input.GetKeyDown(KeyCode.Space) && _grounded)
        {
            _animator.SetTrigger("isJumping");
            _velocity.y = _jumpSpeed;
        }
    }

    private void FixedUpdate()
    { 
        _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
        _velocity.x = _targetVelocity.x;

        _grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);
        Vector2 move = deltaPosition * moveAlongGround.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        _animator.SetBool("isWalking", _grounded && _targetVelocity.x != 0);

        if (_targetVelocity.x != 0)
        {
            _renderer.flipX = _targetVelocity.x > 0;
            Direction = new Vector2(_targetVelocity.normalized.x, 0);
        }
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > _minMoveDistance)
        {
            int count = _rigidbody.Cast(move, _contactFilter, _hitBuffer, distance + _shellRadius);

            for (int i = 0; i < count; i++)
            {
                Vector2 currentNormal = _hitBuffer[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    _grounded = true;

                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                    _velocity -= projection * currentNormal;

                float modifiedDistance = _hitBuffer[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        _rigidbody.position += move.normalized * distance;
    }
}
