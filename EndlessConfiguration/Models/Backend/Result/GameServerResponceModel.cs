namespace EndlessConfiguration.Models.Backend.Result
{
    public class GameServerResponceModel
    {
        public GameServerResponceModel(string message, bool error)
        {
            IsError = error;
            Message = message;
        }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
