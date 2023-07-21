using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : Button {

    private KeyColour _colour;
    private Material _material;

    public KeyColour Colour {
        get { return _colour; }
        set {
            _colour = value;
            _material.color = _colour.Colour;
        }
    }

    protected override void Awake() {
        base.Awake();
        _material = GetComponentInChildren<MeshRenderer>().material;
    }
}
