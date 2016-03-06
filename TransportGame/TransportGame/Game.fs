module Game

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input


type Behaviour<'d> = Behaviour of ('d -> 'd * Behaviour<'d>)

type Entity<'d> = 
    {
        Data        : 'd
        Behaviours  : Behaviour<'d> list 
    }

type FactoryData =
    {
        Position : Vector2
        Texture : Texture2D
        RefreshRate : int
        Storage : int
        MaxStorage : int
    }

type TruckData =
    {
        Position : Vector2
        Texture : Texture2D
        Capacity : int
        MaxCapacity : int
        Arrived : bool
        Loaded : bool
        Factory : Entity<FactoryData>
    }
    

type RoadData =
    {
        Position : Vector2
        Texture : Texture2D
    }


type GameState = 
    {
        Trucks : Entity<TruckData> list
        Roads  : Entity<RoadData> list
        Factories  : Entity<FactoryData> list
        Systems: int list 
    }

let rec repeat c x =
    match c with
    | Behaviour(cfunc) ->   let x', c' = cfunc x
                            //printfn "%A" x'
                            x'


let rec Draw c x (texture : Texture2D) (spriteBatch : SpriteBatch) =
    match c with
    | Behaviour(cfunc) ->   let x', c' = cfunc x
                            match (x' : TruckData) with
                            | TruckData -> spriteBatch.Draw(texture, new Rectangle((int)TruckData.Position.X, (int)TruckData.Position.Y, 30, 30), Color.White)
                            | FactoryData -> spriteBatch.Draw(texture, new Vector2((float32)FactoryData.Position.X, (float32)FactoryData.Position.Y), Color.White)
                            | _ -> ()

let rec MovementBehaviour = Behaviour(fun (x : TruckData) -> (if (Keyboard.GetState().IsKeyDown(Keys.D)) then {x with Position = new Vector2(x.Position.X + 2.0f, x.Position.Y)} 
                                                                    elif (Keyboard.GetState().IsKeyDown(Keys.A)) then {x with Position = new Vector2(x.Position.X - 2.0f, x.Position.Y)}
                                                                    else {x with Position = x.Position}), MovementBehaviour)

let rec AIBehaviour = Behaviour(fun (x : TruckData) -> (if x.Loaded then {x with Position = new Vector2(x.Position.X - 2.0f, x.Position.Y)} else (if x.Position.X < x.Factory.Data.Position.X && x.Arrived = false then {x with Position = new Vector2(x.Position.X + 2.0f, x.Position.Y)} else {x with Arrived = true})), AIBehaviour)
                                                                    
let rec PickupBehaviour = Behaviour(fun (x : TruckData) -> (if (x.Arrived) then (if(x.Capacity < x.MaxCapacity) then {x with Capacity = x.Capacity + 1} else {x with Loaded = true}) else x), PickupBehaviour)


type TransportGame() as this =
    inherit Game()
 
    do this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>
    let mutable state = {Factories = []; Roads = []; Trucks = []; Systems = []}

    override x.Initialize() =
        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        //let parkImage = spriteLoader "parking.png" this.GraphicsDevice

        //movableThingsImages <- [carImage; boatImage];

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])
        
        //let a = {ID = 0; Components = [Position(10.0f, 10.0f)];}

        //let bouncyBall = {ID = 1; Components = [Position(10.0f, 80.0f); Collision(0.0f, 0.0f, 300.0f, 300.0f)];}


        (*GameState <-{ GameState with 
                        Entities = [a; bouncyBall]
                        Systems = [OtherSystem; LogicSystem]
                    }
                    *)
        //State <- MainUpdate State

        let myEvent = Event<_>()
        let observerC = fun i -> Some(i)


        let factory = {Data = {Position = new Vector2(600.0f, 250.0f); Texture = texture; RefreshRate = 10; Storage = 0; MaxStorage = 10;}; Behaviours = []}
        //let reference = ref factory;
        //let normal = !reference;

        let truck = {Data = {Position = new Vector2(0.0f, 50.0f); Texture = texture; Capacity = 0; MaxCapacity = 300; Arrived = false; Loaded = false; Factory = factory;}; Behaviours = [MovementBehaviour; PickupBehaviour; AIBehaviour]}
        state <- {state with Trucks = [truck];}

        ()
 
    override this.Update (gameTime) =

        (* let rec worldLoop (systems : ISystem list) world = // Find out a better solution to this. =D
            match systems with
            | [] -> world
            | x::xs -> worldLoop xs (x.Update world); *)

(*
        let newWorld = GameState.Systems |> List.fold (fun s system -> system.Update s) GameState

        GameState <- newWorld

        //let newWorld = GameState.Systems |> List.map(fun s -> s.Update s GameState) |> Seq.head

        //GameState <- newWorld
        
        *)

        let newTrucks, truckMessages = state.Trucks |> List.map(fun entiteit -> {entiteit with Data = entiteit.Behaviours |> List.fold (fun s comp -> repeat comp s) entiteit.Data}), 0
        
        state <- {state with Trucks = state.Trucks 
                            |> List.map(fun entiteit -> {entiteit with Data = entiteit.Behaviours 
                                                                        |> List.fold (fun s comp -> repeat comp s) entiteit.Data})
                 }

        //Late Update checking messages?

        ()
        
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Black        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        
        state.Trucks |> List.iter(fun entiteit -> entiteit.Behaviours |> List.iter(fun x -> Draw x entiteit.Data texture spriteBatch ))
        //state.Factories |> List.iter(fun entiteit -> entiteit.Behaviours |> List.iter(fun x -> Draw x entiteit.Data texture spriteBatch ))
        spriteBatch.End()      