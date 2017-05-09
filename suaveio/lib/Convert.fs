namespace Realworld
module Convert =
  open RealWorld.Models
  open MongoDB.Bson

  let userRequestToUser (user: UserRequest) = 
    {
      User = {
                Username = user.User.Username;
                Email = user.User.Email;
                PasswordHash = "";
                Token = "";
                Bio = "";
                Image = "";
            };
      Id = BsonObjectId.Empty
    }