namespace Portal.Models
{
    //Used for /token endpoint
    public class TokenUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Grant_Type { get; set; }
    }
}
