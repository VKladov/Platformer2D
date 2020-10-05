using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CoinCounterText : MonoBehaviour
{
    [SerializeField] private Knight _player;

    private Text _textRenderer;

    private void Awake()
    {
        _textRenderer = GetComponent<Text>();
    }

    public void UpdateNumber()
    {
        _textRenderer.text = _player.Coins + "";
    }
}
