using System.Collections;
using UnityEngine;

public class Led : MonoBehaviour
{
    [SerializeField] private Color _colour;
    [SerializeField] private MeshRenderer _cover;
    [SerializeField] private MeshRenderer _ledInner;

    private Coroutine _transition;

    public void Enable() => SetState(true);
    public void Disable() => SetState(false);
    public void SetState(bool shouldTurnOn) {
        if (_transition != null)
            StopCoroutine(_transition);
        _transition = StartCoroutine(DoTransition(shouldTurnOn));
    }

    private IEnumerator DoTransition(bool shouldTurnOn) {
        var initialColour = _cover.material.color;
        var finalColour = shouldTurnOn ? _colour : Color.black;
        var elapsedTime = 0f;

        while (elapsedTime < .2f) {
            var progress = elapsedTime / .2f;
            var newColour = Color.Lerp(initialColour, finalColour, progress);
            _cover.material.color = newColour;
            _ledInner.material.color = newColour;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _cover.material.color = finalColour;
        _ledInner.material.color = finalColour;
    }
}
