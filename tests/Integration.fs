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

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      Expect.equal true true String.Empty

    testCase "Getting articles" <| fun _ -> 
      Expect.equal true true String.Empty
  ]