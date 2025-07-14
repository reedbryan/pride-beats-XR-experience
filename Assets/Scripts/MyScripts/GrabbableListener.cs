using Oculus.Interaction;
using UnityEngine;

using Sampleton = SampleController; // only transitional

public class GrabbableListener : Grabbable
{
    public float grabScaleMultiplier = 1.1f;
    public Color movingColor = Color.cyan;

    private bool isMoving;
    private Vector3 originalScale;
    private Color originalColor;
    public Renderer objectRenderer;
    public TrailRenderer trail;

    
    public override void ProcessPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                EndTransform();
                break;
            case PointerEventType.Unselect:
                ForceMove(evt);
                EndTransform();
                break;
            case PointerEventType.Cancel:
                EndTransform();
                break;
        }

        base.ProcessPointerEvent(evt);

        switch (evt.Type)
        {
            case PointerEventType.Select:
                Sampleton.Log($"{gameObject.name} grabbed by user");
                BeginTransform();
                break;
                case PointerEventType.Unselect:
                    BeginTransform();
                    break;
                case PointerEventType.Move:
                    UpdateTransform();
                    break;
            }
    }
    

    protected override void Awake()
    {
        base.Awake(); //  preserves base Grabbable setup

        originalColor = objectRenderer.material.color;
        originalScale = transform.localScale;

        trail.emitting = false;

        Sampleton.Log("The bruh test");
    }


    private void OnSelected()
    {
        //Sampleton.Log($"{gameObject.name} grabbed by user");
        
        isMoving = true;
        transform.localScale = originalScale * grabScaleMultiplier;
        objectRenderer.material.color = movingColor;
        trail.emitting = true;
    }

    private void OnUnselected()
    {
        //Sampleton.Log($"{gameObject.name} released");
        
        isMoving = false;
        transform.localScale = originalScale;
        objectRenderer.material.color = originalColor;
        trail.emitting = false;
    }

}
