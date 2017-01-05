namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    [Microservice]
    public interface ICallbackMock
    {
        TestResponse SomeMethod(RequestCommand request);
        TestResponse SlowMethod(SlowRequestCommand request);

    }
}