﻿module Game

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let random = System.Random()

type Component = 
    | Position of x : float32 * y : float32
    | Bounds of width : float32 * height : float32


type GenericEntity =
    {
        ID          : int
        Components  : Component list
    }
    static member Add (entity : GenericEntity) (newComponent : Component) =
        {entity with Components = entity.Components @ [newComponent]}
    static member Remove (entity : GenericEntity) (newComponent : Component) =
        {entity with Components = entity.Components |> List.filter(fun c -> c = newComponent)}
    static member Kill (entity : GenericEntity) =
        {entity with Components = []}


type ISystem() =
   abstract member Update   : World -> World
   abstract member Draw     : World -> Texture2D -> SpriteBatch -> unit
   default u.Update (world : World) = world;
   default u.Draw (world : World) (texture : Texture2D) (spriteBatch : SpriteBatch) = ();
and World =
    {
        Entities    : GenericEntity list
        Components  : Component list
        Systems     : ISystem list
    }
    static member Update (world : World) =
        world.Systems |> List.map(fun s -> s.Update world)
    static member Draw (world : World) (texture : Texture2D) (spriteBatch : SpriteBatch) =
        world.Systems |> List.iter(fun s -> s.Draw world texture spriteBatch);


type LogicSystem() =
   inherit ISystem()

   let updatePosition (entity : GenericEntity) = 

    let positionX, positionY, p = entity.Components |> List.pick (fun c ->
                        match c with
                        | Position(x = xPos; y = yPos) -> Some(xPos, yPos, true)
                        | _ -> None)

    if Keyboard.GetState().IsKeyDown(Keys.A) && p then
        {entity with Components = [Position(positionX - 4.0f, positionY)]}
    elif Keyboard.GetState().IsKeyDown(Keys.D) && p then
        {entity with Components = [Position(positionX + 2.0f, positionY)]}
    else
        entity

   override u.Update (world : World) =
        { world with Entities = world.Entities |> List.map(fun e ->
                    let rec updateEntity components (entity : GenericEntity) = 
                        match components with
                            | [] -> entity
                            | Position(x = xPos; y = yPos)::xs -> updateEntity xs {entity with Components = [Position(xPos + 2.0f, yPos)] @ xs}; 
                            | _::xs -> updateEntity xs entity
                    let updatedEntity = updatePosition e
                    updateEntity updatedEntity.Components updatedEntity
            )}
   override u.Draw (world : World) (texture : Texture2D) (spriteBatch : SpriteBatch) =
        world.Entities |> List.iter(fun e -> e.Components |> List.iter(fun c -> 
                    match c with
                        | Position(x = xPos; y = yPos) -> spriteBatch.Draw(texture, new Rectangle(int(xPos), int(yPos), 25, 25), Color.White) // Draw Block
                        | _ -> ()
            ))


type OtherSystem() =
   inherit ISystem()

   override u.Update (world : World) =
        { world with Entities = world.Entities |> List.map(fun e ->
                    let rec updateEntity components (entity : GenericEntity) = 
                        match components with
                            | [] -> entity
                            | Position(x = xPos; y = yPos)::xs -> updateEntity xs {entity with Components = [Position(xPos, yPos + 1.0f)] @ xs}; 
                            | _::xs -> updateEntity xs entity
                    updateEntity e.Components e
            )}
   override u.Draw (world : World) (texture : Texture2D) (spriteBatch : SpriteBatch) =
        world.Entities |> List.iter(fun e -> e.Components |> List.iter(fun c -> ()
            ))

(*
let rec updateEntity components entity
    match components with
        | [] -> entity
        | x::xs when x.Name = "Ball" -> let newEntity = entity; updateEntity components newEntity
        | x::xs -> updateEntity xs entity
*)

type Entity =
    {
        Add          : Component -> Entity
        Remove       : Component -> Entity
    }
    with
        static member CreateEntity (genericEntity : GenericEntity) =
            let add newComponent =
                GenericEntity.Add genericEntity newComponent |> Entity.CreateEntity
            let remove oldComponent =
                GenericEntity.Remove genericEntity oldComponent |> Entity.CreateEntity
            {
                Add = add
                Remove = remove
            }

let mutable GameState : World = { Entities = []; Components = []; Systems = [] }


let spriteLoader (path) graphics = 
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture

type WallmartSimulator() as this =
    inherit Game()
 
    do this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>

    let mutable movableThingsImages = List.Empty;

    override x.Initialize() =
        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        //let parkImage = spriteLoader "parking.png" this.GraphicsDevice

        //movableThingsImages <- [carImage; boatImage];

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        let a = {ID = 0; Components = [Position(10.0f, 10.0f)];}


        GameState <-{ GameState with 
                        Entities = [a]
                        Systems = [new OtherSystem(); new LogicSystem();]
                    }

        //State <- MainUpdate State

        ()
 
    override this.Update (gameTime) =

        (* let rec worldLoop (systems : ISystem list) world = // Find out a better solution to this. =D
            match systems with
            | [] -> world
            | x::xs -> worldLoop xs (x.Update world); *)


        let newWorld = GameState.Systems |> List.fold (fun s system -> system.Update s) GameState

        GameState <- newWorld

        //let newWorld = GameState.Systems |> List.map(fun s -> s.Update s GameState) |> Seq.head

        //GameState <- newWorld
        

    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Black

        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)
        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        //State.Map |> List.iteri(fun x tile -> spriteBatch.Draw(tile.Texture tile, new Rectangle(50 + (x % 10 * 30), 50 + (x / 10 * 30), 25, 25), Color.White))

        GameState.Systems |> List.iter(fun s -> s.Draw GameState texture spriteBatch )

        spriteBatch.End()
        