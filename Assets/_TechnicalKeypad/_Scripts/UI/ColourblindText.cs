using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class ColourblindText : MonoBehaviour
{
    private TextMesh _textMesh;
    private Color _mainColor;

    public string Value { set { _textMesh.text = value; } }

#pragma warning disable IDE0051
    private void Awake() {
        _textMesh = GetComponent<TextMesh>();
        _mainColor = _textMesh.color;
        Value = "";
        
        GetComponentInParent<TechnicalKeypadModule>().OnSetColourblindMode += (shouldEnable) => { _textMesh.color = shouldEnable ? _mainColor : Color.black * 0; };
    }
}
