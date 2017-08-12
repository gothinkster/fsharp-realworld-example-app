module RealWorld.JsonWebToken

open System.IO
open System.Text
open Newtonsoft.Json
open System.Security.Cryptography

type UserRights = 
  { UserName : string }

let private createPassPhrase() = 
    let crypto = System.Security.Cryptography.RandomNumberGenerator.Create()
    let randomNumber = Array.init 32 byte
    crypto.GetBytes(randomNumber)
    randomNumber

let private passPhrase =
    let encoding = Encoding.UTF8
    RealWorld.Effects.DB.getPassPhrase () |> System.Text.Encoding.ASCII.GetBytes

let private encodeString (payload:string) =
    Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

let private decodeString (jwt:string) =
    Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

let encode token =
    JsonConvert.SerializeObject token
    |> encodeString

let decode<'a> (jwt:string) : 'a =
    decodeString jwt
    |> JsonConvert.DeserializeObject<'a>

/// Returns true if the JSON Web Token is successfully decoded and the signature is verified.
let isValid (jwt:string) : UserRights option =
    try
        let token = decode jwt
        Some token
    with
    | _ -> None