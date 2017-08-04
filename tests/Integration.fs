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
      // let encryptedPassword = RealWorld.Hash.Crypto.hash "thistest" 3
      // printfn "Pass: %A" encryptedPassword
      
      Expect.equal true true String.Empty
  ]