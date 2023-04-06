using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarLoader
{
    public struct ResponseTexture : IResponse
    {
        public Texture2D Texture;

        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }

        public void Parse(bool isSuccess, UnityWebRequest request)
        {
            IsSuccess = isSuccess;
            Texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            if (!isSuccess)
            {
                Error = request.error;
            }
        }
    }
    public class Response : IResponse
    {
        private const string LAST_MODIFIED = "Last-Modified";

        public string Text;
        public byte[] Data;
        public ulong Length;
        public string LastModified;

        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }


        public void Parse(bool isSuccess, UnityWebRequest request)
        {
            IsSuccess = isSuccess;
            if (IsSuccess)
            {
                if (!(request.downloadHandler is DownloadHandlerFile))
                {
                    Text = request.downloadHandler.text;
                    Data = request.downloadHandler.data;
                }
                if (request.downloadHandler is DownloadHandlerTexture t)
                {
                    Debug.Log(t.texture);
                }
                Length = request.downloadedBytes;
                LastModified = request.GetResponseHeader(LAST_MODIFIED);
            }
            else
            {
                Error = request.error;
            }
        }
    }

}
