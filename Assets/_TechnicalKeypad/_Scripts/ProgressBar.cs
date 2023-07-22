using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private const float s_emptyPosition = 0.8f;
    private const float s_fullPosition = 0;

    [SerializeField] private Transform _bar;

    private float _fillLevel;
    private Coroutine _fillAnimation;

    public float FillRate { get; set; }
    public float FillLevel {
        get { return _fillLevel; }
        set { _fillLevel = Mathf.Clamp(value, 0, 1); }
    }

#pragma warning disable IDE0051
    private void Awake() {
        _fillAnimation = StartCoroutine(DoFillAnimation());
        _bar.localPosition = Vector3.up * s_emptyPosition;
    }
#pragma warning restore IDE0051

    public void StopFillRoutine() => StopCoroutine(_fillAnimation);

    private IEnumerator DoFillAnimation() {
        while (true) {
            FillLevel += FillRate * Time.deltaTime;
            var targetPosition = Vector3.up * Mathf.Lerp(s_emptyPosition, s_fullPosition, _fillLevel);
            _bar.localPosition = Vector3.Lerp(_bar.localPosition, targetPosition, 100 * Time.deltaTime);
            yield return null;
        }
    }
}
