﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Users
{
    [Table("tokens")]
    public sealed class TokensTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("id_user"), ForeignKey("id_user"), Required] public UsersTable User { get; set; }
        [Column("token"), Required] public string Token { get; set; }
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; }
        [Column("mac_address"), Required] public string MacAddress { get; set; }
    }
}