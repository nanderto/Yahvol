// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceCommandService.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Akka.Actor;

    using Yahvol.Services.Commands;

    public interface IServiceCommandService
    {
        List<Subscriber> Subscribers { get; set; }

        IServiceCommandService AddCommand<T>(T command) where T : ICommand;

        IServiceCommandService AddSubscribers(Subscriber subscriber);

        IEnumerable<ServiceCommand> Get();

        ServiceCommand Get(int id);

        IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class;

        IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate, Expression<Func<ServiceCommand, bool>> userPredicate);

        IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate);

        T Run<T>() where T : ICommand;

        IServiceCommandService WithWorkload(IWorkload workload, TimeSpan timeToExpire);
    }
}