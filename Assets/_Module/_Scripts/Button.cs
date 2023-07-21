using UnityEngine;

[RequireComponent(typeof(KMSelectable), typeof(Animator))]
public class Button : MonoBehaviour {

    [SerializeField] private string _buttonPressSound;
    [SerializeField] private string _buttonReleaseSound;

    private KMAudio _audio;
    private Animator _animator;

    public KMSelectable Selectable { get; private set; }

    protected virtual void Awake() {
        _audio = GetComponentInParent<KMAudio>();
        _animator = GetComponent<Animator>();
        
        Selectable = GetComponent<KMSelectable>();
        Selectable.OnInteract += () => {
            _animator.SetBool("IsPressed", true);
            _audio.PlaySoundAtTransform(_buttonPressSound, transform);
            return false;
        };
        Selectable.OnInteractEnded += () => {
            _animator.SetBool("IsPressed", false);
            _audio.PlaySoundAtTransform(_buttonReleaseSound, transform);
        };
    }
}