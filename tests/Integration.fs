module Integration

open Expecto
open RealWorld.Effects.DB
open Suave
open Suave.Web
open RealWorld.Models
open System
open Helper.Utils
open MongoDB.Bson
open MongoDB.Driver.Linq
open MongoDB.Driver
open RealWorld.Models
  

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      // let articles = RealWorld.Effects.DB.getSavedFollowedArticles databaseClient
      // Expect.equal articles None String.Empty
      Expect.equal true true String.Empty

    testCase "Create empty mongo collection" <| fun _ -> 
      Expect.equal true true String.Empty
  ]