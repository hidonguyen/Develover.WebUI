namespace Develover.WebUI.Models
{
    public class ModelStateError
    {
        public ModelStateError(string key, string state, string message)
        {
            Key = key;
            State = state;
            Message = message;
        }

        public string Key { get; set; }
        public string State { get; set; }
        public string Message { get; set; }
    }
}
