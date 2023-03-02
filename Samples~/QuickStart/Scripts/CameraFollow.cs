using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    public class CameraFollow : MonoBehaviour
    {
        private const string TARGET_NOT_SET = "target not set, disabling component";
        private readonly string TAG = typeof(CameraFollow).ToString();
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform target;
        [SerializeField] private float cameraDistance = -2.4f;
        [SerializeField] private bool lookAtTarget = true;
        [SerializeField] private bool previewLookTarget = true;
        private Transform cameraContainer;
        
        private void Start()
        {
            if (target == null)
            {
                SDKLogger.LogWarning(TAG, TARGET_NOT_SET);
                enabled = false;
            }
        }
        
        private void LateUpdate()
        {
            playerCamera.transform.localPosition = Vector3.forward * cameraDistance;
            transform.position = target.position;
            if (lookAtTarget)
            {
                playerCamera.transform.LookAt(transform);
            }
        }
    }
}
