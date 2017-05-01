open System
open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters
open RealWorld.Stubs

let serverConfig = 
  { defaultConfig with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8070] }

// TODO: Replace each return comments with function to carry out the action.
let app = 
  choose [
    POST >=> path "/users/login" >=> (Successful.OK Responses.usersLogin)
    POST >=> path "/users" >=> (Successful.OK Responses.usersLogin)
    GET  >=> path "/user" >=> (Successful.OK Responses.usersLogin)
    PUT  >=> path "/user" >=> (Successful.OK Responses.usersLogin)
    GET  >=> path "/profile/:username" >=> (Successful.OK Responses.singleProfile) 
    POST >=> path "/profiles/:username/follow" >=> (Successful.OK Responses.singleProfile)
    DELETE >=> path "/profiles/:username/follow" >=> (Successful.OK Responses.singleProfile)
    GET  >=> path "/articles" >=> (Successful.OK Responses.multipleArticles)
    GET  >=> path "/articles/feed" >=> (Successful.OK Responses.multipleArticles)
    GET  >=> path "/articles/:slug" >=> (Successful.OK Responses.singleArticle)
    PUT  >=> path "/articles/:slug" >=> (Successful.OK Responses.singleArticle)
    DELETE >=> path "/articles/:slug" >=> (Successful.OK Responses.singleArticle)
    POST >=> path "/articles/:slug/comments" >=> (Successful.OK Responses.singleComment)
    GET  >=> path "/articles/:slug/comments" >=> (Successful.OK Responses.multipleComments)
    DELETE >=> path "/articles/:slug/comments/:id" >=> (Successful.OK Responses.multipleComments)
    POST >=> path "/articles/:slug/favorite" >=> (Successful.OK Responses.singleArticle)
    DELETE >=> path "/articles/:slug/favorite" >=> (Successful.OK Responses.singleArticle)
    POST >=> path "/articles" >=> (Successful.OK Responses.singleArticle ) // Creates a new article
    GET >=> path "/tags" >=> (Successful.OK Responses.tagList)
    path "/" >=> (Successful.OK "This will return the base page.")
  ]

[<EntryPoint>]
let main argv =
  startWebServer serverConfig app
  0
