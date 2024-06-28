namespace HenryMedsApp.Models
{
    public class Message
    {
        public class GenericItem<T> {
            public GenericItem(){}
            public T Items { get; set; }
            public string Message { get; set; }
            public bool Success {  get; set; }
        }
    }
}
