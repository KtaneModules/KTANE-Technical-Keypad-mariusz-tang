using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHatch : MonoBehaviour
{
    private Animator _animator;
    private Button _button;

#pragma warning disable IDE0051
    private void Awake() {
        _animator = GetComponent<Animator>();
        _button = GetComponentInChildren<Button>();
    }

    private void Start() => DisableButton();

    // Animation events
    private void EnableButton() => _button.Enable();
    private void DisableButton() => _button.Disable();
#pragma warning restore IDE0051

    public void Open() => _animator.SetBool("ShouldOpen", true);
    public void Close() => _animator.SetBool("ShouldOpen", false);

}
