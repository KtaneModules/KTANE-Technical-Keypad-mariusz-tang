using System.Collections;
using KModkit;
using UnityEngine;

public partial class TechnicalKeypadModule : MonoBehaviour
{
    [SerializeField] KeypadButton[] _buttons;
    [SerializeField] FlickerDisplay _digitDisplay;

    private KMBombInfo _bombInfo;
    private KMAudio _audio;
    private KMBombModule _module;

    private static int s_moduleCount;
    private int _moduleId;
    private bool _isSolved;

    private KeypadInfo _keypadInfo;

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
    }

    private void Start() {
        _keypadInfo = KeypadGenerator.GenerateKeypad();

        _digitDisplay.Text = _keypadInfo.Digits;

        for (int pos = 0; pos < 9; pos++)
            _buttons[pos].Colour = _keypadInfo.Colours[pos];

        _buttons[0].Selectable.OnInteract += () => { Strike("bruh"); return false; };
        _buttons[1].Selectable.OnInteract += () => { Solve(); return false; };

        Log($"Intersection points are {_keypadInfo.IntersectionPositions.Join(", ")}");
    }
#pragma warning restore IDE0051

    private void Activate() { }

    private void OnBombExploded() { }
    private void OnBombSolved() { }

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
