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

  let toUser (bdocUser: BsonDocument) = serializeBsonTo<User> bdocUser
  let toUserDetail (bdocUser: BsonDocument) = (serializeBsonTo<User> bdocUser).user