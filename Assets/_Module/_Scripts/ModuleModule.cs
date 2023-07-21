using System.Collections;
using KModkit;
using UnityEngine;

public partial class ModuleModule : MonoBehaviour {

    [SerializeField] KeypadButton[] _buttons;
    [SerializeField] FlickerDisplay _digitDisplay;

    private KMBombInfo _bombinfo;
    private KMAudio _audio;
    private KMBombModule _module;

    private static int _moduleCount;
    private int _moduleId;
    private bool _isSolved;

    private KeypadInfo _keypadInfo;

    private void Awake() {
        _moduleId = _moduleCount++;

        _bombinfo = GetComponent<KMBombInfo>();
        _audio = GetComponent<KMAudio>();
        _module = GetComponent<KMBombModule>();

        // ! Remove if not used.
        _module.OnActivate += Activate;
        _bombinfo.OnBombExploded += OnBombExploded;
        _bombinfo.OnBombSolved += OnBombSolved;
    }

    private void Start() {
        _keypadInfo = KeypadGenerator.GenerateKeypad();

        _digitDisplay.Text = _keypadInfo.Digits;

        for (int pos = 0; pos < 9; pos++)
            _buttons[pos].Colour = _keypadInfo.Colours[pos];
    }

    private void Activate() { }

    // * These are quite self-explanatory.
    private void OnBombExploded() { }
    private void OnBombSolved() { }

    public void Log(string message) {
        Debug.Log($"[Module #{_moduleId}] {message}");
        throw new System.NotImplementedException("Change the logging tag to match the module name.");
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
