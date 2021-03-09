using System;

namespace VardoneEntities.Models.GeneralModels.PrivateChats
{
    public class PrivateMessageModel
    {
        public string Text { get; set; } = null;
        private string _base64Image;
        public string Base64Image
        {
            get => _base64Image;
            set
            {
                if (Convert.TryFromBase64String(value, new Span<byte>(new byte[value.Length]), out _))
                    _base64Image = value;
            }
        }
    }
}