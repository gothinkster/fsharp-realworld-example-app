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

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      // This test can be used to testing individual functions if you don't want to use 
      // F# interactive      
      // let article = { defaultArticle with Id = "59a1095466506945d64c690e" }           

      Expect.equal true true String.Empty   
  ]