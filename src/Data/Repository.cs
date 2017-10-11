﻿namespace Yahvol.Data
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;

    public abstract class Repository : IRepository
	{
		#region Constructors

		~Repository()
	    {
			this.Dispose(false);
	    }

		#endregion

		#region Properties

		public bool IsDisposed { get; private set; }

		public bool IsDisposing { get; private set; }

		#endregion

		#region Methods

	    public void Dispose()
	    {
			if (!this.IsDisposed && !this.IsDisposing)
		    {
			    this.IsDisposing = true;

				this.Dispose(true);

				GC.SuppressFinalize(this);

				this.IsDisposed = true;
				this.IsDisposing = false;
		    }
	    }

	    public abstract void SaveChanges();

		public abstract void Insert<T>(T item) where T : class;

		public abstract void Delete<T>(T item) where T : class;

		public abstract T Get<T>(Expression<Func<T, bool>> predicate) where T : class;

	    public abstract IQueryable<T> All<T>() where T : class;

	    public abstract IQueryable<T> GetAll<T>(Expression<Func<T, bool>> predicate) where T : class;

		protected abstract void Dispose(bool disposing);

		#endregion
	}

	public abstract class Repository<T> : Repository, IRepository<T> where T : class
	{
		#region Fields

		private T context;

		#endregion

		#region Constructors

		protected Repository() : this(new DefaultConstructorInstanceFactory<T>())
		{
		}

		protected Repository(IInstanceFactory<T> factory)
		{
			this.Factory = factory;
		}

		#endregion

		#region Properties

		public T Context
		{
			get
			{
				if (this.context == null)
				{
					this.context = this.Create();
				}

				return this.context;
			}
		}

		public IInstanceFactory<T> Factory { get; protected set; }

		#endregion

		#region Methods

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				var disposable = this.context as IDisposable;

				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		protected virtual T Create()
		{
			if (this.Factory != null)
			{
				return this.Factory.Create();
			}

			return default(T);
		}

		#endregion
	}
}
