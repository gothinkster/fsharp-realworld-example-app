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

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      //getArticleBySlug databaseClient "just-inserted" |> printfn "Article: %A"
      Expect.equal true true String.Empty

    testCase "Getting articles" <| fun _ -> 
      RealWorld.Hash.Crypto.fastHash "test" |> printfn "Password hash: %A"
      Expect.equal true true String.Empty
  ]