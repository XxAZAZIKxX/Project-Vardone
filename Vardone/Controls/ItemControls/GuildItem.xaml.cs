﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for GuildItem.xaml
    /// </summary>
    public partial class GuildItem
    {
        public Guild guild;
        public GuildItem([NotNull] Guild guild)
        {
            InitializeComponent();
            this.guild = guild;
            if (guild.Base64Avatar is not null) Avatar.ImageSource = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(this.guild.Base64Avatar));
        }

        private void AvatarClicked(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().OpenGuild(guild);
    }
}