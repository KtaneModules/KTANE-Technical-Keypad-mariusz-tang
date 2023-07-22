using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : Button
{
    [SerializeField] private MeshRenderer _buttonRenderer;
    [SerializeField] private ColourblindText _colourblindText;

    private KeyColour _colour;
    private Material _material;

    public KeyColour Colour {
        get { return _colour; }
        set {
            _colour = value;
            _material.color = _colour.Colour;
            _colourblindText.Value = _colour.ColourblindText;
        }
    }

    protected override void Awake() {
        base.Awake();
        _material = _buttonRenderer.material;
    }
}
