// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkloadBase.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services.Commands;

    /// <summary>
    /// Base class for workloads. This class ensures that commands that are added to the serviceCommandService are available to your workload
    /// Override the Execute method to perform the work that you want completed.
    /// When inheriting from this class you can pass in any components that you need on the constructor. Typically you pass in repositories,
    /// service agents, messaging resources, what ever is needed for the execution of the method should be passed into the constructor.
    /// A workload should be used for one type of functional area. For example, a command may be submitted and require changes in the DB and in AD.
    /// This command should have two workloads, one for the DB updates and one for AD. If a command requires two changes in AD, 
    /// those should be handled in one workload.
    /// </summary>
    /// <typeparam name="TCommand">
    /// Implementation of Command object. Each implementation of WorkloadBase can handle only one type of command.
    /// </typeparam>
    public abstract class WorkloadBase<TCommand> : IWorkload
        where TCommand : CommandBase, ICommand
    {
        private ICommand command;

        private Dictionary<string, Action> commandHandlers;

        private Dictionary<string, Action> events;

        public WorkloadBase(RetryPolicy retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
        }

        public event Func<ServiceCommandLogger> Logger = () => null;

        public Action BeforeExecuteAction { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public TCommand Command => (TCommand)this.command;

        public Action OnCompletedAction { get; set; }

        public RetryPolicy RetryPolicy { get; private set; }

        public Subscriber Subscriber { get; set; }

        private Dictionary<string, Action> CommandHandlers => this.commandHandlers ?? (this.commandHandlers = new Dictionary<string, Action>());

        private Dictionary<string, Action> Events
        {
            get
            {
                if (this.events == null)
                {
                    this.events = new Dictionary<string, Action>();
                }

                return this.events;
            }
        }

        /// <summary>
        /// Override this method to get work done.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>returns an task of the workload that is being executed</returns>
        public async Task BeforeExecute(CancellationToken cancellationToken)
        {
            var before = this.Events.FirstOrDefault(e => e.Key == "BeforeExecute").Value;
            before?.DynamicInvoke();
        }

        /// <summary>Handle a command with no CommandType of Action specified.</summary>
        public virtual void DefaultAction()
        {
        }

        public virtual async Task<IWorkload> Execute(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;

            if (this.CancellationToken.CanBeCanceled && this.CancellationToken.IsCancellationRequested)
            {
                return this;
            }

            Action actionMethod;
            if (!this.CommandHandlers.TryGetValue(this.Command.CommandAction, out actionMethod))
            {
                actionMethod = this.DefaultAction;
            }

            this.LogMessage($"Executing {this.GetType().Name}.{actionMethod.Method.Name}", "Yahvol.Services.WorkloadBase");
            await Task.Run(() => actionMethod(), cancellationToken);

            return this;
        }

        /// <summary>Handle an implementation of Command.</summary>
        /// <param name="commandAction">The Command's CommandAction</param>
        /// <param name="actionMethod">The Action to call within the Workload implementation</param>
        public void HandleCommand(string commandAction, Action actionMethod)
        {
            if (!this.CommandHandlers.ContainsKey(commandAction))
            {
                this.CommandHandlers.Add(commandAction, actionMethod);
            }
            else
            {
                if (commandAction != string.Empty)
                {
                    throw new ArgumentException("Command handler already exists.");
                }
            }
        }

        public void LogError(string exception, string source = null)
        {
            var logger = this.Logger();
            if (logger != null)
            {
                logger.Log(exception, source ?? this.GetType().ToString());
            }
        }

        public void LogMessage(string message, string source = null)
        {
            var logger = this.Logger();
            if (logger != null)
            {
                logger.Log(message, source ?? this.GetType().ToString());
            }
        }

        public async Task OnCompletion(CancellationToken cancellationToken)
        {
            if (this.OnCompletedAction != null)
            {
                await Task.Run(() => this.OnCompletedAction.DynamicInvoke(), cancellationToken);
            }
        }

        public void RegisterEvent(string name, Action action)
        {
            this.Events.Add(name, action);
        }

        /// <summary>
        /// Set Command is called by framework to add the command to the workload. THis should not be called by workloads.
        /// </summary>
        /// <param name="commandToSet">The command too add to the workload. All workloads require a command. 
        /// The command carries the data that the workload uses to complete its work</param>
        public void SetCommand(ICommand commandToSet)
        {
            this.command = commandToSet;
        }
    }
}