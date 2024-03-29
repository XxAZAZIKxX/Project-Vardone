﻿using System.Linq;

namespace VardoneApi.Core.Checks
{
    internal static class ChannelChecks
    {
        public static bool IsChannelExists(long channelId) => Program.DataContext.Channels.Any(p => p.Id == channelId);
    }
}