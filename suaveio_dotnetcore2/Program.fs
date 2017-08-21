open System
open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters
open Suave.Json
open System.IO
open Microsoft.Extensions.Configuration
open MongoDB.Driver
open RealWorld.Models
open RealWorld.Effects.DB
open RealWorld.Effects.Actions
open MongoDB.Bson
open Newtonsoft.Json

let serverConfig = 
  let randomPort = Random().Next(7000, 7999)
  { defaultConfig with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" randomPort] }

// curried functions so we can pass the database client to the actions
let validateCredentials dbClient   = RealWorld.Auth.loginWithCredentials dbClient
let updateCurrentUser dbClient     = updateUser dbClient
let userProfile dbClient username  = getUserProfile dbClient username
let followUser dbClient username   = getFollowedProfile dbClient username
let unfollowUser dbClient username = removeFollowedProfile dbClient username
let articles dbClient              = getArticles dbClient
let articlesForFeed dbClient       = getArticlesForFeed dbClient
let favArticle slug dbClient       = favoriteArticle slug dbClient
let removeFavArticle slug dbClient = removeFavoriteCurrentUser slug dbClient
let mapJsonToArticle dbClient      = createNewArticle dbClient 
  
let app (dbClient: IMongoDatabase) = 
  choose [
    POST >=> path "/users/login" >=> validateCredentials dbClient
    POST >=> path "/users" >=> registerNewUser dbClient
    GET  >=> path "/user" >=> getCurrentUser dbClient
    PUT  >=> path "/user" >=> updateCurrentUser dbClient
    GET  >=> pathScan "/profile/%s" (fun username -> userProfile dbClient username)
    POST >=> pathScan "/profiles/%s/follow" (fun username -> followUser dbClient username)
    DELETE >=> pathScan "/profiles/%s/follow" (fun username -> unfollowUser dbClient username)
    GET  >=> path "/articles" >=> articles dbClient
    GET  >=> path "/articles/feed" >=> articlesForFeed dbClient
    GET  >=> pathScan "/articles/%s" (fun slug -> getArticlesBy slug dbClient)
    PUT  >=> pathScan "/articles/%s" (fun slug -> request(fun req -> addArticleWithSlug req.rawForm slug dbClient))
    DELETE >=> pathScan "/articles/%s" (fun slug -> deleteArticleBy slug dbClient)
    POST >=> pathScan "/articles/%s/comments" (fun slug -> request( fun req -> addCommentBy req.rawForm slug dbClient))  
    GET  >=> pathScan "/articles/%s/comments" (fun slug -> getCommentsBySlug slug dbClient)
    DELETE >=> pathScan "/articles/%s/comments/%s" (fun slugAndId -> deleteComment slugAndId dbClient)
    POST >=> pathScan "/articles/%s/favorite" (fun slug -> favArticle slug dbClient) 
    DELETE >=> pathScan "/articles/%s/favorite" (fun slug -> removeFavArticle slug dbClient)     
    POST >=> path "/articles" >=> mapJsonToArticle dbClient 
    GET >=> path "/tags" >=> getTagList dbClient
    path "/" >=> (Successful.OK "This will return the base page.")
  ]

open RealWorld.Effects.DB

[<EntryPoint>]
let main argv = 
  startWebServer serverConfig (RealWorld.Effects.DB.getDBClient () |> app)
  0
