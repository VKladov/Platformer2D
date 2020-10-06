using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected int _maxHealth;
    [SerializeField] private LayerMask _deathMaskLayer;
    [SerializeField] private UnityEvent _changedHealth;

    public UnityEvent Died;


    protected Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected BoxCollider2D _collider;
    protected SpriteRenderer _renderer;

    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    protected abstract void BeforeAwake();

    public void StartIgnore(Character character)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), character.GetComponent<Collider2D>(), true);
    }

    private void Awake()
    {
        BeforeAwake();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        Health = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsAlive == false)
            return;

        Health -= damage;
        StartCoroutine(ShowDamage());

        _changedHealth?.Invoke();

        if (IsAlive == false)
            Die();
    }

    public void Heal(int heal)
    {
        Health = Mathf.Min(_maxHealth, Health + heal);
        _changedHealth?.Invoke();
    }

    protected virtual void Die()
    {
        _animator.SetTrigger("die");
        _animator.SetBool("isAlive", false);
        gameObject.layer = (int)Mathf.Log(_deathMaskLayer.value, 2f);
        Died?.Invoke();
    }

    private IEnumerator ShowDamage()
    {
        _renderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        _renderer.color = Color.white;
    }
}
