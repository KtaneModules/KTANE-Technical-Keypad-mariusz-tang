using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ButtonHatch : MonoBehaviour
{
    [SerializeField] private Button _button;

    private Animator _animator;
    
    public KMSelectable Selectable { get; private set; }

#pragma warning disable IDE0051
    private void Awake() {
        _animator = GetComponent<Animator>();
        Selectable = _button.GetComponent<KMSelectable>();
    }

    private void Start() => DisableButton();

    // Animation events
    private void EnableButton() => _button.Enable();
    private void DisableButton() => _button.Disable();
#pragma warning restore IDE0051

    public void Open() => _animator.SetBool("ShouldOpen", true);
    public void Close() => _animator.SetBool("ShouldOpen", false);
}
