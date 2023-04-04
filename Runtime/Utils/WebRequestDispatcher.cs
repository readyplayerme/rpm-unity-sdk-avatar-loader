using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for handling asynchronous WebRequests of different types include POST and GET requests.
    /// </summary>
    public class WebRequestDispatcher
    {
        private const int TIMEOUT = 20;
        private const string LAST_MODIFIED = "Last-Modified";
        private const string NO_INTERNET_CONNECTION = "No internet connection.";
        private const string CLOUDFRONT_IDENTIFIER = "cloudfront";

        public Action<float> ProgressChanged;

        private bool HasInternetConnection => Application.internetReachability != NetworkReachability.NotReachable;

        /// <summary>
        /// This asynchronous method makes a <see cref="UnityWebRequest.Put()" /> request to the provided
        /// <paramref name="url" /> and returns the response as a json <c>string</c>.
        /// </summary>
        /// <param name="url">The URL to make the <see cref="UnityWebRequest" /> to.</param>
        /// <param name="bytes">The data to send as a <c>byte[]</c>.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The response as a json <c>string</c> if successful otherwise it will throw an exception.</returns>
        public async Task<string> Dispatch(string url, byte[] bytes, CancellationToken token)
        {
            if (HasInternetConnection)
            {
                using (UnityWebRequest request = UnityWebRequest.Put(url, bytes))
                {
                    request.method = "POST";
                    request.SetRequestHeader("Content-Type", "application/json");

                    UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();
                    if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    return request.downloadHandler.text;
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        /// <summary>
        /// This asynchronous method makes GET request to the <paramref name="url" /> and returns the data in the
        /// <see cref="Response" />.
        /// </summary>
        /// <param name="url">The URL to make the <see cref="UnityWebRequest" /> to.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <param name="timeout">The number of seconds to wait for the WebRequest to finish before aborting.</param>
        /// <returns>A <see cref="Response" /> if successful otherwise it will throw an exception.</returns>
        public async Task<Response> DownloadIntoMemory(string url, CancellationToken token, int timeout = TIMEOUT)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = timeout;
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.method = UnityWebRequest.kHttpVerbGET;

                    if (!url.Contains(CLOUDFRONT_IDENTIFIER)) // Required to prevent CORS errors in WebGL
                    {
                        foreach (KeyValuePair<string, string> header in CommonHeaders.GetRequestHeaders())
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.downloadedBytes == 0 || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    return new Response(
                        request.downloadHandler.text,
                        request.downloadHandler.data,
                        request.GetResponseHeader(LAST_MODIFIED));
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        /// <summary>
        /// This asynchronous method makes a web request to the <paramref name="url" /> and stores the data into a file at
        /// <paramref name="path" />.
        /// </summary>
        /// <param name="url">The URL to make the <see cref="UnityWebRequest" /> to.</param>
        /// <param name="path">Where to create the file and store the response data.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <param name="timeout">The number of seconds to wait for the WebRequest to finish before aborting.</param>
        /// <returns>A <see cref="Response" /> with the data included if successful otherwise it will throw an exception.</returns>
        public async Task<Response> DownloadIntoFile(string url, string path, CancellationToken token, int timeout = TIMEOUT)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = timeout;
                    var downloadHandler = new DownloadHandlerFile(path);
                    downloadHandler.removeFileOnAbort = true;
                    request.downloadHandler = downloadHandler;

                    if (!url.Contains(CLOUDFRONT_IDENTIFIER)) // Required to prevent CORS errors in WebGL
                    {
                        foreach (KeyValuePair<string, string> header in CommonHeaders.GetRequestHeaders())
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.downloadedBytes == 0 || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    var byteLength = (long) request.downloadedBytes;
                    var info = new FileInfo(path);

                    while (info.Length != byteLength)
                    {
                        info.Refresh();
                        await Task.Yield();
                    }

                    // Reading file since can't access raw bytes from downloadHandler
                    var bytes = File.ReadAllBytes(path);

                    return new Response(
                        string.Empty,
                        bytes,
                        request.GetResponseHeader(LAST_MODIFIED));
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        /// <summary>
        /// This asynchronous method makes a web request to the <paramref name="url" /> and returns the data as a
        /// <see cref="Texture2D" />.
        /// </summary>
        /// <param name="url">The URL to make the <see cref="UnityWebRequest" /> to.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <param name="timeout">Used to set how long to wait before the request will time out</param>
        /// <returns>The response data as a <see cref="Texture2D" /> if successful otherwise it will throw an exception.</returns>
        public async Task<Texture2D> DownloadTexture(string url, CancellationToken token, int timeout = TIMEOUT)
        {
            if (HasInternetConnection)
            {
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
                {
                    request.timeout = timeout;
                    UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();
                    var errorDetected = request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError;

                    if (!errorDetected && request.downloadHandler is DownloadHandlerTexture downloadHandlerTexture)
                    {
                        return downloadHandlerTexture.texture;
                    }
                    
                    throw new CustomException(FailureType.DownloadError, request.error);
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }
    }
}
