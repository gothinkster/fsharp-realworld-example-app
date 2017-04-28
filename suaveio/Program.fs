open System
open Suave
open Suave.Http

let serverConfig = 
  { defaultConfig with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8070] }

[<EntryPoint>]
let main argv =
  startWebServer serverConfig (Successful.OK "This is just the beginning!")
  0
