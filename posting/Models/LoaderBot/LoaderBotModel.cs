using posting.Models.Directions;

namespace posting.Models.LoaderBot
{
    public class LoaderBotModel
    {
        public string token { get; set; }
        public string access_hash { get; set; } 
        public int direction_id {  get; set; }          
    }
}
