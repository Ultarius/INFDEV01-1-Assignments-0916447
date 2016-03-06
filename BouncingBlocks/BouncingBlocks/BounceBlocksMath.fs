module BounceBlocksMath

//Written out long for clearance

    [<Measure>]
    type Meters
    [<Measure>]
    type Kilograms
    [<Measure>]
    type Seconds
    [<Measure>]
    type Newton = Kilograms * Meters / Seconds ^ 2
   
    type Vector2<[<Measure>] 'a> = 
        {
            X: float<'a>
            Y: float<'a>
        }

        // Zero

        static member Zero : Vector2<'a> = 
           {
                X = 0.0<_>
                Y = 0.0<_>
           }

        // Addition

        static member (+) (vector1:Vector2<'a>, vector2:Vector2<'a>) : Vector2<'a> =
            { 
                X = vector1.X + vector2.X
                Y = vector1.Y + vector2.Y
            }

        static member (+) (vector1:Vector2<'a>, k:float<'a>) : Vector2<'a> =
            { 
                X = vector1.X + k
                Y = vector1.Y + k
            }

        static member (+) (k:float<'a>, vector:Vector2<'a>) : Vector2<'a> = 
            k + vector

        // Unary subtraction

        static member (~-) (vector:Vector2<'a>) : Vector2<'a> = 
            {
                X = -vector.X
                Y = -vector.Y
            }

        // Subtraction

        static member (-) (vector1:Vector2<'a>, vector2:Vector2<'a>) : Vector2<'a> = 
            {
                X = vector1.X - vector2.X
                Y = vector1.Y - vector2.Y
            }

        static member (-) (vector:Vector2<'a>, k:float<'a>) : Vector2<'a> = 
            vector + (-k)

        static member (-) (k:float<'a>, vector:Vector2<'a>) : Vector2<'a> = 
            k + (-vector)

        // Multiplying

        static member (*) (vector1:Vector2<'a>, vector2:Vector2<'b>) : Vector2<'a*'b> =
            { 
                X = vector1.X * vector2.X
                Y = vector1.Y * vector2.Y
            }

        static member (*) (vector:Vector2<'a>, k:float<'b>) : Vector2<'a*'b> =
            { 
                X = vector.X * k
                Y = vector.Y * k
            }

        static member (*) (k:float<'b>,v:Vector2<'a>):Vector2<'a> =
            v * k

        // Dividing

        static member (/) (vector1:Vector2<'a>, vector2:Vector2<'a>) : Vector2<'a/'b> =
            { 
                X = vector1.X / vector2.X
                Y = vector1.Y / vector2.Y
            }

        static member (/) (k:float<'a>, vector:Vector2<'a>) : Vector2<'a/'b> =
            { 
                X = vector.X / k
                Y = vector.Y / k
            }

        static member (/) (vector:Vector2<'a>, k:float<'a>) : Vector2<'a/'b> =
            { 
                X = k / vector.X
                Y = k / vector.Y
            }

        // Length

        member this.Length : float<'a> = 
            sqrt(this.X * this.X + this.Y * this.Y)

        // Distance

        static member Distance(vector1:Vector2<'a>, vector2:Vector2<'a>) =
            (vector1 - vector2).Length

        // Normalize

        static member Normalize(vector:Vector2<'a>) : Vector2<1> =
            {
                X = vector.X / vector.Length
                Y = vector.Y / vector.Length
            }
