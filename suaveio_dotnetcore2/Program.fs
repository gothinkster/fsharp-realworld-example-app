open System
open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters
open Suave.Json
open RealWorld.Stubs
open System.IO
open Microsoft.Extensions.Configuration
open MongoDB.Driver
open RealWorld.Models
open RealWorld.Effects.DB
open RealWorld.Effects.Actions
open MongoDB.Bson


let serverConfig = 
  let randomPort = Random().Next(7000, 7999)
  { defaultConfig with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" randomPort] }

let validateCredentials dbClient = 
  request (fun inputGraph -> 
    let user = Suave.Json.fromJson<UserRequest> inputGraph.rawForm |> loginUser dbClient
    Successful.OK (sprintf "%A" inputGraph)
    
  )

let extractStringQueryVal (queryParameters : HttpRequest) name =
  match queryParameters.queryParam name with
  | Choice1Of2 queryVal -> queryVal
  | Choice2Of2 _ -> String.Empty

let extractNumericQueryVal (queryParameters : HttpRequest) name =
  match queryParameters.queryParam name with
  | Choice1Of2 limit -> Convert.ToInt32 limit
  | Choice2Of2 _ -> 0
  
let routeByOptions (queryParameters : HttpRequest) =
  let listArticleOptions = {
    Limit = extractNumericQueryVal queryParameters "limit";
    Tag = extractStringQueryVal queryParameters "tag";
    Author = extractStringQueryVal queryParameters "author";
    Favorited = extractStringQueryVal queryParameters "favorited";
    Offset = extractNumericQueryVal queryParameters "offset";
  } 

  // TODO: pass the options to mongo to filter properly
  (Successful.OK "successful")

let initProfile = 
  {username = ""; bio = ""; image = ""; following = false;}

let mapJsonToArticle (article : Article) dbClient = 
  createNewArticle article dbClient

//TODO: Replace each return comments with function to carry out the action.
let app (dbClient: IMongoDatabase) = 
  choose [
    POST >=> path "/users/login" >=> validateCredentials dbClient
    POST >=> path "/users" >=> registerUserNewUser dbClient
    GET  >=> path "/user" >=> getCurrentUser dbClient
    PUT  >=> path "/user" >=> updateUser dbClient
    GET  >=> pathScan "/profile/%s" (fun username -> getUserProfile dbClient username)
    // Come back to these when authentication gets implemented because it's needed to follow a user by their username
    POST >=> path "/profiles/:username/follow" >=> (Successful.OK Responses.singleProfile)
    DELETE >=> path "/profiles/:username/follow" >=> (Successful.OK Responses.singleProfile)
    GET  >=> path "/articles" >=> getArticles dbClient
    GET  >=> path "/articles/feed" >=> getArticlesForFeed dbClient
    GET  >=> pathScan "/articles/%s" (fun slug -> getArticlesBy slug dbClient)
    PUT  >=> pathScan "/articles/%s" (fun slug -> request(fun req -> addArticleWithSlug req.rawForm slug dbClient))
    DELETE >=> pathScan "/articles/%s" (fun slug -> deleteArticleBy slug dbClient)
    POST >=> pathScan "/articles/%s/comments" (fun slug -> request( fun req -> addCommentBy req.rawForm slug dbClient))  
    GET  >=> pathScan "/articles/%s/comments" (fun slug -> getCommentsBySlug slug dbClient)
    DELETE >=> pathScan "/articles/%s/comments/%s" (fun slugAndId -> deleteComment slugAndId dbClient)
    POST >=> pathScan "/articles/%s/favorite" (fun slug -> favoriteArticle slug dbClient) 
    DELETE >=> pathScan "/articles/%s/favorite" (fun slug -> removeFavoriteCurrentUser slug dbClient) 
    POST >=> path "/articles" >=> (mapJson (fun (newArticle : Article) -> mapJsonToArticle newArticle dbClient)) // Creates a new article
    GET >=> path "/tags" >=> getTagList dbClient
    path "/" >=> (Successful.OK "This will return the base page.")
  ]

open RealWorld.Effects.DB

[<EntryPoint>]
let main argv = 
  startWebServer serverConfig (RealWorld.Effects.DB.getDBClient () |> app)
  0
