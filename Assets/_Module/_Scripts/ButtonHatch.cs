using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHatch : MonoBehaviour {


    private Animator _animator;
    private KMSelectable _selectable;

    private Vector3 _selectableHighlightScale;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _selectable = GetComponentInChildren<KMSelectable>();

        _selectableHighlightScale = _selectable.Highlight.transform.localScale;
        DisableSelectable();
    }

    public void Open() => _animator.SetBool("ShouldOpen", true);
    public void Close() => _animator.SetBool("ShouldOpen", false);

    // Animation events
    private void EnableSelectable() => _selectable.Highlight.transform.localScale = _selectableHighlightScale;
    private void DisableSelectable() => _selectable.Highlight.transform.localScale = Vector3.zero;
}
