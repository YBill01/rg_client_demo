using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using EZCameraShake;
using DG.Tweening;

namespace Legacy.Client
{
    public class MainCameraMoveBehaviour : MonoBehaviour
    {
        public static MainCameraMoveBehaviour instance;

        [SerializeField] CameraShaker shaker;
        [SerializeField] private Transform moveContainer;
        [SerializeField] private CameraPosition camera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Canvas touchZone;
        [SerializeField, Range(0, 1)] private float cameraMoveSpeed = 0.2f;

        private bool isMoving = false;

        private bool canAccessToCamera;
        private Vector3 mouseOrigin;
        private Vector3 clampedVectorMove = Vector3.zero;
        private float elapsedTime = 0f;
        private float elapsedTimeRotate = 0f;

        private const float deltavectorToMove = 12f;

        private void Start()
        {
            instance = this;
            RemoveAccesFromCamera();
            //    cameraPivot.transform.localRotation = Quaternion.Euler(-2.5f, 0, 0);
        }

        public void StartZoomCamera(float requiredTime, float distanceInPersentage)
        {
            Vector3 newPosition = camera.GetPersentagePosition(distanceInPersentage);
            camera.transform.DOKill();
            camera.transform.DOMove(newPosition, requiredTime)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() => {
                    shaker.SetRest(camera.transform.position);
                });
        }

        private void Update()
        {
            if (!canAccessToCamera) return;
            float delta = 0.0f;
            if (Input.touchSupported)
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began && canAccessToCamera && IsEnteredInTouchZone(touch))
                    {
                        isMoving = true;
                    }
                    if (touch.phase == TouchPhase.Moved && canAccessToCamera && isMoving)
                    {
                        delta = touch.deltaPosition.magnitude;
                    }
                }
                else
                {
                    isMoving = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && IsEnteredInTouchZone())
                {
                    isMoving = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isMoving = false;
                }
                if (Input.GetMouseButton(0) && isMoving)
                {
                    delta = (Input.mousePosition - mouseOrigin).magnitude;
                }
            }
            //Debug.Log("Delta magnitude to move camera: " + delta);

            //var speed = camera.GetAdaptableMoveToZoomSpeed(cameraMoveSpeed);

            if (delta > deltavectorToMove && isMoving)
            {
                float multiplier = cameraMoveSpeed * delta / 10;
                Vector3 direction = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
                moveContainer.Translate(-direction.normalized.x * multiplier, 0, -direction.normalized.y * multiplier, Space.World);

                ClampPosition();
            }
            mouseOrigin = Input.mousePosition;
        }

        public void ClampPosition()
        {
            var deltaX = camera.GetCurrentPositionDelta().Item1;
            var deltaZ = camera.GetCurrentPositionDelta().Item2;

            clampedVectorMove.x = Mathf.Clamp(moveContainer.position.x, -deltaX, deltaX);
            clampedVectorMove.y = 0;
            clampedVectorMove.z = Mathf.Clamp(moveContainer.position.z, -deltaZ, deltaZ);

            moveContainer.position = clampedVectorMove;
        }

        public bool IsEnteredInTouchZone()
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;

            EventSystem.current.RaycastAll(pointerData, results);

            return Convert.ToBoolean(results.Any(x => x.gameObject.GetComponent<Canvas>() == touchZone));
        }

        public bool IsEnteredInTouchZone(Touch touch)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = touch.position;

            EventSystem.current.RaycastAll(pointerData, results);

            return Convert.ToBoolean(results.Any(x => x.gameObject.GetComponent<Canvas>() == touchZone));
        }

        public void SetAccesToCamera()
        {
            canAccessToCamera = true;
        }
        private void RemoveAccesFromCamera()
        {
            canAccessToCamera = false;
        }
    }
}
