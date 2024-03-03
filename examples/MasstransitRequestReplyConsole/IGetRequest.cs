namespace MasstransitRequestReplyConsole
{

    internal interface IGetRequest
    {
        public string Name { get; set; }
    }

    internal class GetRequest : IGetRequest
    {
        public string Name { get; set; }
    }
}
