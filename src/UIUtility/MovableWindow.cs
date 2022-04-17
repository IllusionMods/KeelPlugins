using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UILib
{
    public class MovableWindow : UIBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Vector2 _cachedDragPosition;
        private Vector2 _cachedMousePosition;
        private bool _pointerDownCalled;
        private BaseCameraControl _cameraControl;
        private BaseCameraControl.NoCtrlFunc _noControlFunctionCached;

        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> Drag;
        public event Action<PointerEventData> PointerUp;

        public RectTransform toDrag;
        public bool preventCameraControl;

#if KKS || HS2 || AI
        public override void Awake()
#else
        protected override void Awake()
#endif
        {
            base.Awake();
            _cameraControl = FindObjectOfType<BaseCameraControl>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(preventCameraControl && _cameraControl)
            {
                _noControlFunctionCached = _cameraControl.NoCtrlCondition;
                _cameraControl.NoCtrlCondition = () => true;
            }
            _pointerDownCalled = true;
            _cachedDragPosition = toDrag.position;
            _cachedMousePosition = Input.mousePosition;
            PointerDown?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_pointerDownCalled == false)
                return;
            toDrag.position = _cachedDragPosition + ((Vector2)Input.mousePosition - _cachedMousePosition);
            Drag?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(_pointerDownCalled == false)
                return;
            if(preventCameraControl && _cameraControl)
                _cameraControl.NoCtrlCondition = _noControlFunctionCached;
            _pointerDownCalled = false;
            PointerUp?.Invoke(eventData);
        }
    }
}
