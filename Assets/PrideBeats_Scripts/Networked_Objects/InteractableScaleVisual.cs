using UnityEngine;
using Oculus.Interaction;

using Sampleton = SampleController; // only transitional

public class InteractableScaleVisual : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _interactableViewObj;
    private IInteractableView _interactableView;

    [SerializeField]
    private Vector3 normalScale = Vector3.one;

    [SerializeField]
    private Vector3 hoverScale = new Vector3(1.5f, 1.5f, 1.5f);

    [SerializeField]
    private Vector3 selectedScale = new Vector3(2f, 2f, 2f);


    private void Awake()
    {
        _interactableView = _interactableViewObj as IInteractableView;
    }

    private void OnEnable()
    {
        if (_interactableView != null)
        {
            _interactableView.WhenStateChanged += OnStateChanged;
        }
    }

    private void OnDisable()
    {
        if (_interactableView != null)
        {
            _interactableView.WhenStateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(InteractableStateChangeArgs args)
    {
        // You can use args.NewState directly, or query _interactableView.State

        switch (args.NewState)
        {
            case InteractableState.Select:
                Sampleton.Log("SELECTED");
                transform.localScale = selectedScale;
                break;
            case InteractableState.Hover:
                Sampleton.Log("HOVER");
                transform.localScale = hoverScale;
                break;
            case InteractableState.Normal:
                Sampleton.Log("NORMAL");
                transform.localScale = normalScale;
                break;
            case InteractableState.Disabled:
                Sampleton.Log("DISABLED");
                transform.localScale = normalScale;
                break;
        }
    }
}
