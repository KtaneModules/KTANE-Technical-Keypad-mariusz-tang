using System.Collections;
using UnityEngine;

using Rnd = UnityEngine.Random;

[RequireComponent(typeof(TextMesh))]
public class Display : MonoBehaviour
{
    private const int s_flickerRepeats = 3;
    private readonly Color _full = new Color(113 / 255f, 113 / 255f, 113 / 255f, 1);
    private readonly Color _half = new Color(50 / 255f, 50 / 255f, 50 / 255f, 1);
    private readonly WaitForSeconds _flickerInterval = new WaitForSeconds(.03f);

    private TextMesh _mesh;
    private Coroutine _flicker;

    public string Text {
        get { return _mesh.text; }
        set { _mesh.text = value; }
    }

#pragma warning disable IDE0051
    private void Awake() {
        _mesh = GetComponent<TextMesh>();
        _mesh.color = Color.black;
    }
#pragma warning restore IDE0051

    public void Enable() {
        StopRoutine();
        _flicker = StartCoroutine(FlickerOn());
    }

    public void Disable() {
        StopRoutine();
        _flicker = StartCoroutine(FlickerOff());
    }

    private void StopRoutine() {
        if (_flicker != null)
            StopCoroutine(_flicker);
    }

    private IEnumerator FlickerOn() {
        yield return _flickerInterval;
        for (int i = 0; i < s_flickerRepeats; i++) {
            _mesh.color = _half;
            yield return _flickerInterval;
            _mesh.color = _full;
            yield return _flickerInterval;
        }
    }

    private IEnumerator FlickerOff() {
        for (int i = 0; i < s_flickerRepeats; i++) {
            _mesh.color = _full;
            yield return _flickerInterval;
            _mesh.color = _half;
            yield return _flickerInterval;
        }
        _mesh.color = Color.black;
    }
}
