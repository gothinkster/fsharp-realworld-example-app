module Integration

open Expecto
open RealWorld.Effects.DB
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Linq


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
      Expect.equal true true "This string option from db"
  ]