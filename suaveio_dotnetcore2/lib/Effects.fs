namespace RealWorld.Effects

module DB =
  open Microsoft.Extensions.Configuration
  open MongoDB.Driver
  open System.IO
  open RealWorld.Models 
  open MongoDB.Bson
  open MongoDB.Driver.Linq

  // TODO: Convert side effects to return option types
  let currentDir = Directory.GetCurrentDirectory()

  let getConfigDbConnection currentDir = 
    let builder = ConfigurationBuilder().SetBasePath(currentDir).AddJsonFile("appsettings.json")
    builder.Build()
    
  let getSavedTagList (dbClient : IMongoDatabase) =
    let collection = dbClient.GetCollection<TagCloud>("Tags")
    let numberOfTagDocs = collection.AsQueryable().ToList().Count
    if numberOfTagDocs > 0 then Some (collection.AsQueryable().First()) else None
    
  let getArticleViaOptions options = 
    ()

  let getSavedArticles (dbClient : IMongoDatabase) =
    let collection = dbClient.GetCollection<Article>("Article")
    collection.AsQueryable().ToList() |> List.ofSeq

  let insertNewArticle (article : Article) (dbClient : IMongoDatabase) = 
    let articleDetails = BsonDocument([
                                        BsonElement("slug", BsonValue.Create article.Article.Slug);
                                        BsonElement("title", BsonValue.Create article.Article.Title);
                                        BsonElement("description", BsonValue.Create "");
                                        BsonElement("body", BsonValue.Create "");
                                        BsonElement("createdat", BsonValue.Create "");
                                        BsonElement("updateat", BsonValue.Create "");
                                        BsonElement("favorited", BsonValue.Create "");
                                        BsonElement("favoritesCount", BsonValue.Create 0);
                                        BsonElement("author", BsonValue.Create "");
                                        BsonElement("tagList", BsonValue.Create [||]);
                                      ])
    let bsonArticle = BsonDocument([
                                      BsonElement("article", BsonValue.Create articleDetails)
                                  ])

    let collection = dbClient.GetCollection<BsonDocument> "Article"
    collection.InsertOne(bsonArticle)    
    article

  let getDBClient () = 
    let mongoConn : string = (currentDir |> getConfigDbConnection).GetValue("ConnectionStrings:DefaultConnection")
    let client = new MongoClient(mongoConn)
    client.GetDatabase((currentDir |> getConfigDbConnection).GetValue("ConnectionStrings:dbname"))

  (* 
    If we save docs using bson documents we won't have to create a separate record that can be passed around 
    without the password hash or id
  *)
  let registerWithBson (dbClient: IMongoDatabase) (request: UserRequest) = 
    // TODO: Add the password hash
    let details = BsonDocument ([
                                  BsonElement("username", BsonValue.Create request.User.Username);
                                  BsonElement("email", BsonValue.Create request.User.Email);
                                  BsonElement("token", BsonValue.Create "");
                                  BsonElement("bio", BsonValue.Create "");
                                  BsonElement("image", BsonValue.Create "");
                                  BsonElement("passwordhash", BsonValue.Create "");
                                ])
    let bsonUser = BsonDocument ([
                                  BsonElement("_id" , BsonObjectId(ObjectId.GenerateNewId()) );
                                  BsonElement("user", details)
                                ])
    let collection = dbClient.GetCollection<BsonDocument>("Users")
    collection.InsertOne bsonUser
    request

  let getCurrentUser (dbClient: IMongoDatabase) request = 
    // TODO: Create handle to grab the current user
    (* TODO: Implement after adding the authentication lib *)
    None

  let updateRequestedUser (dbClient : IMongoDatabase) (request : UserDetails) = 
    let collection = dbClient.GetCollection<User> "Users"
    
    let requestedUser = Builders.Filter.Eq((fun doc -> doc.User.Email), request.Email)
    let updateUser = Builders.Update.Set((fun doc -> doc.User.Bio), request.Bio)
                                    .Set((fun doc -> doc.User.Image), request.Image)
                                    .Set((fun doc -> doc.User.Username), request.Username)
               
    Some (collection.UpdateOne(requestedUser, updateUser))
    
  let registerNewUser (dbClient:IMongoDatabase) (request: UserRequest) = 
    // TODO: Create hash for password
    // TODO: Check if the user already exist
    let newUser = {
      Id = BsonObjectId(ObjectId.GenerateNewId());
      User = {
                Username = request.User.Username;
                Email = request.User.Email;
                Token = "";
                Bio = "";
                Image = "";
                PasswordHash = "";
      }
    }
    let collection = dbClient.GetCollection<User>("Users")
    collection.InsertOne(newUser)

  let loginUser (dbClient: IMongoDatabase) (request: UserRequest)  = 
    let collection = dbClient.GetCollection<UserDetails>("Users")

    // TODO: Finish JWT authentication; Below is an example of how to use filters for mongo
    //let passwordFilter = Builders.Filter.Eq((fun doc -> doc.Password), request.Password)
    let usernameFilter = Builders.Filter.Eq((fun doc -> doc.Email), request.User.Email)
    //let filter = Builders.Filter.And(passwordFilter, usernameFilter)
    // Return collections to avoid leaking nulls into your program from C#.
    collection.Find(usernameFilter).ToList() |> Seq.first

  let getArticleBySlug (dbClient: IMongoDatabase) slug = 
    let collection = dbClient.GetCollection<Article>("Article")
    let articleFilter = Builders.Filter.Eq((fun article -> article.Article.Slug), slug)
    collection.Find(articleFilter).ToList() |> Seq.first