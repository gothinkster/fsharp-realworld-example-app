namespace Realworld
module Convert =
  open RealWorld.Models
  open MongoDB.Bson
  open MongoDB.Driver

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
  
  let updateUser (user:UserDetails) (result : UpdateResult option) : string  =
    match result with
    | Some _ ->  user |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString
    | None -> { Errors = { Body = [|"Error updating this user."|] } } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString
    