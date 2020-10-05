using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))] 
public class HealthBar : MonoBehaviour
{
    [SerializeField] Character _character;
    [SerializeField] private Image _heartImage;
    [SerializeField] private Color _liveHeartColor;
    [SerializeField] private Color _deadHeartColor;

    private List<Image> _hearts = new List<Image>();

    private void Start()
    {
        for (int i = 0; i < _character.Health; i++)
        {
            Image heart = Instantiate(_heartImage, gameObject.transform);
            heart.color = _liveHeartColor;
            _hearts.Add(heart);
        }
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < _hearts.Count; i++)
        {
            if (i < _character.Health)
            {
                _hearts[i].color = _liveHeartColor;
            }
            else
            {
                _hearts[i].color = _deadHeartColor;
            }
        }
    }
}
