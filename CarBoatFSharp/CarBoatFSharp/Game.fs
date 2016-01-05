module Game

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
     
let random = System.Random()

type TileType = Water | Road | Building | Park | Void

type Tile = {Position : Point; Type : TileType; Textures : Texture2D list} with
    member this.Texture (tile : Tile) =
        match tile.Type with
            | Park -> tile.Textures.[0]
            | Road -> tile.Textures.[1]
            | Water -> tile.Textures.[2]
            | Building -> tile.Textures.[3]
 
let MapLookup (map : Tile list) i j = 
    let X = (i - 1)
    let Y = (j - 1) * 10
    if (X >= 0 && X < 10) && (Y >= 0 && Y < 100) then
        map.Item(X + Y).Type
    else 
        Void

type Car =
    {
        Position : Point
        Timer : float32
        Texture : Texture2D
    }
    static member Draw (car : Car) (spriteBatch : SpriteBatch) =
         spriteBatch.Draw(car.Texture, new Rectangle(25 + car.Position.X * 30, 25 + (car.Position.Y * 30), 15, 15), Color.White)
    static member UpdateNormal (car : Car) (map : Tile list) =
        if car.Timer < 0.0f then
            let rec options ways output = 
                match ways with
                | x::xs when (MapLookup map (car.Position.X + fst x) (car.Position.Y + snd x) = Road) ||
                             (MapLookup map (car.Position.X + fst x) (car.Position.Y + snd x) = Park) -> options xs output @ [new Point(car.Position.X + fst x, car.Position.Y + snd x)]
                | x::xs -> options xs output
                | [] -> output
                | _ -> []

            let waytogo = options [(-1,0);(0, 1);(1, 0);(0,-1)] []
            //printfn "%A" waytogo
            let thechosenone = if waytogo.Length > 0 then waytogo |> List.sortBy (fun _ -> random.Next()) |> Seq.head else (new Point(car.Position.X, car.Position.Y))
            //printfn "%A" thechosenone
            {car with Position = thechosenone; Timer = 5.0f}
        else
            {car with Timer = car.Timer - 0.25f} 

type Boat =
    {
        Position : Point
        Timer : float32
        Texture : Texture2D
    }
    static member Draw (boat : Boat) (spriteBatch : SpriteBatch) =
        spriteBatch.Draw(boat.Texture, new Rectangle(25 + boat.Position.X * 30, 25 + (boat.Position.Y * 30), 15, 15), Color.White)
    static member UpdateNormal (boat : Boat) (map : Tile list) =
        if boat.Timer < 0.0f then
            let rec options ways output = 
                match ways with
                | x::xs when (MapLookup map (boat.Position.X + fst x) (boat.Position.Y + snd x) = Water) ||
                             (MapLookup map (boat.Position.X + fst x) (boat.Position.Y + snd x) = Park) -> options xs output @ [new Point(boat.Position.X + fst x, boat.Position.Y + snd x)]
                | x::xs -> options xs output
                | [] -> output
                | _ -> []

            let waytogo = options [(-1,0);(0, 1);(1, 0);(0,-1)] []
            //printfn "%A" waytogo
            let thechosenone = if waytogo.Length > 0 then waytogo |> List.sortBy (fun _ -> random.Next()) |> Seq.head else (new Point(boat.Position.X, boat.Position.Y))
            //printfn "%A" thechosenone
            {boat with Position = thechosenone; Timer = 10.0f}
        else
            {boat with Timer = boat.Timer - 0.25f} 


type Entity =
    {
        Update      : Tile list -> Entity
        IsArrived   : Tile list -> bool
        Draw        : SpriteBatch -> unit
    }
    with 
        static member CreateFromCar(car:Car) =
            let update map =
                Car.UpdateNormal car map |> Entity.CreateFromCar
            let draw spriteBatch =
                Car.Draw car spriteBatch
            let isArrived map =
                (MapLookup map car.Position.X car.Position.Y) <> Park
            {
                Update = update
                IsArrived = isArrived
                Draw = draw
            }
        static member CreateFromBoat(boat:Boat) =
            let update map =
                Boat.UpdateNormal boat map |> Entity.CreateFromBoat
            let draw spriteBatch =
                Boat.Draw boat spriteBatch
            let isArrived map =
                (MapLookup map boat.Position.X boat.Position.Y) <> Park
            {
                Update = update
                IsArrived = isArrived
                Draw = draw
            }


type gameState =
    {
        Map : Tile list;
        Objects : Entity list;
    }

let mutable State = {   Map = [];
                        Objects = [] }


let randomTile (random : int) : TileType = 
    match random with
    | random when random < 1 -> Park
    | random when random < 3 -> Building
    | random when random < 6 -> Water
    | random -> Road


let createMap list size : Tile list =
    let riverPosX = 1
    let riverPosY = 1
    [
        for i = 1 to size do
            for j = 1 to size do
                yield
                    {
                        Position = new Point(j, i)
                        Type = if (i = riverPosX || j = riverPosY) then Water
                                 elif ((i - 1) = riverPosX ||
                                       (j - 1) = riverPosY ||
                                       (i + 1) = riverPosX ||
                                       (j + 1) = riverPosY) then
                                    Road
                                 else 
                                    randomTile(random.Next(0, 20))
                        Textures = list
                    }
    ]

let addCars amount (map : Tile list) (img : Texture2D) : Entity list =
    [
        for i = 1 to amount do
          yield
            {
              Car.Position = map |> List.sortBy (fun _ -> random.Next()) |> List.find (fun x -> x.Type = Road) |> (fun i -> printfn "%A" i.Position; i.Position)
              Timer = 5.0f
              Texture = img
            }
    ] |> List.map Entity.CreateFromCar

let addBoats amount (map : Tile list) (img : Texture2D) : Entity list =
    [
        for i = 1 to amount do
          yield
            {
              Boat.Position = map |> List.sortBy (fun _ -> random.Next()) |> List.find (fun x -> x.Type = Water) |> (fun i -> printfn "%A" i.Position; i.Position)
              Timer = 10.0f
              Texture = img
            }
    ] |> List.map Entity.CreateFromBoat

let spriteLoader (path) graphics = 
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture

type CarBoat() as this =
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

        let parkImage = spriteLoader "parking.png" this.GraphicsDevice
        let roadImage = spriteLoader "road.png" this.GraphicsDevice
        let waterImage = spriteLoader "water.png" this.GraphicsDevice
        let buildingImage = spriteLoader "building.png" this.GraphicsDevice
        let carImage = spriteLoader "car.png" this.GraphicsDevice
        let boatImage = spriteLoader "boat.png" this.GraphicsDevice

        movableThingsImages <- [carImage; boatImage];

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        let map = createMap [parkImage; roadImage; waterImage; buildingImage] 10

        State <- {  Objects = (addBoats 1 map boatImage) @ (addCars 1 map carImage)
                    Map = map }

        //State <- MainUpdate State

        ()
 
    override this.Update (gameTime) =
        State <- {  State with
                        Objects = State.Objects |> List.map (fun obj -> obj.Update State.Map)
                                                |> List.filter (fun obj -> obj.IsArrived State.Map)
                 }

                      //   Cars = State.Cars |> List.filter (fun car -> (MapLookup State.Map car.Position.X car.Position.Y) <> Park) 
                      //                    |> List.map (fun car -> car.UpdateNormal car State.Map)
                      //                    |> if (random.Next(0, 100) < 1) then (@) (addCars 1 State.Map movableThingsImages.[0]) else id
                      //  Boats = State.Boats |> List.filter (fun boat -> (MapLookup State.Map boat.Position.X boat.Position.Y) <> Park) 
                      //                      |> List.map (fun boat -> boat.UpdateNormal boat State.Map) 
                      //                      |> if (random.Next(0, 100) < 1) then (@) (addBoats 1 State.Map movableThingsImages.[1]) else id


    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Green

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)
        
        State.Map |> List.iteri(fun x tile -> spriteBatch.Draw(tile.Texture tile, new Rectangle(50 + (x % 10 * 30), 50 + (x / 10 * 30), 25, 25), Color.White))

        State.Objects |> List.iteri(fun _ obj -> obj.Draw spriteBatch)

        //State.Cars |> List.iteri(fun _ car -> spriteBatch.Draw(car.Texture, new Rectangle(25 + car.Position.X * 30, 25 + (car.Position.Y * 30), 15, 15), Color.White))
        
        //State.Boats |> List.iteri(fun _ boat -> spriteBatch.Draw(boat.Texture, new Rectangle(25 + boat.Position.X * 30, 25 + (boat.Position.Y * 30), 15, 15), Color.Green))

        spriteBatch.End()
        ()