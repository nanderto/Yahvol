// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceCommandContext.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public interface IServiceCommandContext : IDisposable
    {
        IDbSet<LogEntry> LogEntries { get; set; }

        IDbSet<ServiceCommand> ServiceCommands { get; set; }

        IDbSet<Subscriber> Subscribers { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}