using UnityEngine;

namespace Legacy.Client
{
    public class MainCameraZoomBehaviour : MonoBehaviour
    {
        [SerializeField] private float cameraZoomSpeed;//потом вернуть - чтоб не лезть в battle префаб
        [SerializeField] private CameraPosition camera;
        [SerializeField] private MainCameraMoveBehaviour moveBehaviour;
        [Range(-5, 5)]
        [SerializeField] private float zoomRange;
        [SerializeField, Range(10, 100)] private float ScrollToTouchCoeff = 30.0f;

        private Vector3 clampedVectorZoom;

        void Update()
        {
#if UNITY_EDITOR
            if (zoomRange != 0)
            {
                camera.transform.Translate(Vector3.forward * zoomRange, Space.Self);

                ClampZoom();
                moveBehaviour.ClampPosition();
            }
#endif
            float deltaDistance = 0.0f;
            //if (Input.touchSupported)
            //{
            //    if (Input.touchCount == 2)
            //    {
            //        Touch touchZero = Input.GetTouch(0);
            //        Touch touchOne = Input.GetTouch(1);

            //        if (moveBehaviour.IsEnteredInTouchZone(touchZero) && moveBehaviour.IsEnteredInTouchZone(touchOne))
            //        {
            //            deltaDistance = CountDeltaDistance(touchZero, touchOne);                    
            //        }
            //    }
            //}
            //else
            {
                if (Input.mouseScrollDelta.magnitude > 0.0f)
                {
                    deltaDistance = Input.mouseScrollDelta.y * ScrollToTouchCoeff;
                }
            }

            if (deltaDistance != 0.0f)
            {
                //Debug.Log("DeltaDistance for zoomCamera: " + Input.mouseScrollDelta);
                //Debug.Log("DeltaDistance for zoomCamera: " + Input.mouseScrollDelta);

                camera.transform.Translate(Vector3.forward * deltaDistance * cameraZoomSpeed, Space.Self);

                ClampZoom();
                moveBehaviour.ClampPosition();
            }

        }

        private float CountDeltaDistance(Touch touchZero, Touch touchOne)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float deltaDistance = currentMagnitude - prevMagnitude;

            return deltaDistance;
        }

        private void ClampZoom()
        {
            float cameraY = camera.transform.position.y;

            clampedVectorZoom.x = 0;
            clampedVectorZoom.y = Mathf.Clamp(camera.transform.localPosition.y, camera.aspect.min.cameraPosition.y, camera.aspect.max.cameraPosition.y);
            clampedVectorZoom.z = Mathf.Clamp(camera.transform.localPosition.z, camera.aspect.max.cameraPosition.z, camera.aspect.min.cameraPosition.z);//press f

            camera.transform.localPosition = clampedVectorZoom;
        }
    }
}
