module Integration

open Expecto
open RealWorld.Effects.DB
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Linq
open Testing
open Suave
open Suave.Web
open RealWorld.Models
open System

let runWithDefaultConfig = runWith defaultConfig

let databaseClient =
  let mongoConn : string = "mongodb://localhost:27017"
  let client = new MongoClient(mongoConn)
  client.GetDatabase("realworld")

(*
  Example of sending a json doc to the api endpoint:
  
  let sampleDoc = { id = System.Guid(validId); body = "Text" }
  let sampleDocJson = sampleDoc |> toJson

  use data = new System.Net.Http.StringContent(sampleDocJson)
  let result = runWithDefaultConfig (Program.app databaseClient) |> req HttpMethod.GET "/user" (Some data)
*)

[<Tests>]
let tests = 
  testList "Articles" [
    testCase "Should return none since it was not in the db" <| fun _ ->
      let result = getArticleBySlug databaseClient "this slug will not be in the db"
      Expect.equal result None "Did not return None from database"

    testCase "Should return fake current user" <| fun _ -> 
      let result = runWithDefaultConfig (Program.app databaseClient) |> req HttpMethod.GET "/user" None
      let user = Json.fromJson<UserRequest> (System.Text.Encoding.Unicode.GetBytes result)
       
      Expect.equal user.User.Username String.Empty "The user name is not matching."

    testCase "Should add a new article" <| fun _ -> 
      Expect.equal true true "Stub for adding new article"
  ]