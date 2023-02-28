using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    public class CameraFollow : MonoBehaviour
    {
        private readonly string TAG = typeof(CameraFollow).ToString();
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform target;
        [SerializeField] private float cameraDistance = -2.4f;
        [SerializeField] private Vector3 lookOffset = new Vector3(0, 0.8f, 0);
        [SerializeField] private bool lookAtTarget = true;
        private Transform cameraContainer;
        private const string TARGET_NOT_SET = "target not set, disabling component";
        private void Start()
        {
            if (target == null)
            {
                SDKLogger.LogWarning(TAG, TARGET_NOT_SET);
                enabled = false;
            }
        }
        
        void LateUpdate()
        {
            playerCamera.transform.localPosition = Vector3.forward * cameraDistance;
            transform.position = target.position + lookOffset;
            if (lookAtTarget)
            {
                playerCamera.transform.LookAt(transform);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(target.position + lookOffset, 0.2f);
        }
    }
}
