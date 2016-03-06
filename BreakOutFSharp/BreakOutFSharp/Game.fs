module Game

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let random = System.Random()

type PaddleState = Normal | Sticky


type Block =
    {
        X           : float32
        Y           : float32
        Width       : float32
        Height      : float32
        Texture     : Texture2D
        Color       : Color
    }
    member x.Bounds
        with get() = Rectangle(int x.X, int x.Y, int x.Width, int x.Height)
    static member Update (block : Block) =
        {block with X = block.X
                    Y = block.Y}
    static member Draw (block : Block) (spriteBatch : SpriteBatch) =
        spriteBatch.Draw(block.Texture, block.Bounds, block.Color)
    static member OnCollision (block : Block) (collisionMap : Rectangle list) =
        let rec collisionCheck (bounds : Rectangle list) = 
            match bounds with
            | [] -> false
            | x::xs when x <> block.Bounds && block.Bounds.Intersects x -> true
            | x::xs -> collisionCheck xs
        collisionCheck collisionMap

type Particle =
    {
        X               : float32
        Y               : float32
        DirX            : float32
        DirY            : float32
        Width           : float32
        Height          : float32
        Texture         : Texture2D
        Color           : Color
        LifeSpan        : float32
        CurrentLifeSpan : float32
    }
    member x.Bounds
        with get() = Rectangle(int x.X, int x.Y, int x.Width, int x.Height)
    static member Update (particle : Particle) =
        {particle with CurrentLifeSpan = particle.CurrentLifeSpan - 0.025f;
                       X = particle.X + particle.DirX
                       Y = particle.Y + particle.DirY}
    static member Draw (particle : Particle) (spriteBatch : SpriteBatch) =
        spriteBatch.Draw(particle.Texture, particle.Bounds, particle.Color * (particle.CurrentLifeSpan / particle.LifeSpan * 1.0f))



type Ball =
    {
        X           : float32
        Y           : float32
        Width       : float32
        Height      : float32
        Texture     : Texture2D
        Speed       : float32
        VelocityX   : float32
        VelocityY   : float32
    }
    member x.Bounds
        with get() = Rectangle(int x.X, int x.Y, int x.Width, int x.Height)
    static member Update (ball : Ball) (collisionMap : Rectangle list) =


        let preciseCollisionCheck (ball : Ball) (bounds : Rectangle) =
            let ballModifier = 1.025f;
            match bounds with
            // Left Right
            | x when ((int(ball.X + ball.Width) < bounds.Left || int(ball.X + ball.Width) > bounds.Right) &&
                        (int(ball.Y) < bounds.Bottom && int(ball.Y + ball.Height) > bounds.Top) ) -> {ball with VelocityX = -ball.VelocityX * ballModifier; X = ball.X - ball.VelocityX }
            // Top Bottom
            | x when (int(ball.Y + ball.Height) > bounds.Bottom || int(ball.Y) < bounds.Top &&
                        int(ball.X + ball.Width) > bounds.Left && int(ball.X) < bounds.Right ) -> {ball with VelocityY = -ball.VelocityY * ballModifier; Y = ball.Y - ball.VelocityY }
            | x -> {ball with X = ball.X + ball.VelocityX; Y = ball.Y + ball.VelocityY }

        let preciseCollisionCheck2 (ball : Ball) (bounds : Rectangle) =
            let ballModifier = 1.025f;
            let x = (bounds.X + (bounds.Width / 2)) - (int(ball.X) + (int(ball.Width) / 2))
            let y = (bounds.Y + (bounds.Height / 2)) - (int(ball.Y) + (int(ball.Height) / 2))
            if (abs(x) > abs(y)) then
                let newVelocityX = if abs(-ball.VelocityX * ballModifier) < 8.0f then -ball.VelocityX * ballModifier else -ball.VelocityX
                {ball with VelocityX = newVelocityX; X = ball.X + newVelocityX }
            else
                let newVelocityY = if abs(-ball.VelocityY * ballModifier) < 8.0f then -ball.VelocityY * ballModifier else -ball.VelocityY
                {ball with VelocityY = newVelocityY; Y = ball.Y + newVelocityY }


        let rec collisionCheck (bounds : Rectangle list) = 
            let ballModifier = 1.025f;
            match bounds with
            | [] when ball.X > (800.0f - ball.Width) || ball.X < 0.0f -> {ball with VelocityX = -ball.VelocityX * ballModifier; X = ball.X - ball.VelocityX }
            | [] when ball.Y > (475.0f - ball.Height) || ball.Y < 0.0f -> {ball with VelocityY = -ball.VelocityY * ballModifier; Y = ball.Y - ball.VelocityY }
            | [] -> {ball with X = ball.X + ball.VelocityX; Y = ball.Y + ball.VelocityY }
            //| x::xs when ball.Bounds.Intersects x -> preciseCollisionCheck2 ball x
            | x::xs when x <> ball.Bounds && ball.Bounds.Intersects x -> {ball with VelocityX = -(1.0f - 2.0f * (ball.X - float32(x.X)) / float32(x.Width / 2)); VelocityY = -ball.VelocityY; Y = ball.Y - ball.VelocityY; X = ball.X - ball.VelocityX }
            | x::xs -> collisionCheck xs


        collisionCheck collisionMap

    static member Draw (ball : Ball) (spriteBatch : SpriteBatch) =
        spriteBatch.Draw(ball.Texture, ball.Bounds, Color.White)


type Paddle =
    {
        X           : float32
        Y           : float32
        Width       : float32
        Height      : float32
        Texture     : Texture2D
        State       : PaddleState
    }
    member x.Bounds
        with get() = Rectangle(int x.X, int x.Y, int x.Width, int x.Height)
    static member Draw (paddle : Paddle) (spriteBatch : SpriteBatch) =
        spriteBatch.Draw(paddle.Texture, paddle.Bounds, Color.White)
    static member Update (paddle : Paddle) =
        if (Keyboard.GetState().IsKeyDown(Keys.A) && (paddle.X > 0.0f)) then
            {paddle with X = paddle.X - 10.0f
                         Y = paddle.Y}
        elif (Keyboard.GetState().IsKeyDown(Keys.D) && (paddle.X < 725.0f)) then
            {paddle with X = paddle.X + 10.0f
                         Y = paddle.Y}
        else
            {paddle with X = paddle.X
                         Y = paddle.Y}

type Entity =
    {
        //Update      : Tile list -> Entity
        Update      : Entity list -> Rectangle list -> Entity
        Draw        : SpriteBatch -> unit
        OnCollision : Rectangle list -> bool
        GetBounds   : Rectangle
    }
    with
        static member CreateBlock (block:Block) =
            let update map (collisionMap : Rectangle list) =
                Block.Update block |> Entity.CreateBlock
            let draw spriteBatch =
                Block.Draw block spriteBatch
            let onCollision collisionMap =
                Block.OnCollision block collisionMap
            let getBounds =
                block.Bounds
            {
                Update = update 
                Draw = draw
                OnCollision = onCollision
                GetBounds = getBounds
            }
        static member CreatePaddle (paddle:Paddle) =
            let update map (collisionMap : Rectangle list) =
                Paddle.Update paddle |> Entity.CreatePaddle
            let draw spriteBatch =
                Paddle.Draw paddle spriteBatch
            let onCollision map =
                false
            let getBounds =
                paddle.Bounds
            {
                Update = update 
                Draw = draw
                OnCollision = onCollision
                GetBounds = getBounds
            }
        static member CreateBall (ball:Ball) =
            let update map (collisionMap : Rectangle list) =
                Ball.Update ball collisionMap |> Entity.CreateBall
            let draw spriteBatch =
                Ball.Draw ball spriteBatch
            let onCollision map =
                false
            let getBounds =
                ball.Bounds
            {
                Update = update 
                Draw = draw
                OnCollision = onCollision
                GetBounds = getBounds
            }
        static member CreateParticle (particle:Particle) =
            let update map (collisionMap : Rectangle list) =
                Particle.Update particle |> Entity.CreateParticle
            let draw spriteBatch =
                Particle.Draw particle spriteBatch
            let onCollision map =
                false
            {
                Update = update 
                Draw = draw
                OnCollision = onCollision
                GetBounds = new Rectangle(0,0,0,0)
            }

type gameState =
    {
        Map : Particle list;
        Objects : Entity list;
    }

let mutable State = {   Map = [];
                        Objects = [] }


let spriteLoader (path) graphics = 
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture

let blockColors = [Color.Red; Color.Blue; Color.White; Color.Yellow; Color.Purple; Color.Orange]


let addParticle (pos : Vector2) (dir : Vector2) width height (tex : Texture2D) (col : Color) : Particle =
    {
        X = pos.X
        Y = pos.Y        
        DirX = dir.X
        DirY = dir.Y
        Width = width
        Height = height
        Texture = tex
        Color = col
        CurrentLifeSpan = 3.0f
        LifeSpan = 3.0f
    }

let addBlocks amount (tex : Texture2D) : Entity list =
    [
        for i = 0 to amount do
          yield
            {
                Block.X = 100.0f + ((float32)(i % 8) * 75.0f)
                Y = 50.0f + ((float32)(i / 8) * 75.0f)
                Width = 50.0f
                Height = 15.0f
                Color = blockColors |> List.sortBy(fun _ -> random.Next()) |> Seq.head
                Texture = tex
            }
     ] |> List.map Entity.CreateBlock

type BreakOutGame() as this =
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

        let paddle = [{Paddle.X = 30.0f; Paddle.Y = 350.0f; Paddle.Height = 10.0f; Paddle.Width = 75.0f; Paddle.Texture = texture; Paddle.State = Normal} |> Entity.CreatePaddle]
        let ball = [{Ball.X = 30.0f; Ball.Y = 100.0f; Ball.VelocityX = 2.0f; Ball.VelocityY = 2.0f; Ball.Height = 15.0f; Ball.Width = 15.0f; Ball.Texture = texture; Ball.Speed = 5.0f;} |> Entity.CreateBall]



        State <-    { State with 
                        Objects = (@) (addBlocks 15 texture) paddle @ ball
                        //Map     = [addParticle (new Vector2(0.0f, 0.0f)) (new Vector2(0.5f, 0.1f)) 50.0f 50.0f texture Color.Green] @ [addParticle (new Vector2(50.0f, 0.0f)) (new Vector2(0.5f, 0.1f)) 50.0f 50.0f texture Color.Blue]
                    }

        //State <- MainUpdate State

        ()
 
    override this.Update (gameTime) =
        

        let destroyed, newEntities = State.Objects |> List.map (fun obj -> obj.Update State.Objects (State.Objects |> List.map(fun x -> x.GetBounds))) 
                                                   |> (fun x -> (x |> List.filter (fun obj -> (obj.OnCollision (State.Objects |> List.map(fun x -> x.GetBounds)))), x |> List.filter (fun obj -> not (obj.OnCollision (State.Objects |> List.map(fun x -> x.GetBounds))))))

        let particles = destroyed |> List.map(fun x -> addParticle (new Vector2(float32(x.GetBounds.X), float32(x.GetBounds.Y))) (new Vector2(0.0f, 0.25f)) 50.0f 15.0f texture Color.Red |> Entity.CreateParticle)
        
                                                   //|> List.filter (fun obj -> not (obj.OnCollision (State.Objects |> List.map(fun x -> x.GetBounds))))

        State <- {  State with
                        Objects = particles @ newEntities
                 }


        (*let newParticles = (destroyed |> List.map(fun x -> addParticle (new Vector2(float32(x.GetBounds.X), float32(x.GetBounds.Y))) (new Vector2(0.5f, 0.1f)) 25.0f 25.0f texture Color.Green)) @ State.Map

        State <- {  State with
                        Map = newParticles |> List.filter (fun p -> p.CurrentLifeSpan > 0.0f && p.Width > 10.0f)
                                           |> List.map (fun p -> if p.CurrentLifeSpan < 2.0f then (addParticle (new Vector2(float32(p.Bounds.X), float32(p.Bounds.Y))) (new Vector2(0.5f / 2.0f, 0.1f / 2.0f)) (p.Width / 2.0f) (p.Height / 2.0f) texture Color.Green) else p)
                                           |> List.map (fun p -> Particle.Update p)
                 }*)

    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.Black

        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)
        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        //State.Map |> List.iteri(fun x tile -> spriteBatch.Draw(tile.Texture tile, new Rectangle(50 + (x % 10 * 30), 50 + (x / 10 * 30), 25, 25), Color.White))

        State.Objects |> List.iter(fun obj -> obj.Draw spriteBatch)
        
        State.Map |> List.iter(fun p -> Particle.Draw p spriteBatch)

        spriteBatch.End()
        