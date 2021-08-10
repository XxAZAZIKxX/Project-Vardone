﻿using System;

namespace VardoneEntities.Models.GeneralModels.Guilds
{
    public class UpdateGuildModel
    {
        public long GuildId { get; set; }
        public string Name { get; set; } = null;
        private string _base64Image;
        public string Base64Image
        {
            get => _base64Image;
            set
            {
                if (value is not null && Convert.TryFromBase64String(value, new Span<byte>(new byte[value.Length]), out _))
                    _base64Image = value;
            }
        }
    }
}