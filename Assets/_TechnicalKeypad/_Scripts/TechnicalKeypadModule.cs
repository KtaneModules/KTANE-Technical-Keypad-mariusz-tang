using System;
using System.Collections;
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

    public event Action<bool> OnSetColourblindMode;

    private KMBombInfo _bombInfo;
    private KMAudio _audio;
    private KMBombModule _module;

    private static int s_moduleCount;
    private int _moduleId;

    private bool _hasActivated;
    private bool _isColourblindMode;

    private KeypadInfo _keypadInfo;
    private KeypadSolver _solver;

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
                _hasActivated = true;
                DoInitialLogging();
                _solver = new KeypadSolver(_keypadInfo, _bombInfo, Log);
            }
        };

        OnSetColourblindMode += (value) => _isColourblindMode = value;
        _statusLightSelectable.OnInteract += () => { OnSetColourblindMode(!_isColourblindMode); return false; };
    }

    private void Start() {
        // TODO: Order this in a sensible manner.
        OnSetColourblindMode(GetComponent<KMColorblindMode>().ColorblindModeActive);
        _keypadInfo = KeypadGenerator.GenerateKeypad();

        _digitDisplay.Text = _keypadInfo.Digits;

        for (int pos = 0; pos < 9; pos++)
            _buttons[pos].Colour = _keypadInfo.Colours[pos];

        Log("Focus the module to begin.");
    }
#pragma warning restore IDE0051

    private void Activate() { }

    private void OnBombExploded() { }
    private void OnBombSolved() { }

    private void DoInitialLogging() {
        Log($"The displayed digits are {_keypadInfo.Digits}");
        Log($"The colours are, in reading order, {_keypadInfo.Colours.Join(", ").ToLower()}.");
        Log($"The leds, from top to bottom, are {_keypadInfo.LedStates.Select(s => s ? "on" : "off").Join(", ")}.");
    }

    public void Log(string message) {
        Debug.Log($"[Module #{_moduleId}] {message}");
    }

    public void Strike(string message) {
        Log($"✕ {message}");
        _module.HandleStrike();
    }

    public void Solve() {
        Log("◯ Module solved.");
        _module.HandlePass();
    }
}
