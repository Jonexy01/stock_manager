namespace stockmanager.Models
{
    public class Response
    {
        public string detail { get; set; }
        public UserResponse? user { get; set; }

        public Stock? stock { get; set; }
    }
}
