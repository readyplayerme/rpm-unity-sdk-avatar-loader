using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 1.7f, -2.4f);
        [SerializeField] private Vector3 lookOffset = new Vector3(0, 0.8f);
        [SerializeField] private bool lookAtTarget = true;
        private Transform cameraContainer;

        private void Start()
        {
            if (target == null)
            {
                SDKLogger.LogWarning("CameraFollow", "lookTarget not set, disabling component");
                enabled = false;
            }
        }
        
        void LateUpdate()
        {
            camera.transform.localPosition = cameraOffset;
            transform.position = target.position + lookOffset;
            if (lookAtTarget)
            {
                camera.transform.LookAt(transform);
            }
        }
    }
}
