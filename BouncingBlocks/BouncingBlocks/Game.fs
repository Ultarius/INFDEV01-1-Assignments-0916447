module Game


open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open BounceBlocksMath

let random = System.Random()

let deltaTime = 0.16<Seconds>

let gravity = 
    {  
        X = 0.0< Meters/Seconds ^2 >
        Y = -9.81< Meters/Seconds ^2 >
    }

let gravityFactor = 1.0

type Block =
    {
        mutable Position : Vector2<Meters>
        mutable Velocity : Vector2<Meters/Seconds>
    }
    static member Update (position : Vector2<Meters>, velocity : Vector2<Meters/Seconds>) = 
        {
              Position = position
              Velocity = velocity 
        }
    member this.Draw (spriteBatch : SpriteBatch) (texture : Texture2D) = 
        spriteBatch.Draw(texture,
                    new Rectangle(int this.Position.X, int -this.Position.Y, 75, 75),
                    System.Nullable(),
                    Color.White,
                    0.0f,
                    new Vector2(0.0f,0.0f),
                    SpriteEffects.None,
                    1.0f)

let collisionMap (position : Vector2<Meters> , velocity: Vector2<Meters/Seconds>) =
    let bounceForce = 0.65
    //printfn "%A" position.Y
    match (position, velocity) with
    |(position, velocity) when position.X < 0.0<Meters> -> ({X = 0.0<Meters> ; Y = position.Y}, {X = -velocity.X * bounceForce ; Y = velocity.Y * bounceForce})
    |(position, velocity) when position.X > 425.0<Meters> -> ({X = 425.0<Meters> ; Y = position.Y}, {X = -velocity.X * bounceForce ; Y = velocity.Y * bounceForce})
    |(position, velocity) when position.Y < -425.0<Meters> -> ({X = position.X ; Y = -425.0<Meters>}, {X = velocity.X * bounceForce; Y = -velocity.Y * bounceForce})
    |(position, velocity) when position.Y > 0.0<Meters> -> ({X = position.X ; Y = 0.0<Meters>}, {X = velocity.X * bounceForce ; Y = -velocity.Y * bounceForce})
    | _ -> (position, velocity)

let simulation_step (blocks: Block list) =
    let blocks' = blocks |> List.map (fun (x) -> (x.Position + x.Velocity * deltaTime, x.Velocity + (gravity * gravityFactor) * deltaTime))
                         |> List.map collisionMap
    blocks'

type gameState =
    {
        Blocks : Block list;
    }

let MainUpdate (previousState : gameState) : gameState =
    {
    previousState with
        Blocks =    previousState.Blocks |>
                    simulation_step |>
                    List.map (fun x -> Block.Update x) |>
                    List.filter (fun x -> not (
                                            abs(x.Position.Y) > 420.0<Meters> &&
                                            (abs x.Velocity.Y) < 0.1<Meters/Seconds>))
    }
let mutable State = { Blocks = []; }


let addBlock amount : Block list =
    [
        for i = 1 to amount do
          yield
            {
              Position = {
                            X = 250.0<Meters>
                            Y = -25.0<Meters>
                         }
              Velocity = {
                            X = -60.0<Meters/Seconds>
                            Y = 5.0<Meters/Seconds>
                         }
            }
    ]

type BouncingBlocks() as this =
    inherit Game()
 
    do this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>

    do graphics.PreferredBackBufferWidth  <- 500
    do graphics.PreferredBackBufferHeight <- 500

    override x.Initialize() =
        do base.Initialize()
        ()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        do texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        State <- {
            Blocks = addBlock 1
        }


        State <- MainUpdate State

        ()
 
    override this.Update (gameTime) =
        
        if State.Blocks.Length < 1 then   
            State <- { Blocks = addBlock 1 }

        State <- MainUpdate State

        ()
 
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Black

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)

        for block in State.Blocks do
            block.Draw spriteBatch texture

        spriteBatch.End()
        ()