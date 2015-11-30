module AsteroidsGame

open System.Collections.Generic 
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let velocityX = 0.03f
let velocityY = 0.03f



let mutable mouseState = Mouse.GetState()

let mutable spriteTexture = Unchecked.defaultof<Texture2D>


type GameState = 
    | Game
    | Menu
    | Paused

type WeaponType = 
    | DualFire
    | MachineGun

type Sprite(startX : float32, startY : float32, width : float32, height : float32, scale : float32) =

    let Width = width * scale
    let Height = height * scale

    member val X = startX with get, set
    
    member val Y = startY with get, set

    member this.Origin
        with get() = new Vector2((Height / scale) / 2.0f, (Width / scale) / 2.0f)

    member this.Visible
        with get() = int (this.X + Width) > 0

    member this.Bounds
        with get() = Rectangle(int this.X, int this.Y, int Width, int Height)

    //member this.Draw(spriteBatch : SpriteBatch, texture) =
        //spriteBatch.Draw(texture, this.Bounds, Color.Green)

    abstract member Render: SpriteBatch -> Texture2D -> unit
        default this.Render (spriteBatch : SpriteBatch) (texture : Texture2D) = spriteBatch.Draw(texture, this.Bounds, Color.Green)

[<AbstractClass>]
type Obstacle(startX, startY, width, height) =
    inherit Sprite(startX, startY, width, height, 1.0f)

    abstract member onCollision: Obstacle list -> Obstacle list
        default this.onCollision (obstacles : Obstacle list) = obstacles |> List.filter (fun other -> not (other.Bounds.Intersects this.Bounds))

   // member this.onCollision (obstacles : Obstacle list) =
        //let newList = obstacles |> List.filter (fun other -> not (other.Bounds.Intersects this.Bounds))
        //newList

    abstract member Update : single -> unit 

type Rock(startX, startY) =
    inherit Obstacle(startX, startY, 50.0f, 50.0f)

    let mutable lives = 1

    override this.onCollision(obstacles : Obstacle list) = 
        //printfn "%A" [ lives ]; 
        let newList:Obstacle list =
           let isHit = fun (other: Obstacle) -> not (other.Bounds.Intersects this.Bounds) && not (lives <= 0)
           List.filter isHit obstacles
        newList

        //let newList = obstacles |> List.filter (fun other -> not (other.Bounds.Intersects this.Bounds) && lives > 0)
        //newList

    override this.Update(deltaTime : single) = 
        this.X <- this.X

type Bullet(startX, startY, rot, dx : single, dy : single) =
    inherit Obstacle(startX, startY, 3.0f, 10.0f)

    override this.Update(deltaTime : single) = 
        this.X <- this.X + deltaTime * dy
        this.Y <- this.Y + deltaTime * dx

    override this.Render (spriteBatch) (texture) = 
        spriteBatch.Draw(texture,
                         this.Bounds,
                         System.Nullable(),
                         Color.DarkRed,
                         (-rot),
                         new Vector2(0.5f, 0.5f),
                         SpriteEffects.None,
                         1.0f)


let mutable bullets: Obstacle list = []


let Rotation a b = atan2 a b

type Ship(spriteTexture : Texture2D, startX) =
    inherit Sprite(startX, 0.0f, float32 spriteTexture.Bounds.Width, float32 spriteTexture.Bounds.Height, 0.125f)
    let mutable speed = 0.25f

    member val Weapon = WeaponType.MachineGun with get, set

    member this.Update (deltaTime) =
        mouseState <- Mouse.GetState()
        let mouseX = float32 mouseState.X - this.X
        let mouseY = float32 mouseState.Y - this.Y
        let rotation = Rotation mouseX mouseY
        let dx = cos rotation
        let dz = sin rotation
        this.X <- this.X + speed * dz * deltaTime
        this.Y <- this.Y + speed * dx * deltaTime

    member this.Fire =
        mouseState <- Mouse.GetState()
        let mouseX = float32 mouseState.X - this.X
        let mouseY = float32 mouseState.Y - this.Y
        let rotation = Rotation mouseX mouseY
        let dx = cos rotation
        let dz = sin rotation
        if this.Weapon = WeaponType.MachineGun then
            let newList = List.append bullets [ Bullet( this.X, this.Y, rotation, dx, dz ) ]
            bullets <- newList

    override this.Render (spriteBatch) (_) = 
        mouseState <- Mouse.GetState()
        let mouseX = float32 mouseState.X - this.X
        let mouseY = float32 mouseState.Y - this.Y
        let rotation = Rotation mouseX mouseY
        spriteBatch.Draw(spriteTexture,
                         this.Bounds,
                         System.Nullable(),
                         Color.White,
                         -rotation,
                         this.Origin,
                         SpriteEffects.None,
                         1.0f)


type Asteroids() as this =
    inherit Game()
 
    do this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>


    let mutable ship = Unchecked.defaultof<Ship>

    let mutable obstacles : Obstacle list = []
    let mutable astroids : Obstacle list = []
 
    override x.Initialize() =
        do base.Initialize()
        ()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        use imagePath = System.IO.File.OpenRead("spaceShip.png")
        let shipTexture = Texture2D.FromStream(this.GraphicsDevice, imagePath)
        let shipData = Array.create<Color> (shipTexture.Width * shipTexture.Height) Color.Transparent
        shipTexture.GetData(shipData)


        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        ship <- Ship(shipTexture, 30.0f) 

        //obstacles <- [ Rock(200.0f, 100.0f) ]


        let newList = List.append astroids [ Rock(500.0f, 300.0f) ]
        astroids <- newList

        let newList1 = List.append astroids [ Rock(200.0f, 100.0f) ]
        astroids <- newList1

        ()
 
    override this.Update (gameTime) =
        let newState = Mouse.GetState()
        let deltaTime = single(gameTime.ElapsedGameTime.TotalMilliseconds)

        if newState.LeftButton = ButtonState.Pressed then
            ship.Update deltaTime 
        if newState.RightButton = ButtonState.Pressed then
            ship.Fire

        


        let weaponSelection position =
            match position with
            | 0 -> WeaponType.MachineGun
            | 40 -> WeaponType.DualFire
            | _ -> WeaponType.MachineGun

        let num = weaponSelection (abs (newState.ScrollWheelValue % 80))

        ship.Weapon = num

        printfn "%A" [ ship.Weapon ]; 
        printfn "%A" [ num ]; 

        for bullet in bullets do 
            bullet.Update(deltaTime)
        ()
 
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Black

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)

        ship.Render spriteBatch texture

        for obstacle in obstacles do
            obstacle.Render spriteBatch texture

            //bullets <- obstacle.onCollision bullets
         
        for astroid in astroids do
            astroid.Render spriteBatch texture

            bullets <- astroid.onCollision bullets

        for bullet in bullets do
            //printfn "%A" [  ]; 
            bullet.Render spriteBatch texture


            //astroids <- bullet.onCollision astroids
            

        spriteBatch.End()
        ()