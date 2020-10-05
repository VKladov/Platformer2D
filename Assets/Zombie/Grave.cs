using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    [SerializeField] Zombie _zombiePrefab;
    [SerializeField] Coin _coinPrefab;
    [SerializeField] float _spawnDelay = 5;

    private List<Zombie> _zombies = new List<Zombie>();

    public void StartSpawnZombies()
    {
        StartCoroutine(SpawnZombie());
    }

    private void SpawnCoin()
    {
        Coin newCoin = Instantiate(_coinPrefab, transform.position, Quaternion.identity);
        if (newCoin.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.AddForce(new Vector2(Random.Range(-1f, 1f), 1), ForceMode2D.Impulse);
        }
    }

    private void OnDestroy()
    {
        foreach(Zombie zombie in _zombies)
            zombie.Died.RemoveListener(SpawnCoin);
    }

    private IEnumerator SpawnZombie()
    {
        while (enabled)
        {
            Zombie newZombie = Instantiate(_zombiePrefab, transform.position, Quaternion.identity);
            newZombie.Died.AddListener(SpawnCoin);

            _zombies.Add(newZombie);

            yield return new WaitForSeconds(_spawnDelay);
        }
    }
}
