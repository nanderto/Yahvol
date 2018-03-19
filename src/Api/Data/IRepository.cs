// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="N.K.Anderton">
// Copyright © 2015 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Yahvol.Data
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IRepository : IDisposable
    {
        IQueryable<T> All<T>() where T : class;

        void Delete<T>(T item) where T : class;

        T Get<T>(Expression<Func<T, bool>> predicate) where T : class;

        IQueryable<T> GetAll<T>(Expression<Func<T, bool>> predicate) where T : class;

        void Insert<T>(T item) where T : class;

        void SaveChanges();
    }

    public interface IRepository<out T> : IRepository
        where T : class
    {
        T Context { get; }

        IInstanceFactory<T> Factory { get; }
    }
}