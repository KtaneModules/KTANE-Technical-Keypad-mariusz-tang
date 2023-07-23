using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : Button
{
    [SerializeField] private MeshRenderer _buttonRenderer;
    [SerializeField] private ColourblindText _colourblindText;

    public event Action<int> OnInteract;

    private KeyColour _colour;
    private Material _material;

    private float _holdTime;

    private Coroutine _holdTracker;

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

        Selectable.OnInteract += () => { _holdTracker = StartCoroutine(TrackHold()); return false; };
        Selectable.OnInteractEnded += () => { StopCoroutine(_holdTracker); OnInteract(Mathf.FloorToInt(_holdTime * 2)); };
    }

    private IEnumerator TrackHold() {
        _holdTime = 0;
        var nextBeepTick = 1;
        while (true) {
            yield return null;
            _holdTime += Time.deltaTime;
            if (_holdTime >= nextBeepTick / 2f) {
                Audio.PlaySoundAtTransform("HoldBeep", transform);
                nextBeepTick++;
            }
        }
    }
}
