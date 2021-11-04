﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.Checks;
using VardoneApi.Entity.Models.Channels;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    var guild = guilds.First(p => p.Id == guildId);
                    channels.Add(new ChannelsTable { Name = name ?? "New channel", Guild = guild });
                    dataContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

                return Ok("Created");
            }));
        }
        //
        [HttpPost, Route("deleteChannel")]
        public async Task<IActionResult> DeleteChannel([FromQuery] long channelId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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

                    var channel = channels.First(p => p.Id == channelId);
                    if (channel.Guild.Owner.Id != userId) return BadRequest();

                    channels.Remove(channel);
                    dataContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

                return Ok("Deleted");
            }));
        }
        //
        [HttpPost, Route("updateChannel")]
        public async Task<IActionResult> UpdateChannel([FromBody] UpdateChannelModel updateChannelModel)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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

                    var channel = channels.First(p => p.Id == updateChannelModel.ChannelId);
                    if (channel.Guild.Owner.Id != userId) return BadRequest("You are not owner");

                    channel.Name = updateChannelModel.Name ?? channel.Name;
                    channels.Update(channel);

                    dataContext.SaveChanges();
                    return Ok("Updated");
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (!ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");
                if (message is null) return BadRequest("Message is null");
                if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.Base64Image)) return BadRequest("Empty message");

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

                    channelMessages.Add(new ChannelMessagesTable
                    {
                        Author = users.First(p => p.Id == userId),
                        Channel = channel,
                        CreatedTime = DateTime.Now,
                        Image = message.Base64Image is not null ? Convert.FromBase64String(message.Base64Image) : null,
                        Text = message.Text ?? ""
                    });

                    dataContext.SaveChanges();
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    if (limit > 0) selectedMessages = selectedMessages.OrderByDescending(p=>p.Id).Take(limit);

                    var returnMessages = new List<ChannelMessage>();
                    foreach (var m in selectedMessages)
                    {
                        returnMessages.Add(new ChannelMessage
                        {
                            MessageId = m.Id,
                            Text = m.Text,
                            CreatedTime = m.CreatedTime,
                            Base64Image = m.Image is not null ? Convert.ToBase64String(m.Image) : null,
                            Author = new User
                            {
                                UserId = m.Author.Id,
                                Username = m.Author.Username,
                                Base64Avatar = m.Author.Info?.Avatar is not null ? Convert.ToBase64String(m.Author.Info.Avatar) : null,
                                Description = m.Author.Info?.Description
                            },
                            Channel = new Channel
                            {
                                ChannelId = m.Channel.Id,
                                Name = m.Channel.Name,
                                Guild = new Guild
                                {
                                    GuildId = m.Channel.Guild.Id,
                                    Name = m.Channel.Guild.Name,
                                    Base64Avatar = m.Channel.Guild.Info?.Avatar is not null ? Convert.ToBase64String(m.Channel.Guild.Info.Avatar) : null,
                                    Owner = new User
                                    {
                                        UserId = m.Channel.Guild.Owner.Id,
                                        Username = m.Channel.Guild.Owner.Username,
                                        Description = m.Channel.Guild.Owner.Info?.Description,
                                        Base64Avatar = m.Channel.Guild.Owner.Info?.Avatar is not null ? Convert.ToBase64String(m.Channel.Guild.Owner.Info.Avatar) : null
                                    }
                                }
                            }
                        });
                    }
                    return Ok(returnMessages);
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    channelMessages.Include(p => p.Channel.Guild.Owner).Load();

                    if (channelMessages.Count(p => p.Id == messageId) == 0) return BadRequest("Message is not exists");

                    var channelMessage = channelMessages.First(p => p.Id == messageId);

                    if (channelMessage.Author.Id != userId && channelMessage.Channel.Guild.Owner.Id != userId) return BadRequest("You cannot delete this message");

                    channelMessages.Remove(channelMessage);
                    dataContext.SaveChanges();
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