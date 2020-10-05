using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Walking))]
public class Knight : Character
{
    [SerializeField] private UnityEvent _changedCoins;

    private Walking _walking;
    private bool _isAttacking = false;

    public int Coins { get; private set; } = 0;

    protected override void BeforeAwake()
    {
        _walking = GetComponent<Walking>();
    }

    private void Update()
    {
        if (IsAlive == false)
            return;

        if (Input.GetKeyDown(KeyCode.X) && !_isAttacking)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        _isAttacking = true;
        _animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(0.2f);

        var hits = Physics2D.RaycastAll(transform.position, _walking.Direction, 1f);
        var filtered = hits.Where(hit => hit.collider != _collider).ToList();

        for (int i = 0; i < filtered.Count; i++)
        {
            RaycastHit2D currentHit = filtered[i];
            if (currentHit.transform.gameObject.TryGetComponent(out Zombie zombie))
                zombie.TakeDamage(_damage);
        }

        yield return new WaitForSeconds(0.2f);

        _isAttacking = false;
    }

    protected override void Die()
    {
        base.Die();
        _walking.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Coin coin))
        {
            Coins++;
            Destroy(coin.gameObject);
            _changedCoins?.Invoke();
        }
    }
}
