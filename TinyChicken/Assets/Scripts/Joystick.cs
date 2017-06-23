using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Takes user input and returns [-1, 1] values
/// </summary>
public class Joystick : Graphic, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    /// <summary>
    /// Current joystick value
    /// </summary>
    public Vector2 Value
    {
        get;
        private set;
    }

    /// <summary>
    /// Is finger on joystick
    /// </summary>
    public bool IsCaptured
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether you are ready to shoot
    /// </summary>
    public bool IsActivated
    {
        get;
        private set;
    }

    public event Action ActionApplied;
    public event Action<Vector3> PointWasSet;

    [SerializeField]
    private Transform thumb;
    [SerializeField]
    private Transform thumbRoot;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private float joystickMaxOffset = 300f;
    [SerializeField]
    private float joystickActivationValue = 0.5f;
    [SerializeField]
    private RectTransform allowedRectTransform;

    private Vector2 firstTouchPosition;
    private Vector3 touchPosition;
    private Vector3 baseThumpPosition;
    private Vector3 baseThumpRootPosition;
    private bool wasDragged = false;
    private int dragFingerId = -1;

    protected override void Start()
    {
        base.Start();
        baseThumpPosition = thumb.transform.position;
        baseThumpRootPosition = thumbRoot.transform.position;
        //thumbRoot.gameObject.SetActive(false);
    }

    private void CaptureJoystick(Vector3 screenPosition)
    {
        touchPosition = screenPosition;
        firstTouchPosition = touchPosition;
        thumbRoot.transform.position = firstTouchPosition;
        IsCaptured = true;
        //thumbRoot.gameObject.SetActive(true);
    }

    private void ReleaseJoystick()
    {
        ApplyAction();
        Value = Vector3.zero;
        thumbRoot.transform.position = baseThumpPosition;
        thumb.transform.position = baseThumpRootPosition;
        IsCaptured = false;
        IsActivated = false;
        //thumbRoot.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wasDragged = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (wasDragged && eventData.pointerId == dragFingerId)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(eventData.position);
        var hitResults = new RaycastHit[1];
        if (Physics.RaycastNonAlloc(ray, hitResults, 10000, 1 << LayerMask.NameToLayer("Arena")) > 0)
        {
            OnPointWasSet(hitResults[0].point);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (wasDragged && eventData.pointerId == dragFingerId)
        {
            ReleaseJoystick();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        touchPosition = eventData.position;
        var allowedRect = ToScreenSpaceRect(allowedRectTransform);

        wasDragged = true;
        dragFingerId = eventData.pointerId;

        if (IsCaptured)
        {
            Vector3 offset = touchPosition - thumbRoot.position;

            var magnitude = offset.magnitude;
            if (magnitude > joystickMaxOffset)
            {
                var delta = offset - offset.normalized * joystickMaxOffset;
                thumbRoot.position += delta;

                offset = offset.normalized * joystickMaxOffset;
            }

            var thumbRootposition = thumbRoot.position;
            thumbRootposition.x = Mathf.Clamp(thumbRootposition.x, allowedRect.xMin, allowedRect.xMax);
            thumbRootposition.y = Mathf.Clamp(thumbRootposition.y, allowedRect.yMin, allowedRect.yMax);
            thumbRoot.position = thumbRootposition;

            //   offset = Vector2.ClampMagnitude(offset, joystickMaxOffset);

            float percentage = magnitude / joystickMaxOffset;
            Value = offset.normalized * curve.Evaluate(percentage);

            if (percentage > joystickActivationValue)
            {
                IsActivated = true;
            }
            else
            {
                IsActivated = false;
            }

            thumb.transform.position = thumbRoot.position + offset;
        }
        else
        {
            CaptureJoystick(eventData.position);
        }
    }

    private void ApplyAction()
    {
        if (Value.magnitude > joystickActivationValue)
        {
            OnActionApplied();
        }
    }

    private void OnActionApplied()
    {
        var handler = ActionApplied;
        if (handler != null)
        {
            handler();
        }
    }

    private void OnPointWasSet(Vector3 point)
    {
        var handler = PointWasSet;
        if (handler != null)
        {
            PointWasSet(point);
        }
    }

    public static Rect ToScreenSpaceRect(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }
}