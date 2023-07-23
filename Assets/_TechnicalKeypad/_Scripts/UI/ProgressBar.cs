using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private const float s_emptyPosition = .8f;
    private const float s_fullPosition = 0;
    private readonly Color _neutralColour = new Color(113 / 255f, 113 / 255f, 113 / 255f, 1);
    private readonly Color _warningColour = new Color(131 / 255f, 0, 0, 1);

    [SerializeField] private SpriteRenderer _mainBarRenderer;
    [SerializeField] private Transform _successBar;
    private Transform _mainBar;

    private float _fillLevel;
    private float _displayFillLevel;
    private Coroutine _fillAnimation;

    public float FillRate { get; set; }
    public float FillLevel {
        get { return _fillLevel; }
        set { _fillLevel = Mathf.Clamp(value, 0, 1); }
    }

#pragma warning disable IDE0051
    private void Awake() {
        _mainBar = _mainBarRenderer.transform;
        _fillAnimation = StartCoroutine(DoFillAnimation());
        _mainBar.localPosition = Vector3.up * s_emptyPosition;
        _successBar.localPosition = Vector3.up * s_emptyPosition;
    }
#pragma warning restore IDE0051

    private IEnumerator DoFillAnimation() {
        while (true) {
            FillLevel += FillRate * Time.deltaTime;
            _displayFillLevel = Mathf.Lerp(_displayFillLevel, FillLevel, 5 * Time.deltaTime);

            _mainBarRenderer.color = Color.Lerp(_neutralColour, _warningColour, (.35f - _displayFillLevel) / .35f);
            _mainBar.localPosition = Vector3.up * Mathf.Lerp(s_emptyPosition, s_fullPosition, _displayFillLevel / .8f);
            _successBar.localPosition = Vector3.up * Mathf.Lerp(s_emptyPosition, s_fullPosition, (_displayFillLevel - .8f) / .2f);
            yield return null;
        }
    }
}
