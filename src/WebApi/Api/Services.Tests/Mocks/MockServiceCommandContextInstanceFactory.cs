namespace Yahvol.Services.Tests.Mocks
{
    using Yahvol.Data;

    public class MockServiceCommandContextInstanceFactory : IInstanceFactory<MockServiceCommandContext>
    {
        public MockServiceCommandContextInstanceFactory()
        {
            this.Context = new MockServiceCommandContext();
        }

        public MockServiceCommandContext Context { get; set; }

        public MockServiceCommandContext Create()
        {
            return this.Context;
        }
    }
}