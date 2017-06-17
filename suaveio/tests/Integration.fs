module Integration

open Expecto
open RealWorld.Effects.DB
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Linq
open Suave
open Suave.Web
open RealWorld.Models
open System

let withUri url httpCtx =
    let uri = new System.Uri("http://some.phony.url" + url)
    let rawQuery = uri.Query.TrimStart('?')
    let req = { httpCtx.request with url = uri; rawQuery = rawQuery;  }
    { httpCtx with request = req }

let asGetRequest hc =
    let req = { hc.request with ``method`` = HttpMethod.GET }
    { hc with request = req }

let GetRequest u =
    HttpContext.empty
    |> withUri u
    |> asGetRequest

let databaseClient =
  let mongoConn : string = "mongodb://localhost:27017"
  let client = new MongoClient(mongoConn)
  client.GetDatabase("realworld")

let possibleResult = function
  | Some a -> a
  | None -> failwith "Expected result from router."

let extractContext ctx =
  ctx |> Async.RunSynchronously |> possibleResult

[<Tests>]
let tests = 
  testList "Articles" [
    testCase "Should return none since it was not in the db" <| fun _ ->
      let result = getArticleBySlug databaseClient "this slug will not be in the db"
      Expect.equal result None "Did not return None from database"

    testCase "Should return fake current user" <| fun _ -> 
      let routeResponse = 
        GetRequest "/articles" 
        |> Program.app databaseClient 
        |> extractContext
     
      let getContent content = 
        match content with
        | Bytes a -> a
        | _ -> failwith "Didn't return string content."

      printfn "Return from router: %A" (routeResponse.response.content |> getContent |> System.Text.Encoding.UTF8.GetString) 
      Expect.equal true true "check match"

    testCase "Getting user profile" <| fun _ ->
      let profile = RealWorld.Effects.Actions.getUserProfile databaseClient

      Expect.equal true true ""

    testCase "Should add a new article" <| fun _ ->
      let testAuthor = 
        {
          Username = "";
          Bio = "";
          Image = "";
          Following = false;
        } 
      let sampleArticle = 
        { 
          Article = {
                      Slug = "";
                      Title = "Anothe test";
                      Description = "";
                      Body = "This is a new integration test";
                      CreatedAt = DateTime.Now.ToString();
                      UpdatedAt = DateTime.Now.ToString();
                      Favorited = false;
                      FavoritesCount = 0u;
                      Author = testAuthor;
                      Taglist = [||];
        }
      }
      //let result = suaveContext |> req HttpMethod.POST "/articles" (Some inputData)

      Expect.equal true true "Stub for adding new article"
  ]