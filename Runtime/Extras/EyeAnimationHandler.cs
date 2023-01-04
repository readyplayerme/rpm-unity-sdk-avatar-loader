using System.Collections;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class adds a blink animation at regular intervals to an avatar <c>SkeletonMeshRenderer</c> using blendshapes and bone rotation adjustments.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Ready Player Me/Eye Animation Handler", 0)]
    public class EyeAnimationHandler : MonoBehaviour
    {

        private const int VERTICAL_MARGIN = 15;
        private const int HORIZONTAL_MARGIN = 5;
        private const string EYE_BLINK_LEFT_BLEND_SHAPE_NAME = "eyeBlinkLeft";
        private const string EYE_BLINK_RIGHT_BLEND_SHAPE_NAME = "eyeBlinkRight";
        private const string HALF_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Neck/Head/LeftEye";
        private const string FULL_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
        private const string HALF_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Neck/Head/RightEye";
        private const string FULL_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";
        private const string ARMATURE_HIPS_LEFT_UP_LEG_BONE_NAME = "Armature/Hips/LeftUpLeg";
        private const float EYE_BLINK_MULTIPLIER = 100f;
        private const float HALFBODY_OFFSET_X = 90;
        private const float HALFBODY_OFFSET_Z = 180;
        
        [SerializeField, Range(0, 1)][Tooltip("Effects the duration of the avatar blink animation in seconds.")] 
        private float blinkDuration = 0.1f;

        [SerializeField, Range(1, 10)][Tooltip("Effects the amount of time in between each blink.")]  
        private float blinkInterval = 3f;

        private WaitForSeconds blinkDelay;
        private Coroutine blinkCoroutine;

        private SkinnedMeshRenderer headMesh;
        private int eyeBlinkLeftBlendShapeIndex = -1;
        private int eyeBlinkRightBlendShapeIndex = -1;

        private Transform leftEyeBone;

        private Transform rightEyeBone;

        private bool isFullBody;
        private bool hasEyeBlendShapes;

        public float BlinkDuration
        {
            get => blinkDuration;
            set
            {
                blinkDuration = value;
                if (Application.isPlaying) Initialize();
            }
        }

        public float BlinkInterval
        {
            get => blinkDuration;
            set
            {
                blinkInterval = value;
                if (Application.isPlaying) Initialize();
            }
        }

        /// <summary>
        /// This method is used to setup the coroutine and repeating functions. 
        /// </summary>
        public void Initialize()
        {
            blinkDelay = new WaitForSeconds(blinkDuration);

            CancelInvoke();
            InvokeRepeating(nameof(AnimateEyes), 1, blinkInterval);
        }

        /// <summary>
        /// This method is called when the scene is loaded and is used to setup properties and references.
        /// </summary>
        private void Start()
        {
            headMesh = gameObject.GetMeshRenderer(MeshType.HeadMesh);

            eyeBlinkLeftBlendShapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EYE_BLINK_LEFT_BLEND_SHAPE_NAME);
            eyeBlinkRightBlendShapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EYE_BLINK_RIGHT_BLEND_SHAPE_NAME);

            hasEyeBlendShapes = eyeBlinkLeftBlendShapeIndex > -1 && eyeBlinkRightBlendShapeIndex > -1;

            isFullBody = transform.Find(ARMATURE_HIPS_LEFT_UP_LEG_BONE_NAME);
            leftEyeBone = transform.Find(isFullBody ? FULL_BODY_LEFT_EYE_BONE_NAME : HALF_BODY_LEFT_EYE_BONE_NAME);
            rightEyeBone = transform.Find(isFullBody ? FULL_BODY_RIGHT_EYE_BONE_NAME : HALF_BODY_RIGHT_EYE_BONE_NAME);
            if (leftEyeBone == null || rightEyeBone == null)
            {
                Debug.Log("No eyebones found, disabling EyeAnimationHandler");
                enabled = false;
                CancelInvoke();
                blinkCoroutine?.Stop();
            }
        }
        
        private void OnEnable()
        {
            Initialize();
        }
        
        private void OnDisable()
        {
            CancelInvoke();
        }
        
        private void OnDestroy()
        {
            CancelInvoke();
            blinkCoroutine?.Stop();
        }

        /// <summary>
        /// Rotates the eyes and assigns the blink coroutine. Called in the Initialize method. 
        /// </summary>
        private void AnimateEyes()
        {
            RotateEyes();

            if (hasEyeBlendShapes)
            {
                blinkCoroutine = BlinkEyes().Run();
            }
        }

        /// <summary>
        ///  Rotates the eye bones in a random direction. 
        /// </summary>
        private void RotateEyes()
        {
            float vertical = Random.Range(-VERTICAL_MARGIN, VERTICAL_MARGIN);
            float horizontal = Random.Range(-HORIZONTAL_MARGIN, HORIZONTAL_MARGIN);

            Quaternion rotation = isFullBody
                ? Quaternion.Euler(horizontal, vertical, 0)
                : Quaternion.Euler(horizontal - HALFBODY_OFFSET_X, 0, vertical + HALFBODY_OFFSET_Z);

            leftEyeBone.localRotation = rotation;
            rightEyeBone.localRotation = rotation;
        }

        /// <summary>
        /// A coroutine that manipulates BlendShapes to open and close the eyes. 
        /// </summary>
        private IEnumerator BlinkEyes()
        {
            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendShapeIndex, EYE_BLINK_MULTIPLIER);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendShapeIndex, EYE_BLINK_MULTIPLIER);

            yield return blinkDelay;

            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendShapeIndex, 0);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendShapeIndex, 0);
        }
    }
}
