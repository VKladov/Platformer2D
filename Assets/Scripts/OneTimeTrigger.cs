using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OneTimeTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Knight>())
        {
            _onEnter?.Invoke();
            _onEnter?.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}
