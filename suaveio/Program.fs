open System
open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters

let serverConfig = 
  { defaultConfig with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8070] }

// TODO: Replace each return comments with function to carry out the action.
let app = 
  choose [
    POST >=> path "/users/login" >=> (Successful.OK "Login endpoint.")
    POST >=> path "/users" >=> (Successful.OK "List of users in the system.")
    GET  >=> path "/user" >=> (Successful.OK "Return current user.")
    PUT  >=> path "/user" >=> (Successful.OK "Should update the user")
    GET  >=> path "/profile/:username" >=> (Successful.OK "Return the profile of the requested username.") 
    POST >=> path "/profiles/:username/follow" >=> (Successful.OK "Follow a user.")
    DELETE >=> path "/profiles/:username/follow" >=> (Successful.OK "Delete user via thier profile")
    GET  >=> path "/articles" >=> (Successful.OK "Return articles with query parameters")
    GET  >=> path "/articles/feed" >=> (Successful.OK "Feed articles")
    GET  >=> path "/articles/:slug" >=> (Successful.OK "Returns a single article.")
    PUT  >=> path "/articles/:slug" >=> (Successful.OK "Updates an article.")
    DELETE >=> path "/articles/:slug" >=> (Successful.OK "Updates an article.")
    POST >=> path "/articles/:slug/comments" >=> (Successful.OK "Add comments to an article")
    GET  >=> path "/articles/:slug/comments" >=> (Successful.OK "Get comments to an article")
    DELETE >=> path "/articles/:slug/comments/:id" >=> (Successful.OK "Delete Comment")
    POST >=> path "/articles/:slug/favorite" >=> (Successful.OK "Favorite an article")
    DELETE >=> path "/articles/:slug/favorite" >=> (Successful.OK "Unfavorite an article")
    POST >=> path "/articles" >=> (Successful.OK "Create a new article")
    GET >=> path "/tags" >=> (Successful.OK "Returns a list tags")
    path "/" >=> (Successful.OK "This will return the base page.")
  ]

[<EntryPoint>]
let main argv =
  startWebServer serverConfig app
  0
