namespace RandPic_bot.Misc
{
    public class ConfObj
    {
        private string address;
        private int port;

        public string Address
        {
            get => address;
            set => address = value;
        }

        public int Port
        {
            get => port;
            set => port = value;
        }
    }

    public class DanbooruObj
    {
        private string tags = "";
        private int limit = 0;

        public string Tags
        {
            get => tags;
            set => tags = value;
        }

        public int Limit
        {
            get => limit;
            set => limit = value;
        }
    }
    
    
}