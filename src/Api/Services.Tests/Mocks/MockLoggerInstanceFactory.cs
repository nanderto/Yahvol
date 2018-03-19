namespace Yahvol.Services.Tests.Mocks
{
    using Yahvol.Data;

    public class MockLoggerInstanceFactory : IInstanceFactory<MockLogger>
    {
        public MockLoggerInstanceFactory()
        {
            this.Logger = new MockLogger();
        }

        public MockLogger Logger { get; set; }

        public MockLogger Create()
        {
            return this.Logger;
        }
    }
}