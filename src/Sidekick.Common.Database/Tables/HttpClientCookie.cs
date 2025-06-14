﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sidekick.Common.Database.Tables;

[PrimaryKey(nameof(ClientName), nameof(Name))]
public class HttpClientCookie
{
    [Key]
    [MaxLength(64)]
    public required string ClientName { get; set; }

    [Key]
    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(1024)]
    public string? Value { get; set; }
}
