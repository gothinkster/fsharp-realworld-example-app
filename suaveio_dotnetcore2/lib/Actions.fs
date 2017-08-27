namespace RealWorld.Effects

module Actions = 
  open Suave
  open RealWorld.Models
  open DB
  open MongoDB.Bson
  open System.Text
  open System
  open RealWorld
  open Suave.RequestErrors
  open Newtonsoft.Json
  open RealWorld.Hash

  let jsonToString (json: 'a) = 
    Newtonsoft.Json.JsonConvert.SerializeObject(json)

  let tagQueryPart queryValue = sprintf """"article.tagList" : ["%s"]""" queryValue
  let authorQueryPart queryValue = sprintf """"article.author.username" : %s""" queryValue
  let favoriteQueryPart queryValue = sprintf """"article.author.username" : %s, "article.favorited" : true""" queryValue  

  let extractStringQueryVal (queryParameters : HttpRequest) name queryPart =
    match queryParameters.queryParam name with
    | Choice1Of2 queryVal -> " " + queryPart queryVal
    | Choice2Of2 _ -> String.Empty

  let extractNumericQueryVal (queryParameters : HttpRequest) name = 
    match queryParameters.queryParam name with
    | Choice1Of2 limit -> Convert.ToInt32 limit
    | Choice2Of2 _ -> 0  
    
  let hashPassword (request: UserRequest) = 
    {request with user = { request.user with hash = RealWorld.Hash.Crypto.fastHash request.user.password } }

  let registerNewUser dbClient = 
    request ( fun inputGraph -> 
      Newtonsoft.Json.JsonConvert.DeserializeObject<UserRequest>(inputGraph.rawForm |> System.Text.ASCIIEncoding.UTF8.GetString)
      |> hashPassword
      |> registerWithBson dbClient 
      |> RealWorld.Convert.userRequestToUser 
      |> jsonToString 
      |> Successful.OK
    )

  let currentUserByEmail dbClient email = 
    (getUser dbClient email).Value
    |> BsonDocConverter.toProfile
    |> jsonToString

  let getCurrentUser dbClient httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try  
        return! Successful.OK (currentUserByEmail dbClient token.UserName) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let updateUser dbClient  httpContext = 
     Auth.useToken httpContext (fun token -> async {
      try  
        let user = JsonConvert.DeserializeObject<UserRequest>( httpContext.request.rawForm |> System.Text.ASCIIEncoding.UTF8.GetString)
        
        updateRequestedUser dbClient user |> ignore

        return! Successful.OK (user |> jsonToString) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })
  
  let getUserProfile dbClient username httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try 
        return! Successful.OK (currentUserByEmail dbClient username) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let createNewArticle dbCLient httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try 
        let newArticle = (JsonConvert.DeserializeObject<Article>(httpContext.request.rawForm |> System.Text.Encoding.UTF8.GetString))  
    
        let checkedArticle = 
          newArticle 
          |> RealWorld.Convert.checkNullAuthor 
          |> RealWorld.Convert.checkNullSlug
          |> RealWorld.Convert.checkFavoriteIds
          |> RealWorld.Convert.addDefaultSlug
        
        insertNewArticle checkedArticle dbCLient |> ignore
    
        return! Successful.OK (checkedArticle |> jsonToString) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })


  let getArticlesBy slug dbClient =  
    printfn "Getting articles by"   
    getArticleBySlug dbClient slug
    |> RealWorld.Convert.extractArticleList
    |> jsonToString
    |> Successful.OK

  let defaultTagsIfEmpty = function
    | Some tags -> tags
    | None -> { tags = [||] }

  let defaultArticleIfEmpty = function
    | Some articles -> Array.ofList articles
    | None -> [||]

  let getTagList dbClient =
    getSavedTagList dbClient 
    |> defaultTagsIfEmpty
    |> jsonToString
    |> Successful.OK 

  let chooseQuery (ctx: HttpContext) =     
    sprintf "{%s%s%s}" 
      (extractStringQueryVal ctx.request "tag" tagQueryPart)
      (extractStringQueryVal ctx.request "author" authorQueryPart)   
      (extractStringQueryVal ctx.request "favorited" favoriteQueryPart)                  

  let getArticles dbClient httpContext =              
    Auth.useToken httpContext (fun token -> async {
      try             
        // Not very efficient but I have to do this since I could not find documentation on doing limit and skip in mongo
        let options = (extractNumericQueryVal httpContext.request "limit", extractNumericQueryVal httpContext.request "offset")
        
        let articles = 
          match options with
          | (limit, _) when limit > 0 -> 
            getSavedArticles dbClient (chooseQuery httpContext)
            |> RealWorld.BsonDocConverter.toArticleList
            |> Seq.take limit
            |> jsonToString
          | (_,offset) when offset > 0 -> 
            getSavedArticles dbClient (chooseQuery httpContext)
            |> RealWorld.BsonDocConverter.toArticleList
            |> Seq.skip offset
            |> jsonToString
          | _ -> 
            getSavedArticles dbClient (chooseQuery httpContext)
            |> RealWorld.BsonDocConverter.toArticleList
            |> jsonToString              
        
        return! Successful.OK articles httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let getArticlesForFeed dbClient httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try 
        let articles = 
          getSavedFollowedArticles dbClient
          |> defaultArticleIfEmpty
          |> jsonToString           

        return! Successful.OK articles httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })
    

  let addArticleWithSlug json (slug: string) (dbClient: MongoDB.Driver.IMongoDatabase) =
    // This should be updating an article by the slug 
    let currentArticle = json |> Suave.Json.fromJson<Article> 
    let updatedSlug = { currentArticle.article with slug = slug}
    
    insertNewArticle ({currentArticle with article = updatedSlug }) dbClient
    |> jsonToString
    |> Successful.OK

  let deleteArticleBy slug dbClient httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try  
        deleteArticleBySlug slug dbClient |> ignore
        return! Successful.OK ("") httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })    

  let addCommentBy rawJson slug dbClient  =   
    let json = rawJson |> System.Text.Encoding.UTF8.GetString
    let possibleArticleId = getArticleBySlug dbClient slug
    match possibleArticleId with
    | Some articleId -> 
      saveNewComment (JsonConvert.DeserializeObject<RequestComment> json ) (articleId.Id.ToString()) dbClient |> ignore
      Successful.OK (json |> jsonToString)
    | None -> 
      Successful.OK ({errors = {body = [|"Could not find article by slug"|]}} |> jsonToString) 

  let getCommentsBySlug slug dbClient = 
    getCommentsFromArticlesBySlug slug dbClient
    |> jsonToString
    |> Successful.OK 

  let deleteComment (_, (id: string)) dbCLient = 
    deleteWithCommentId id dbCLient
    |> jsonToString
    |> Successful.OK

  let favoriteArticle slug dbClient httpContext = 
    // TODO: Get the current user, then get the article by the slug and add the object id to the users favorite list
     Auth.useToken httpContext (fun token -> async {
      try  
        favoriteArticleForUser dbClient token.UserName slug |> ignore
        return! Successful.OK ("") httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let removeFavoriteCurrentUser slug dbClient httpContext =     
    Auth.useToken httpContext (fun token -> async {
      try         
        removeFavoriteArticleFromUser dbClient token.UserName slug |> ignore
        return! Successful.OK ("") httpContext  
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let getFollowedProfile dbClient username httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try        
        let profile = followUser dbClient token.UserName username |> Convert.userToProfile

        return! Successful.OK (profile |> jsonToString) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })

  let removeFollowedProfile dbClient username httpContext = 
    Auth.useToken httpContext (fun token -> async {
      try 
        let profile = unfollowUser dbClient token.UserName username |> Convert.userToProfile 
        return! Successful.OK (profile |> jsonToString) httpContext
      with ex ->
        return! Suave.RequestErrors.NOT_FOUND "Database not available" httpContext
    })