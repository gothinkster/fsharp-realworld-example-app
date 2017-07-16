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

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      let articles = getSavedArticles databaseClient
      printfn "No articles: %A" articles
      Expect.equal true true String.Empty
  ]