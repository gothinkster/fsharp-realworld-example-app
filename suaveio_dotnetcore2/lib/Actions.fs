namespace RealWorld.Effects

module Actions = 
  open Suave
  open RealWorld.Models
  open DB
  open MongoDB.Bson

  let jsonToString (json: 'a) = json |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let fakeReply email = 
    {user = { email = email; token = ""; username=""; bio=""; image=""; PasswordHash=""; }; Id=(BsonObjectId(ObjectId.GenerateNewId()))  }
    
  let registerUserNewUser dbClient = 
    request ( fun inputGraph -> 
      Suave.Json.fromJson<UserRequest> inputGraph.rawForm
      |> registerWithBson dbClient 
      |> RealWorld.Convert.userRequestToUser 
      |> jsonToString 
      |> Successful.OK
    )
      
  let getCurrentUser dbClient =
    request (fun inputGraph ->
      Successful.OK (fakeReply "" |> jsonToString)
    )

  let updateUser dbClient = 
    request (fun inputGraph ->
      let userToUpdate = (Suave.Json.fromJson<User> inputGraph.rawForm).user

      userToUpdate
      |> updateRequestedUser dbClient
      |> RealWorld.Convert.updateUser userToUpdate
      |> Successful.OK
    )

  open RealWorld.Stubs
  let getUserProfile dbClient username = 
    (Successful.OK Responses.singleProfile)

  let createNewArticle (articleToAdd : Article) dbCLient = 
    // TODO: add success response
    let succesful = insertNewArticle articleToAdd dbCLient
    articleToAdd 

  let getArticlesBy slug dbClient =
    (* TODO: Add suave testing for this. *)
    getArticleBySlug dbClient slug
    |> RealWorld.Convert.extractArticleList
    |> jsonToString
    |> Successful.OK

  let defaultTagsIfEmpty = function
    | Some tags -> tags
    | None -> { tags = [||] }

  let getTagList dbClient =
    getSavedTagList dbClient 
    |> defaultTagsIfEmpty
    |> jsonToString
    |> Successful.OK 

  let getArticles dbClient = 
    getSavedArticles dbClient
    |> jsonToString
    |> Successful.OK