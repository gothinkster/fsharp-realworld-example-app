namespace RealWorld.Effects

module DB =
  open Microsoft.Extensions.Configuration
  open MongoDB.Driver
  open System.IO
  open RealWorld.Models 
  open MongoDB.Bson
  open MongoDB.Driver.Linq
  open System

  // TODO: Convert side effects to return option types
  let currentDir = Directory.GetCurrentDirectory()

  let getConfigDbConnection currentDir = 
    let builder = ConfigurationBuilder().SetBasePath(currentDir).AddJsonFile("appsettings.json")
    
    builder.Build()

  let getPassPhrase () =
    (getConfigDbConnection currentDir).GetValue<string>("jwtPassPhrase")
    
  let getSavedTagList (dbClient : IMongoDatabase) =
    let collection = dbClient.GetCollection<TagCloud>("Tags")
    let numberOfTagDocs = collection.AsQueryable().ToList().Count
    if numberOfTagDocs > 0 then Some (collection.AsQueryable().First()) else None
 
  let getSavedArticles (dbClient : IMongoDatabase) =
    // TODO: Add sort by date desc to query
    let collection = dbClient.GetCollection<BsonDocument>("Article")
    let articleList = collection.Find(Builders<BsonDocument>.Filter.Empty)
                                .ToList()
                                |> List.ofSeq
                                
    if not (List.isEmpty articleList) then Some (articleList) else None

  let getSavedFollowedArticles (dbClient : IMongoDatabase) = 
    let collection = dbClient.GetCollection<Article>("Article")
    let articleList = collection.AsQueryable()
                                .Where(fun art -> art.article.author.following)
                                .OrderByDescending(fun art -> art.article.createdAt)
                                .ToList() |> List.ofSeq
    if not (List.isEmpty articleList) then Some (articleList) else None

  let insertNewArticle (article : Article) (dbClient : IMongoDatabase) = 
    let profileDetails = BsonDocument([
                                        BsonElement("username", BsonValue.Create article.article.author.username);
                                        BsonElement("bio", BsonValue.Create article.article.author.bio);
                                        BsonElement("image", BsonValue.Create article.article.author.image);
                                        BsonElement("following", BsonValue.Create article.article.author.following);
                                      ])
    let articleDetails = BsonDocument([
                                        BsonElement("slug", BsonValue.Create article.article.slug);
                                        BsonElement("title", BsonValue.Create article.article.title);
                                        BsonElement("description", BsonValue.Create article.article.description);
                                        BsonElement("body", BsonValue.Create article.article.body);
                                        BsonElement("createdAt", BsonValue.Create article.article.createdAt);
                                        BsonElement("updatedAt", BsonValue.Create article.article.updatedAt);
                                        BsonElement("favorited", BsonValue.Create article.article.favorited);
                                        BsonElement("favoritesCount", BsonValue.Create article.article.favoritesCount);
                                        BsonElement("author", BsonValue.Create profileDetails);
                                        BsonElement("tagList", BsonValue.Create article.article.tagList);
                                      ])
    let bsonArticle = BsonDocument([
                                      BsonElement("article", BsonValue.Create articleDetails)
                                  ])

    let collection = dbClient.GetCollection<BsonDocument> "Article"
    collection.InsertOne(bsonArticle)    
    article

  let getDBClient () = 
    let mongoConn : string = (currentDir |> getConfigDbConnection).GetValue("ConnectionStrings:DefaultConnection")
    let client = MongoClient(mongoConn)
    client.GetDatabase((currentDir |> getConfigDbConnection).GetValue("ConnectionStrings:dbname"))

  (* 
    If we save docs using bson documents we won't have to create a separate record that can be passed around 
    without the password hash or id
  *)
  let registerWithBson (dbClient: IMongoDatabase) (request: UserRequest) = 
    // TODO: Add the password hash
    let details = BsonDocument ([
                                  BsonElement("username", BsonValue.Create request.user.username);
                                  BsonElement("email", BsonValue.Create request.user.email);
                                  BsonElement("token", BsonValue.Create "");
                                  BsonElement("bio", BsonValue.Create "");
                                  BsonElement("image", BsonValue.Create "");
                                  BsonElement("passwordhash", BsonValue.Create "");
                                ])
    let bsonUser = BsonDocument ([
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
    
    let requestedUser = Builders.Filter.Eq((fun doc -> doc.user.email), request.email)
    let updateUser = Builders.Update.Set((fun doc -> doc.user.bio), request.bio)
                                    .Set((fun doc -> doc.user.image), request.image)
                                    .Set((fun doc -> doc.user.username), request.username)
               
    Some (collection.UpdateOne(requestedUser, updateUser))
    
  let registerNewUser (dbClient:IMongoDatabase) (request: UserRequest) = 
    // TODO: Create hash for password
    // TODO: Check if the user already exist
    let newUser = {
      Id = BsonObjectId(ObjectId.GenerateNewId());
      user = {
                username = request.user.username;
                email = request.user.email;
                token = "";
                bio = "";
                image = "";
                PasswordHash = "";
                favorites = [||];
      }
    }
    let collection = dbClient.GetCollection<User>("Users")
    collection.InsertOne(newUser)

  let loginUser (dbClient: IMongoDatabase) (userName: string)  = 
    let collection = dbClient.GetCollection<UserDetails>("Users")
    let usernameFilter = Builders.Filter.Eq((fun doc -> doc.email), userName)
    collection.Find(usernameFilter).ToList() |> Seq.first

  let articleFilter slug = Builders.Filter.Eq((fun article -> article.article.slug), slug)

  let getArticleBySlug (dbClient: IMongoDatabase) slug = 
    let collection = dbClient.GetCollection<Article>("Article")
    collection.Find(articleFilter slug).ToList() |> Seq.first

  let deleteArticleBySlug slug (dbClient: IMongoDatabase) =
    let collection = dbClient.GetCollection<Article> "Article"
    collection.DeleteMany(articleFilter slug).DeletedCount > 0L

  let getArticleIdBySlug slug (dbClient: IMongoDatabase) = 
    let requestedArticle = getArticleBySlug dbClient slug
    match requestedArticle with
    | Some article -> Some article.Id
    | _ -> None

  let saveNewComment (comment: Comment) articleId (dbClient: IMongoDatabase) =
    let collection = dbClient.GetCollection<BsonDocument> "Comment"
    (* TODO: Add user to the saved comment *)
    let commentDetails = BsonDocument([
                                        BsonElement("id",BsonValue.Create articleId);
                                        BsonElement("createdAt",BsonDateTime.Create DateTime.Now);
                                        BsonElement("updatedAt",BsonDateTime.Create DateTime.Now);
                                        BsonElement("body",BsonValue.Create comment.comment.body);
                                      ])
    collection.InsertOne commentDetails
    comment

  let getCommentsWithArticleId art (dbClient: IMongoDatabase) = 
    let collection = dbClient.GetCollection<Comment> "Comment"
    let commentFilter = Builders.Filter.Eq((fun comment -> comment.comment.id), art.Id.ToString())
    collection.Find(commentFilter).ToList() |> List.ofSeq

  let getCommentsFromArticlesBySlug slug (dbClient: IMongoDatabase) =
    let collection = dbClient.GetCollection<Article> "Article"
    let article = collection.Find(articleFilter slug).ToList() |> Seq.first
    match article with
    | Some art -> getCommentsWithArticleId art dbClient
    | _ -> List.empty<Comment>

  let deleteWithCommentId id (dbClient: IMongoDatabase) =
    let collection = dbClient.GetCollection<Comment> "Comment"
    collection.DeleteOne(Builders.Filter.Eq((fun comment -> comment.comment.id), id)).DeletedCount > 0L
    