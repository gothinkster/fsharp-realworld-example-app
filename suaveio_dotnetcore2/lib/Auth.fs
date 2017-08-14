/// Login web part and functions for API web part request authorisation with JWT.
module RealWorld.Auth

open Suave
open Suave.RequestErrors
open MongoDB.Driver
open RealWorld.Models
open RealWorld.Effects
open BsonDocConverter

type Login = {
  UserName: string;
  Password: string;
}

let unauthorized s = Suave.Response.response HTTP_401 s

let UNAUTHORIZED s = unauthorized (UTF8.bytes s)

/// Login web part that authenticates a user and returns a token in the HTTP body.
let login (ctx: HttpContext) = async {
    let login = 
        ctx.request.rawForm 
        |> System.Text.Encoding.UTF8.GetString
        |> RealWorld.Json.ofJson<Login>

    try
        if (login.UserName <> "test" || login.Password <> "test") && 
           (login.UserName <> "test2" || login.Password <> "test2") then
            return! failwithf "Could not authenticate %s" login.UserName
        let user : JsonWebToken.UserRights = { UserName = "fill_in_later" }
        let token = JsonWebToken.encode user

        return! Successful.OK token ctx
    with
    | _ -> return! UNAUTHORIZED (sprintf "User '%s' can't be logged in." login.UserName) ctx
}

let validatePassword (savedPassword: UserDetails option) passedInPassword = 
  match savedPassword with
  | Some password -> RealWorld.Hash.Crypto.verify password.PasswordHash passedInPassword
  | None _ -> false

let loginWithCredentials dbClient (ctx: HttpContext) = async {
  let login = 
        ctx.request.rawForm 
        |> System.Text.Encoding.UTF8.GetString
        |> RealWorld.Json.ofJson<Login>

  try
      let checkedPassword = RealWorld.Effects.DB.loginUser dbClient login.UserName
      match checkedPassword with
      | Some pass -> 
        if not (validatePassword (Some(toUserDetail pass)) login.Password) then
            return! failwithf "Could not authenticate %s" login.UserName
        
        let user : JsonWebToken.UserRights = { UserName = login.UserName }
        let token = JsonWebToken.encode checkedPassword.Value
        
        return! Successful.OK token ctx
      | None -> 
        return! failwithf "Could not authenticate %s" login.UserName
  with
  | _ -> return! UNAUTHORIZED (sprintf "User '%s' can't be logged in." login.UserName) ctx
}

/// Invokes a function that produces the output for a web part if the HttpContext
/// contains a valid auth token. Use to authorise the expressions in your web part
/// code (e.g. WishList.getWishList).
let useToken ctx f = async {
    match ctx.request.header "Authorization" with
    | Choice1Of2 accesstoken when accesstoken.StartsWith "Bearer " -> 
        let jwt = accesstoken.Replace("Bearer ","")
        match JsonWebToken.isValid jwt with
        | None -> return! FORBIDDEN "Accessing this API is not allowed" ctx
        | Some token -> return! f token
    | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx
}