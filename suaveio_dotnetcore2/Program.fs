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

let validateCredentials dbClient   = RealWorld.Auth.loginWithCredentials dbClient
let updateCurrentUser dbClient     = updateUser dbClient
let userProfile dbClient username  = getUserProfile dbClient username
let followUser dbClient username   = getFollowedProfile dbClient username
let unfollowUser dbClient username = removeFollowedProfile dbClient username

let mapJsonToArticle (article : Article) dbClient = 
  createNewArticle article dbClient

let app (dbClient: IMongoDatabase) = 
  choose [
    POST >=> path "/users/login" >=> validateCredentials dbClient
    POST >=> path "/users" >=> registerNewUser dbClient
    GET  >=> path "/user" >=> getCurrentUser dbClient
    PUT  >=> path "/user" >=> updateCurrentUser dbClient
    GET  >=> pathScan "/profile/%s" (fun username -> userProfile dbClient username)
    POST >=> pathScan "/profiles/%s/follow" (fun username -> followUser dbClient username)
    DELETE >=> pathScan "/profiles/%s/follow" (fun username -> unfollowUser dbClient username)
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
    POST >=> path "/articles" >=> (mapJson (fun (newArticle : Article) -> mapJsonToArticle newArticle dbClient)) 
    GET >=> path "/tags" >=> getTagList dbClient
    path "/" >=> (Successful.OK "This will return the base page.")
  ]

open RealWorld.Effects.DB

[<EntryPoint>]
let main argv = 
  startWebServer serverConfig (RealWorld.Effects.DB.getDBClient () |> app)
  0
