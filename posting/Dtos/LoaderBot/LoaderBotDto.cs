using System.Text.Json.Serialization;

namespace posting.Dtos.LoaderBot
{
    public class LoaderBotDto
    {        
        public string? token { get; set; }        
        public string? username {  get; set; }  
        public int direction_id { get; set; }
        public bool is_active { get; set; }
    }
}
