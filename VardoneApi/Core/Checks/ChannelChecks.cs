﻿using System;
using System.Linq;

namespace VardoneApi.Core.Checks
{
    public static class ChannelChecks
    {
        public static bool IsChannelExists(long channelId)
        {
            var dataContext = Program.DataContext;
            var channels = dataContext.Channels;
            try
            {
                var _ = channels.First(p => p.Id == channelId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}