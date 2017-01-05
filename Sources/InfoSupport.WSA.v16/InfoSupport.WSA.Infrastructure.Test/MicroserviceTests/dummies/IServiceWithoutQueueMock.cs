namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    [Microservice]
    interface IServiceWithoutQueueMock
    {
        void SomeCommandHandler(SomeCommand command);
    }

}