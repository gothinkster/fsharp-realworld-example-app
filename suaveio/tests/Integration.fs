module Integration

open Expecto
open RealWorld.Effects.DB
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Linq
open Testing
open Suave
open Suave.Web

let runWithDefaultConfig = runWith defaultConfig

let databaseClient =
  let mongoConn : string = "mongodb://localhost:27017"
  let client = new MongoClient(mongoConn)
  client.GetDatabase("realworld")

[<Tests>]
let tests = 
  testList "Articles" [
    testCase "Should return none since it was not in the db" <| fun _ ->
      let result = getArticleBySlug databaseClient "this slug will not be in the db"
      Expect.equal result None "Did not return None from database"

    testCase "should blow up in your face" <| fun _ ->
      let result = RealWorld.Effects.Actions.getArticlesBy "something that's not there" databaseClient
      // TODO: Use suave testing for the correct response
      printfn "Get articles result: %A" result

      Expect.equal true true "This string option from db"

    testCase "Should return fake current user" <| fun _ -> 
      (*
        Example of sending a json doc to the api endpoint:
        
        let sampleDoc = { id = System.Guid(validId); body = "Text" }
        let sampleDocJson = sampleDoc |> toJson

        use data = new System.Net.Http.StringContent(sampleDocJson)
        let result = runWithDefaultConfig (Program.app databaseClient) |> req HttpMethod.GET "/user" (Some data)
      *)
      
      let result = runWithDefaultConfig (Program.app databaseClient) |> req HttpMethod.GET "/user" None
      printfn "Request made to get the current user: %A" result   
      Expect.equal true true "this will hit the real endpoint"
  ]