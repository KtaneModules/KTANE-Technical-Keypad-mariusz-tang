using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusLight : MonoBehaviour
{
    [SerializeField] private Transform _spinner;
    [SerializeField] private MeshRenderer _cover;
    [SerializeField] private Light[] _lights;

    private float _spinRate;
    private bool _isActive;
    private bool _stayActive;

    private Coroutine _strikeFlash;
    private Coroutine _lightStateTransition;

#pragma warning disable IDE0051
    private void Awake() => StartCoroutine(Spin());

    private void Start() {
        var module = GetComponentInParent<TechnicalKeypadModule>();

        var scale = module.transform.lossyScale.x * 0.02f;
        DoToLights(l => l.range = scale);

        var modSelectable = module.GetComponent<KMSelectable>();
        modSelectable.OnFocus += () => _isActive = true;
        modSelectable.OnDefocus += () => _isActive = false;

        var moduleModComp = module.GetComponent<KMBombModule>();
        moduleModComp.OnStrike += () => { StopStrikeFlash(); _strikeFlash = StartCoroutine(DoStrikeFlash()); return false; };
        moduleModComp.OnPass += () => { EnterSolveState(); return false; };
    }
#pragma warning restore IDE0051 

    private void DoToLights(Action<Light> action) {
        action(_lights[0]);
        action(_lights[1]);
    }

    private IEnumerator Spin() {
        while (true) {
            _spinner.Rotate(360 * Vector3.right * _spinRate * Time.deltaTime);
            _spinRate = Mathf.Lerp(_spinRate, _isActive || _stayActive ? 1 : 0, Time.deltaTime);
            yield return null;
        }
    }

    private void SetLightColour(Color colour) {
        DoToLights(l => l.color = colour);
    }

    private void SetLightState(bool shouldEnable) {
        if (_lightStateTransition != null)
            StopCoroutine(_lightStateTransition);
        _lightStateTransition = StartCoroutine(TransitionLightState(shouldEnable));
    }

    private IEnumerator TransitionLightState(bool shouldEnable) {
        var initialIntensity = _lights[0].intensity;
        var finalIntensity = shouldEnable ? 20 : 0;
        var initialColour = _cover.material.color;
        var finalColour = shouldEnable ? _lights[0].color * 0.5f : Color.black;
        var elapsedTime = 0f;

        while (elapsedTime < 0.3f) {
            var progress = elapsedTime / 0.3f;
            var newIntensity = Mathf.Lerp(initialIntensity, finalIntensity, progress);
            _cover.material.color = Color.Lerp(initialColour, finalColour, progress);
            DoToLights(l => l.intensity = newIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DoToLights(l => l.intensity = finalIntensity);
        _cover.material.color = finalColour;
    }

    private void StopStrikeFlash() {
        if (_strikeFlash != null)
            StopCoroutine(_strikeFlash);
    }

    private IEnumerator DoStrikeFlash() {
        SetLightColour(Color.red);
        SetLightState(true);
        yield return new WaitForSeconds(2f);
        SetLightState(false);
    }

    private void EnterSolveState() {
        StopStrikeFlash();
        SetLightColour(Color.green);
        SetLightState(true);
        _stayActive = true;
    }
}
