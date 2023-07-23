using UnityEngine;

[RequireComponent(typeof(KMSelectable), typeof(Animator))]
public class Button : MonoBehaviour
{
    [SerializeField] private string _buttonPressSound;
    [SerializeField] private string _buttonReleaseSound;

    protected KMAudio Audio;
    private Animator _animator;
    private KMHighlightable _highlight;

    public KMSelectable Selectable { get; private set; }

    protected virtual void Awake() {
        Audio = GetComponentInParent<KMAudio>();
        _animator = GetComponent<Animator>();
        Selectable = GetComponent<KMSelectable>();
        _highlight = Selectable.Highlight;

        Selectable.OnInteract += () => {
            _animator.SetBool("IsPressed", true);
            if (!string.IsNullOrEmpty(_buttonPressSound))
                Audio.PlaySoundAtTransform(_buttonPressSound, transform);
            return false;
        };
        Selectable.OnInteractEnded += () => {
            _animator.SetBool("IsPressed", false);
            if (!string.IsNullOrEmpty(_buttonReleaseSound))
                Audio.PlaySoundAtTransform(_buttonReleaseSound, transform);
        };
    }

    public void Enable() => SetState(true);
    public void Disable() => SetState(false);
    public void SetState(bool shouldBeEnabled) => _highlight.gameObject.SetActive(shouldBeEnabled);
}