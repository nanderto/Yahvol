// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkload.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services.Commands;

    public interface IWorkload
    {
        event Func<ServiceCommandLogger> Logger;

        Action BeforeExecuteAction { get; set; }

        Action OnCompletedAction { get; set; }

        RetryPolicy RetryPolicy { get; }

        Task BeforeExecute(CancellationToken cancellationToken);

        Task<IWorkload> Execute(CancellationToken cancellationToken);

        Task OnCompletion(CancellationToken cancellationToken);

        void SetCommand(ICommand command);
    }
}