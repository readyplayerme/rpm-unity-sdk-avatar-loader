namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This structure is used to store the response data from the WebRequestDispatcher.
    /// </summary>
    public struct Response
    {
        public string Text;
        public byte[] Data;
        public string LastModified;

        public Response(string text, byte[] data, string lastModified)
        {
            Text = text;
            Data = data;
            LastModified = lastModified;
        }
    }
}
