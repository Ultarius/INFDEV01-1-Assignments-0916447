// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open AsteroidsGame

[<EntryPoint>]
let main argv = 
    use g = new Asteroids()
    g.Run()
    0