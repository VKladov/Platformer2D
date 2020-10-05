using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionConfig : MonoBehaviour
{
    [SerializeField] LayerMask knightLayer;
    [SerializeField] LayerMask deadKnightLayer;
    [SerializeField] LayerMask zombieLayer;
    [SerializeField] LayerMask deadZombieLayer;
    [SerializeField] LayerMask coinLayer;

    private void Awake()
    {
        int zombie = (int)Mathf.Log(zombieLayer.value, 2);
        int deadZombie = (int)Mathf.Log(deadZombieLayer.value, 2);
        int knight = (int)Mathf.Log(knightLayer.value, 2);
        int deadKnight = (int)Mathf.Log(deadKnightLayer.value, 2);
        int coins = (int)Mathf.Log(coinLayer.value, 2);

        Physics2D.IgnoreLayerCollision(zombie, zombie);
        Physics2D.IgnoreLayerCollision(deadZombie, deadZombie);
        Physics2D.IgnoreLayerCollision(zombie, deadZombie);
        Physics2D.IgnoreLayerCollision(deadZombie, knight);
        Physics2D.IgnoreLayerCollision(deadKnight, zombie);
        Physics2D.IgnoreLayerCollision(coins, zombie);
        Physics2D.IgnoreLayerCollision(coins, deadZombie);
        Physics2D.IgnoreLayerCollision(coins, coins);
    }
}
