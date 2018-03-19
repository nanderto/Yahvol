// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceCommandContext.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Data.Entity;

    public interface IServiceCommandContext : IDisposable
    {
        IDbSet<LogEntry> LogEntries { get; set; }

        IDbSet<ServiceCommand> ServiceCommands { get; set; }

        IDbSet<Subscriber> Subscribers { get; set; }

        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}