using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField]
    
    SpriteRenderer spriteRenderer;
    public Sprite[] _sprite;

    [YarnCommand("SwitchEmote")]
    public void SwitchEmote(string emote)
    {
        if (emote == "neutral")
        {
            spriteRenderer.sprite = _sprite[0];
        }
        else if (emote == "sad")
        {
            spriteRenderer.sprite = _sprite[1];
        }
        // elseif (emote == "angry") { spriteRenderer.sprite = _sprite[2]; }

    }
}
