using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public  class UserModel
    {
            public string Email { get; set; }
            public string Password { get; set; }
    
    }

    public class UserModelResponse : BaseResponse
    {
        public User User { get; set; }
        public string  Token { get; set; }
    }
}
