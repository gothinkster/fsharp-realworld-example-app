module Integration

open Expecto
open RealWorld.Effects.DB
open Suave
open Suave.Web
open RealWorld.Models
open System
open Helper.Utils
open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Driver.Linq
open MongoDB.Driver
open RealWorld.Models
open RealWorld.Convert
open Newtonsoft.Json
open MongoDB.Bson.Serialization.Attributes

let dbClient = 
  let client = MongoClient("mongodb://localhost:27017")
  client.GetDatabase("realworld")
 
type PsuedoComment = {    
  [<BsonId>]
  [<BsonRepresentation(BsonType.ObjectId)>]
  Id      : string;    
  articleId: string;
  createdAt: DateTime;
  updatedAt: DateTime;
  body: string;
}

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      // This test can be used to testing individual functions if you don't want to use 
      // F# interactive      
      // let article = { defaultArticle with Id = "59a1095466506945d64c690e" }       

          

      let collection = dbClient.GetCollection<Comment> "Comment"      
      let result = collection.AsQueryable().Where(fun comment -> comment.comment.articleId = "59a1095466506945d64c690e").ToList() |> List.ofSeq
      printfn "Result: %A" result

      Expect.equal true true String.Empty   
  ]