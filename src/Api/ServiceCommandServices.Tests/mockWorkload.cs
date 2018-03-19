using BrookfieldGrs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransfereeService.Tests
{
    public class mockWorkload : IWorkload
    {
        private ManualResetEvent manualResetEvent;
        private TimeSpan timeToExpire;
        private int numberofExceptionsToThrow;

        public bool Completed { get; set; }

        public mockWorkload(ManualResetEvent manualResetEvent)
        {
            this.manualResetEvent = manualResetEvent;
        }

        public mockWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow)
        {
            // TODO: Complete member initialization
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.numberofExceptionsToThrow = numberofExceptionsToThrow;
        }
       

        public async Task<IWorkload> Execute(System.Threading.CancellationToken cancellationToken)
        {
            await Task.Delay(100);
            this.manualResetEvent.Set();
            return this;
        }

        public void Add<T>(T command) where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
