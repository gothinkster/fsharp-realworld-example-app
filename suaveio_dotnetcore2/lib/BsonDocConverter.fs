namespace RealWorld

module BsonDocConverter = 
  open MongoDB.Bson
  open MongoDB.Driver
  open RealWorld.Models

  let serializeBsonTo<'T> (doc:BsonDocument) =
    // This can't be used because it doesn't serialize the Id correctly
    MongoDB.Bson.Serialization.BsonSerializer.Deserialize<'T>(doc) 

  let toArticleList (docs:BsonDocument list option) = 
    match docs with
    | Some bdoc -> List.map serializeBsonTo<Article> bdoc |> List.toArray
    | _ -> [||]

  let toUserDetail (bdoc: BsonDocument) = { 
      username = bdoc.GetElement("username").Value.ToString()
      email = bdoc.GetElement("email").Value.ToString()
      token = bdoc.GetElement("passwordhash").Value.ToString()
      bio = bdoc.GetElement("bio").Value.ToString()
      image = bdoc.GetElement("image").Value.ToString() 
    }

  let toUser (bdocUser: BsonDocument) = 
    { user = (toUserDetail (bdocUser.GetElement("user").Value.AsBsonDocument)) }
  
  let toHash (bdocUser: BsonDocument) = 
    let currentUser = Seq.find (fun (user:BsonElement) -> user.Name = "user") bdocUser
    currentUser.Value.AsBsonDocument.GetElement("passwordhash").Value.ToString()

  let extractPasswordHash dboc = 
    let userToLogin = Seq.find (fun (user:BsonElement) -> user.Name = "user") dboc
    userToLogin.Value.AsBsonDocument.GetElement("passwordhash").Value.ToString()

  let toUserId (bdocUser: BsonDocument) =
    bdocUser.GetElement("_id").Value.ToString()
    