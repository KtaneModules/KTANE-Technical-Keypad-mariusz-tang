using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

[RequireComponent(typeof(KMBombModule), typeof(KMColorblindMode), typeof(KMSelectable))]
public partial class TechnicalKeypadModule : MonoBehaviour
{
    [SerializeField] private Display _digitDisplay;
    [SerializeField] private KeypadButton[] _buttons;
    [SerializeField] private Led[] _leds;
    [SerializeField] private ButtonHatch _submitHatch;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private KMSelectable _statusLightSelectable;
    [SerializeField] private StatusLight _statusLight;

    public event Action<bool> OnSetColourblindMode;

    private KMBombInfo _bombInfo;
    private KMAudio _audio;
    private KMBombModule _module;

    private static int s_moduleCount;
    private int _moduleId;

    private bool _hasActivated;
    private bool _isColourblindMode;

    private KeypadInfo _keypadInfo;
    private KeypadAction[] _correctActions;
    private KeypadAction _currentAction;
    private int _currentActionIndex;
    private int[] _currentExpectedPresses;
    private List<int> _currentPresses = new List<int>();

#pragma warning disable IDE0051
    private void Awake() {
        _moduleId = s_moduleCount++;

        _bombInfo = GetComponent<KMBombInfo>();
        _audio = GetComponent<KMAudio>();
        _module = GetComponent<KMBombModule>();

        // ! Remove if not used.
        _module.OnActivate += Activate;
        _bombInfo.OnBombExploded += OnBombExploded;
        _bombInfo.OnBombSolved += OnBombSolved;


        _keypadInfo = KeypadGenerator.GenerateKeypad();
        // ! Move this to after we get _keypadInfo.
        var modSelectable = GetComponent<KMSelectable>();
        modSelectable.OnFocus += () => {
            if (!_hasActivated) {
                bool[] ledStates = _keypadInfo.LedStates;
                for (int pos = 0; pos < 3; pos++) {
                    if (ledStates[pos])
                        _leds[pos].Enable();
                }
                _digitDisplay.Enable();
                DoInitialLogging();
                _correctActions = KeypadSolver.GenerateSolution(_keypadInfo, _bombInfo, Log);
                _currentAction = _correctActions[0];
                _currentExpectedPresses = _currentAction.ValidButtons;
                LogCurrentRule();
                _hasActivated = true;
            }
        };

        OnSetColourblindMode += (value) => _isColourblindMode = value;
        _statusLightSelectable.OnInteract += () => { OnSetColourblindMode(!_isColourblindMode); return false; };
    }

    private void Start() {
        // TODO: Order this in a sensible manner.
        // TODO: Clean up the logging in KeypadSolver
        OnSetColourblindMode(GetComponent<KMColorblindMode>().ColorblindModeActive);

        _digitDisplay.Text = _keypadInfo.Digits;

        for (int pos = 0; pos < 9; pos++) {
            _buttons[pos].Colour = _keypadInfo.Colours[pos];
            int dummy = pos;
            _buttons[pos].OnInteract += (heldTicks) => HandleInteract(dummy, heldTicks);
        }

        _submitHatch.Selectable.OnInteract += () => { _progressBar.FillLevel += 0.03f; _submitHatch.Selectable.AddInteractionPunch(); return false; };

        Log("Focus the module to begin.");
    }
#pragma warning restore IDE0051

    private void Activate() { }

    private void OnBombExploded() { }
    private void OnBombSolved() { }

    private void HandleInteract(int button, int holdTime) {
        if (holdTime > 0) {
            if (!_currentAction.IsHoldAction)
                Strike("You held a button when you were not supposed to!");
            else if (!_currentExpectedPresses.Contains(button))
                Strike($"You incorrectly held button {button}!");
            else if (_currentAction.HoldTime != holdTime)
                Strike($"You held button {button} for {holdTime} beep(s) when I expected {_currentAction.HoldTime}!");
            else {
                Log($"Correctly held button {button} for {holdTime} beep(s).");
                AdvanceAction();
            }
        }
        else {
            if (_currentAction.IsHoldAction)
                Strike("You tapped a button when you were expected to hold one!");
            else if (_currentPresses.Contains(button))
                Strike($"You pressed button {button} again when you had already pressed it for the current rule!");
            else if (!_currentExpectedPresses.Contains(button))
                Strike($"You incorrectly tapped button {button}!");
            else {
                Log($"Correctly tapped button {button}.");
                _currentPresses.Add(button);
                if (_currentPresses.Count == _currentExpectedPresses.Length)
                    AdvanceAction();
            }
        }
    }

    private void AdvanceAction() {
        Log("Rule passed.");
        _currentActionIndex++;
        _progressBar.FillLevel = 0.5f * _currentActionIndex / _correctActions.Length;

        if (_currentActionIndex >= _correctActions.Length) {
            EnterSubmitState();
            return;
        }

        _currentAction = _correctActions[_currentActionIndex];
        _currentExpectedPresses = _currentAction.ValidButtons;
        _currentPresses = new List<int>();

        LogCurrentRule();
    }

    private void EnterSubmitState() {
        Array.ForEach(_buttons, b => b.Disable());
        _submitHatch.Open();
        _audio.PlaySoundAtTransform("Siren", transform);
        _progressBar.FillRate = -0.1f;
        _statusLight.EnterSirenState();
        StartCoroutine(WatchProgressBar());
    }

    private IEnumerator WatchProgressBar() {
        var timeElapsed = 0f;
        int intTimeHeld = 0;
        while (_progressBar.FillLevel > 0.01f && _progressBar.FillLevel < 0.99f) {
            yield return null;
            timeElapsed += Time.deltaTime;
            if (timeElapsed > intTimeHeld) {
                intTimeHeld++;
                _audio.PlaySoundAtTransform("HoldBeep", transform);
                _leds[0].SetState(intTimeHeld % 2 == 0);
                _leds[1].SetState(intTimeHeld % 2 == 1);
                _leds[2].SetState(intTimeHeld % 2 == 0);
            }
        }
        if (_progressBar.FillLevel <= 0.01f)
            Strike("You let the bar empty all the way!");
        else {
            _progressBar.FillRate = 0;
            _progressBar.FillLevel = 1;
            _submitHatch.Close();
            Solve();
        }
    }

    private void LogCurrentRule() {
        if (_currentAction.IsHoldAction)
            Log($"Current Rule: hold button {_currentExpectedPresses[0]} for {_currentAction.HoldTime} beep(s).");
        else
            Log($"Current Rule: tap buttons {_currentExpectedPresses.Join(", ")}.");
    }

    private void DoInitialLogging() {
        Log($"The displayed digits are {_keypadInfo.Digits}");
        Log($"The colours are, in reading order, {_keypadInfo.Colours.Join(", ").ToLower()}.");
        Log($"The leds, from top to bottom, are {_keypadInfo.LedStates.Select(s => s ? "on" : "off").Join(", ")}.");
    }

    public void Log(string message) {
        Debug.Log($"[Technical Keypad #{_moduleId}] {message}");
    }

    public void Strike(string message) {
        _module.HandleStrike();
        Log($"✕ {message}");
        Log("Resetting.");

        bool[] ledStates = _keypadInfo.LedStates;
        for (int pos = 0; pos < 3; pos++)
            _leds[pos].SetState(ledStates[pos]);

        _submitHatch.Close();
        _progressBar.FillLevel = 0;
        _progressBar.FillRate = 0;
        _currentActionIndex = 0;
        _currentAction = _correctActions[_currentActionIndex];
        _currentExpectedPresses = _currentAction.ValidButtons;
        _currentPresses = new List<int>();
        Array.ForEach(_buttons, b => b.Enable());
        LogCurrentRule();
    }

    public void Solve() {
        Log("◯ Module solved.");
        _module.HandlePass();
        _audio.PlaySoundAtTransform("Solve", transform);
        _leds[0].Enable();
        _leds[1].Disable();
        _leds[2].Disable();
        _digitDisplay.Disable();
    }
}
