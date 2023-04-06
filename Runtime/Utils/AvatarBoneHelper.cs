using UnityEngine;

public static class AvatarBoneHelper 
{
    private const string ARMATURE_HIPS_LEFT_UP_LEG_BONE_NAME = "Armature/Hips/LeftUpLeg";
    private const string HALF_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Neck/Head/LeftEye";
    private const string FULL_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
    private const string HALF_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Neck/Head/RightEye";
    private const string FULL_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";
    
    public static bool IsFullBodySkeleton(Transform avatarRoot)
    {
        return avatarRoot.Find(ARMATURE_HIPS_LEFT_UP_LEG_BONE_NAME);
    }
    
    public static Transform GetLeftEyeBone(Transform avatarRoot, bool isFullBody)
    {
        return avatarRoot.Find(isFullBody ? FULL_BODY_LEFT_EYE_BONE_NAME : HALF_BODY_LEFT_EYE_BONE_NAME);
    }
    
    public static Transform GetRightEyeBone(Transform avatarRoot, bool isFullBody)
    {
        return avatarRoot.Find(isFullBody ? FULL_BODY_RIGHT_EYE_BONE_NAME : HALF_BODY_RIGHT_EYE_BONE_NAME);
    }
}
