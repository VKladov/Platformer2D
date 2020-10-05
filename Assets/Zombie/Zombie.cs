using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum ZombieState
{
    Patroling,
    Chasing
}

public class Zombie : Character
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private LayerMask _obstacles;
    [SerializeField] private float _climbDuration = 1f;
    [SerializeField] private float _forgetDistance = 4f;
    [SerializeField] private float _forgetTime = 2f;
    [SerializeField]  private ZombieState _initialState = ZombieState.Patroling;

    private GameObject _target;
    private float _currentSpeed;
    private Vector2 _direction = new Vector2(1, 0);
    private bool _canAttack = true;
    private bool _isWaiting = false;
    private bool _isClambing = false;
    private ClimbAnchor[] _climbAnchors;
    private ZombieState _state;
     
    private const float _climbHeight = 1.5f;

    private RaycastHit2D _frontHit => Physics2D.Raycast(transform.position, _direction, 0.5f, _obstacles);
    private RaycastHit2D _downHit => Physics2D.Raycast(transform.position + new Vector3(_direction.x * 0.3f, 0, 0), Vector2.down, _collider.bounds.size.y / 2 + 0.2f, _obstacles);

    private ClimbAnchor _nearestAnchor
    {
        get
        {
            ClimbAnchor[] _anchorsAbove = _climbAnchors.Where(anchor => anchor.transform.position.y > transform.position.y && anchor.transform.position.y - transform.position.y < _climbHeight).ToArray();
            if (_anchorsAbove.Length == 0)
                return null;

            ClimbAnchor nearest = _anchorsAbove[0];
            float minDistance = float.MaxValue;
            for (int i = 0; i < _anchorsAbove.Length; i++)
            {
                ClimbAnchor currentAnchor = _anchorsAbove[i];

                Debug.DrawLine(transform.position, currentAnchor.transform.position, Color.red);

                float distance = Vector2.Distance(transform.position, currentAnchor.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = currentAnchor;
                }
            }

            return nearest;
        }
    }

    protected override void BeforeAwake()
    {
        _currentSpeed = _speed;
        _climbAnchors = FindObjectsOfType<ClimbAnchor>();
        _target = FindObjectOfType<Knight>().gameObject;
        _state = _initialState;
    }

    private void Update()
    {
        if (IsAlive == false || _isClambing)
            return;

        if (_state == ZombieState.Chasing)
        {
            if (_target.transform.position.y - transform.position.y > 0.5f)
            {
                if (_isClambing == false)
                {
                    ClimbAnchor nearestAnchor = _nearestAnchor;
                    if (nearestAnchor != null)
                    {
                        float frontX = _direction.x > 0 ? _collider.bounds.max.x : _collider.bounds.min.x;
                        float distanceX = Mathf.Abs(frontX - nearestAnchor.transform.position.x);
                        float distanceY = Mathf.Abs(transform.position.y - nearestAnchor.transform.position.y);
                        if (distanceX < 0.5f && distanceY < _climbHeight)
                        {
                            StartCoroutine(Climb(nearestAnchor));
                            return;
                        }
                    }
                }
            }
        }


        if (_downHit == false && _state == ZombieState.Patroling)
        {
            _currentSpeed = 0;
            if (!_isWaiting)
                StartCoroutine(ResumeWalking(true));
        }
        else if (_frontHit)
        {
            if (_frontHit.transform.gameObject.TryGetComponent(out Knight player) && player.IsAlive)
            {
                _state = ZombieState.Chasing;

                _currentSpeed = 0;
                if (_canAttack)
                    StartCoroutine(Attack());
            }
            else if (_currentSpeed != 0)
            {
                _currentSpeed = 0;
                StartCoroutine(ResumeWalking(true));
            }
        }
        else if (_currentSpeed == 0 && !_isWaiting)
        {
            StartCoroutine(ResumeWalking(false));
        }

        _rigidbody.velocity = new Vector2(_currentSpeed * _direction.x, _rigidbody.velocity.y);
        _animator.SetBool("isWalking", _currentSpeed != 0);
        _renderer.flipX = _direction.x < 0;
    }

    private IEnumerator Attack()
    {
        _animator.SetTrigger("attack");
        _canAttack = false;

        yield return new WaitForSeconds(0.2f);

        if (_frontHit.transform.gameObject.TryGetComponent(out Knight player))
            player.TakeDamage(_damage);

        yield return new WaitForSeconds(0.8f);

        _canAttack = true;
    }

    private IEnumerator Climb(ClimbAnchor climbPoint)
    {
        if (climbPoint.FromLeft)
            _direction = new Vector2(1, 0);
        else
            _direction = new Vector2(-1, 0);

        _isClambing = true;
        _currentSpeed = 0;
        float climbTime = 0;

        Vector2 targetPoint = climbPoint.transform.position + new Vector3(0, _collider.bounds.size.y, 0);
        Vector2 startPoint = transform.position;

        while (climbTime < _climbDuration)
        {
            float progress = climbTime / _climbDuration;
            climbTime += Time.deltaTime;

            _rigidbody.position = Vector2.Lerp(startPoint, targetPoint, progress);

            yield return null;
        }
        _isClambing = false;
    }

    private IEnumerator ResumeWalking(bool changeDirection)
    {
        _isWaiting = true;
        if (_state == ZombieState.Chasing)
            yield return null;
        else
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        if (changeDirection)
            _direction = -_direction;

        _currentSpeed = _speed;
        _isWaiting = false;
    }

    protected override void Die()
    {
        base.Die();
        _rigidbody.velocity = Vector2.zero;
    }
}
