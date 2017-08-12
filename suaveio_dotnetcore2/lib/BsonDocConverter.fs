namespace RealWorld

module BsonDocConverter = 
  open MongoDB.Bson
  open MongoDB.Driver
  open RealWorld.Models

  let bsonToProfile (doc: BsonDocument) = {
    username = doc.GetValue("username").AsString;
    bio = doc.GetValue("bio").AsString;
    image = doc.GetValue("image").AsString;
    following = doc.GetValue("following").AsBoolean;
  }

  let bsonToArticleDetails (doc: BsonDocument) = {
    slug = doc.GetValue("slug").AsString;
    title = doc.GetValue("title").AsString;
    description = doc.GetValue("description").AsString;
    body = doc.GetValue("body").AsString;
    createdAt = System.DateTime.Parse(doc.GetValue("createdAt").AsBsonDateTime.ToString());
    updatedAt = System.DateTime.Parse(doc.GetValue("updatedAt").AsBsonDateTime.ToString());
    favorited = doc.GetValue("favorited").AsBoolean;
    favoritesCount = 0u;
    author = bsonToProfile (doc.GetValue("author").AsBsonDocument);
    tagList = Array.empty<string>;
  }

  let toArticleList (docs:BsonDocument list option) = 
    let bsonToArticle (doc: BsonDocument) =
      { Id = doc.GetValue("_id").AsObjectId; article = bsonToArticleDetails (doc.GetValue("article").AsBsonDocument) }
    match docs with
    | Some bdoc -> List.map bsonToArticle bdoc |> List.toArray
    | _ -> Array.empty<Article>
    