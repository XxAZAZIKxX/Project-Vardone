using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.Channels;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ChannelsController : ControllerBase
    {
        [HttpPost, Route("createChannel")]
        public async Task<IActionResult> CreateChannel([FromQuery] long guildId, [FromQuery] string name = null)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("Not owner");
                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;
                    var channels = dataContext.Channels;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.User).Load();
                    guildMembers.Include(p => p.Guild).Load();
                    var guild = guilds.First(p => p.Id == guildId);
                    var newChannel = new ChannelsTable { Name = string.IsNullOrWhiteSpace(name) ? "New channel" : name, Guild = guild };
                    channels.Add(newChannel);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewChannel,
                            data = GuildCreateHelper.GetChannel(newChannel.Id)
                        };
                        foreach (var id in guildMembers.Where(p => p.Guild.Id == guildId).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
                    return Ok("Created");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("deleteChannel")]
        public async Task<IActionResult> DeleteChannel([FromQuery] long channelId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");
                try
                {
                    var dataContext = Program.DataContext;
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Owner).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();

                    var channel = channels.FirstOrDefault(p => p.Id == channelId);
                    if (channel is null) return BadRequest("Channe is not exists");
                    if (channel.Guild.Owner.Id != userId) return BadRequest("You are not the guild owner");

                    var membersToNotify = guildMembers.Where(p => p.Guild.Id == channel.Guild.Id).Select(p => p.User.Id).ToArray();
                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.DeleteChannel,
                        data = GuildCreateHelper.GetChannel(channel.Id)
                    };
                    channels.Remove(channel);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        foreach (var id in membersToNotify) Program.TcpServer.SendMessageTo(id, tcpNotify);
                    });
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            }));
        }
        //
        [HttpPost, Route("updateChannel")]
        public async Task<IActionResult> UpdateChannel([FromBody] UpdateChannelModel updateChannelModel)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!ChannelChecks.IsChannelExists(updateChannelModel.ChannelId)) return BadRequest("Channel is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Owner).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();

                    var channel = channels.First(p => p.Id == updateChannelModel.ChannelId);
                    if (channel.Guild.Owner.Id != userId) return BadRequest("You are not owner");

                    channel.Name = string.IsNullOrWhiteSpace(updateChannelModel.Name) ? channel.Name : updateChannelModel.Name;
                    channels.Update(channel);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.UpdateChannel,
                            data = GuildCreateHelper.GetChannel(channel.Id)
                        };
                        foreach (var id in guildMembers.Where(p => p.Guild.Id == channel.Guild.Id).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
                    return Ok("Updated");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getChannel")]
        public async Task<IActionResult> GetChannel([FromQuery] long channelId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Token invalid");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Info).Load();
                    channels.Include(p => p.Guild.Owner).Load();
                    channels.Include(p => p.Guild.Owner.Info).Load();

                    var channel = channels.FirstOrDefault(p => p.Id == channelId);
                    if (channel is null) return BadRequest("Channel is not exists");
                    if (!GuildChecks.IsUserMember(token.UserId, channel.Guild.Id)) return BadRequest("You are not a member");

                    return Ok(GuildCreateHelper.GetChannel(channel.Id));
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("sendChannelMessage")]
        public async Task<IActionResult> SendChannelMessage([FromQuery] long channelId, [FromBody] MessageModel message)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");
                if (message is null) return BadRequest("Message is null");
                if (string.IsNullOrWhiteSpace(message.Text) &&
                    (string.IsNullOrWhiteSpace(message.Base64Image) ||
                     !ImageWorker.IsImage(Convert.FromBase64String(message.Base64Image))))
                    return BadRequest("Empty message");
                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var channelMessages = dataContext.ChannelMessages;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();

                    var channel = channels.First(p => p.Id == channelId);
                    var guild = channel.Guild;

                    if (!guildMembers.Any(p => p.User.Id == userId && p.Guild == guild)) return BadRequest("You are not member this guild");

                    var messageTable = new ChannelMessagesTable
                    {
                        Author = users.First(p => p.Id == userId),
                        Channel = channel,
                        CreatedTime = DateTime.Now,
                        Image = message.Base64Image is not null ? ImageWorker.CompressImageQualityLevel(Convert.FromBase64String(message.Base64Image), 70) : null,
                        Text = message.Text ?? ""
                    };
                    channelMessages.Add(messageTable);
                    dataContext.SaveChanges();

                    Task.Run(() =>
                    {
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewChannelMessage,
                            data = MessageCreateHelper.GetChannelMessage(messageTable.Id)
                        };
                        foreach (var id in guildMembers.Where(p => p.Guild.Id == channel.Guild.Id).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
                    return Ok("Message was sent");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getChannelMessages")]
        public async Task<IActionResult> GetChannelMessages([FromQuery] long channelId, [FromQuery] int limit = 0, [FromQuery] long startFrom = 0)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Author.Info).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    channelMessages.Include(p => p.Channel.Guild.Owner).Load();
                    channelMessages.Include(p => p.Channel.Guild.Owner.Info).Load();
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();

                    var channel = channels.First(p => p.Id == channelId);

                    if (guildMembers.Count(p => p.Guild == channel.Guild && p.User.Id == userId) == 0) return BadRequest("You are not a member guild");

                    var selectedMessages = channelMessages.Where(p => p.Channel.Id == channelId);
                    if (startFrom > 0) selectedMessages = selectedMessages.Where(p => p.Id < startFrom);
                    if (limit > 0) selectedMessages = selectedMessages.OrderByDescending(p => p.Id).Take(limit);

                    var returnMessages = new List<ChannelMessage>();
                    foreach (var m in selectedMessages)
                    {
                        var item = MessageCreateHelper.GetChannelMessage(m.Id);
                        returnMessages.Add(item);
                    }
                    return Ok(returnMessages.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getChannelMessage")]
        public async Task<IActionResult> GetChannelMessage([FromQuery] long messageId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Token invalid");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Author.Info).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    channelMessages.Include(p => p.Channel.Guild.Info).Load();

                    var message = channelMessages.FirstOrDefault(p => p.Id == messageId);
                    if (message is null) return BadRequest("Message is not exists");
                    if (!GuildChecks.IsUserMember(token.UserId, message.Channel.Guild.Id)) return BadRequest("You are not a member");

                    var returnMessage = MessageCreateHelper.GetChannelMessage(messageId);
                    return Ok(returnMessage);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("deleteChannelMessage")]
        public async Task<IActionResult> DeleteChannelMessage([FromQuery] long messageId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    channelMessages.Include(p => p.Channel.Guild.Owner).Load();

                    if (channelMessages.Count(p => p.Id == messageId) == 0) return BadRequest("Message is not exists");
                    var channelMessage = channelMessages.FirstOrDefault(p => p.Id == messageId);
                    if (channelMessage is null) return BadRequest("Message is not exists");
                    if (!guildMembers.Any(p => p.Guild.Id == channelMessage.Channel.Guild.Id && p.User.Id == userId)) return BadRequest("You are not member");
                    if (channelMessage.Author.Id != userId && channelMessage.Channel.Guild.Owner.Id != userId) return BadRequest("You cannot delete this message");

                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.DeleteChannelMessage,
                        data = MessageCreateHelper.GetChannelMessage(channelMessage.Id, true)
                    };
                    channelMessages.Remove(channelMessage);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        foreach (var id in guildMembers.Where(p => p.Guild.Id == channelMessage.Channel.Guild.Id).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
    }
}