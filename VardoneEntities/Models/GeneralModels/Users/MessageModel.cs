using System;

namespace VardoneEntities.Models.GeneralModels.Users
{
    public record MessageModel
    {
        public string Text { get; set; }
        private string _base64Image;
        public string Base64Image
        {
            get
            {
                if (_base64Image is not null && Convert.TryFromBase64String(_base64Image, new Span<byte>(new byte[_base64Image.Length]), out _))
                    return _base64Image;

                return null;
            }
            set
            {
                if (value is not null && Convert.TryFromBase64String(value, new Span<byte>(new byte[value.Length]), out _))
                    _base64Image = value;
            }
        }
    }
}