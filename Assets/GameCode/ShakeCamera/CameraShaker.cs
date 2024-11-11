using UnityEngine;
using System.Collections.Generic;
using System;

namespace EZCameraShake
{
    public class CameraShaker : MonoBehaviour
    {
        [Serializable] 
        public struct ShakeSettings
        {
            public bool enabled;
            [Range(0.0f, 10.0f)] public float magnitude;
            [Range(0.0f, 20.0f)] public float roughness;
            [Range(0.0f, 2.0f)] public float fadeInDuration;
            [Range(0.0f, 5.0f)] public float fadeOutDuration;

            public void Shake()
            {
                if (enabled)
                {
                    Instance.ShakeOnce(magnitude, roughness, fadeInDuration, fadeOutDuration);
                }
            }
        }

        [SerializeField] CameraPosition positionSettings;
        /// <summary>
        /// The single instance of the CameraShaker in the current scene. Do not use if you have multiple instances.
        /// </summary>
        public static CameraShaker Instance;
        static Dictionary<string, CameraShaker> instanceList = new Dictionary<string, CameraShaker>();

        /// <summary>
        /// The default position influcence of all shakes created by this shaker.
        /// </summary>
        public Vector3 DefaultPosInfluence = new Vector3(0.15f, 0.15f, 0.15f);
        /// <summary>
        /// The default rotation influcence of all shakes created by this shaker.
        /// </summary>
        public Vector3 DefaultRotInfluence = new Vector3(1, 1, 1);
        /// <summary>
        /// Offset that will be applied to the camera's default (0,0,0) rest position
        /// </summary>
        public Vector3 RestPositionOffset = new Vector3(0, 0, 0);
        /// <summary>
        /// Offset that will be applied to the camera's default (0,0,0) rest rotation
        /// </summary>
        public Vector3 RestRotationOffset = new Vector3(0, 0, 0);

        Vector3 posAddShake, rotAddShake;

        List<CameraShakeInstance> cameraShakeInstances = new List<CameraShakeInstance>();

        void Start()
        {
            Instance = this;
            instanceList.Add(gameObject.name, this);
            
        }

        public void SetRest()
        {            
            RestRotationOffset = positionSettings.transform.rotation.eulerAngles;
            RestPositionOffset = positionSettings.aspect.max.cameraPosition;
        }

        public void SetRest(Vector3 position)
        {
            RestPositionOffset = position;
        }

        public void SetNewPosition(Vector3 position)
        {
            RestPositionOffset = position;
        }

        void Update()
        {
            posAddShake = Vector3.zero;
            rotAddShake = Vector3.zero;

            for (int i = 0; i < cameraShakeInstances.Count; i++)
            {
                if (i >= cameraShakeInstances.Count)
                    break;

                CameraShakeInstance c = cameraShakeInstances[i];

                if (c.CurrentState == CameraShakeState.Inactive && c.DeleteOnInactive)
                {
                    cameraShakeInstances.RemoveAt(i);
                    i--;
                }
                else if (c.CurrentState != CameraShakeState.Inactive)
                {
                    posAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.PositionInfluence);
                    rotAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.RotationInfluence);
                }
            }

            transform.localPosition = posAddShake + RestPositionOffset;
            transform.localEulerAngles = rotAddShake + RestRotationOffset;
        }

        /// <summary>
        /// Gets the CameraShaker with the given name, if it exists.
        /// </summary>
        /// <param name="name">The name of the camera shaker instance.</param>
        /// <returns></returns>
        public static CameraShaker GetInstance(string name)
        {
            CameraShaker c;

            if (instanceList.TryGetValue(name, out c))
                return c;

            Debug.LogError("CameraShake " + name + " not found!");

            return null;
        }

        /// <summary>
        /// Starts a shake using the given preset.
        /// </summary>
        /// <param name="shake">The preset to use.</param>
        /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
        public CameraShakeInstance Shake(CameraShakeInstance shake)
        {
            cameraShakeInstances.Add(shake);
            return shake;
        }

        /// <summary>
        /// Shake the camera once, fading in and out  over a specified durations.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake.</param>
        /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
        /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
        /// <param name="fadeOutTime">How long to fade out the shake, in seconds.</param>
        /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
        public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = DefaultPosInfluence;
            shake.RotationInfluence = DefaultRotInfluence;
            cameraShakeInstances.Add(shake);

            return shake;
        }

        /// <summary>
        /// Shake the camera once, fading in and out over a specified durations.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake.</param>
        /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
        /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
        /// <param name="fadeOutTime">How long to fade out the shake, in seconds.</param>
        /// <param name="posInfluence">How much this shake influences position.</param>
        /// <param name="rotInfluence">How much this shake influences rotation.</param>
        /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
        public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            cameraShakeInstances.Add(shake);

            return shake;
        }

        /// <summary>
        /// Start shaking the camera.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake.</param>
        /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
        /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
        /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
        public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
            shake.PositionInfluence = DefaultPosInfluence;
            shake.RotationInfluence = DefaultRotInfluence;
            shake.StartFadeIn(fadeInTime);
            cameraShakeInstances.Add(shake);
            return shake;
        }

        /// <summary>
        /// Start shaking the camera.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake.</param>
        /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
        /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
        /// <param name="posInfluence">How much this shake influences position.</param>
        /// <param name="rotInfluence">How much this shake influences rotation.</param>
        /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
        public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            shake.StartFadeIn(fadeInTime);
            cameraShakeInstances.Add(shake);
            return shake;
        }

        /// <summary>
        /// Gets a copy of the list of current camera shake instances.
        /// </summary>
        public List<CameraShakeInstance> ShakeInstances
        { get { return new List<CameraShakeInstance>(cameraShakeInstances); } }

        void OnDestroy()
        {
            instanceList.Remove(gameObject.name);
        }
    }
}