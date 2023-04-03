using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

public static class RenderSceneHelper 
{
    public static string GetSceneNameAsString(this AvatarRenderScene avatarRenderScene)
    {
        Debug.Log($"{RenderSceneMap[avatarRenderScene]}");
        return RenderSceneMap[avatarRenderScene];
    }
    
    public static readonly  Dictionary<AvatarRenderScene , string> RenderSceneMap = new Dictionary<AvatarRenderScene, string>
    {
        { AvatarRenderScene.FullbodyPortrait, "fullbody-posture-v1" },
        { AvatarRenderScene.HalfbodyPortrait, "halfbody-posture-v1" },
        { AvatarRenderScene.FullbodyPortraitTransparent, "fullbody-posture-v1-transparent" },
        { AvatarRenderScene.HalfbodyPortraitTransparent, "halfbody-posture-v1-transparent" },
        { AvatarRenderScene.FullBodyPostureTransparent, "fullbody-posture-v1-transparent" }
    };
}
