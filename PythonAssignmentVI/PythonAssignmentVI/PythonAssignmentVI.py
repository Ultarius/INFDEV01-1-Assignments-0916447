import math

def CircleMethod():
    radius = 20
    result = ""
    for i in range(0, radius):
        for j in range(0, radius):
            di = i - radius / 2
            dj = j - radius / 2
            dist = math.sqrt( (di * di) + (dj * dj) )
            if dist < radius / 2:
                result += "*"
            else:
                result += " " 
        result += "\n"

    print result

def SquareMethod(_height, _width, _inner):
    result = ""
    for i in range(0, _height):
        for j in range(0, _width):
            if not _inner or (j == 0 or j == (_width - 1)) or (i == 0 or i == (_height - 1)):
                result += "*"
            else:
                result += " "
        result += "\n"

    print result

def FaceMethod():
    radius = 50
    result = ""
    for i in range(0, radius):
        for j in range(0, radius):
            di = i - radius / 2
            dj = j - radius / 2
            dist = math.sqrt( (di * di) + (dj * dj) )
            if dist < radius / 2 and dist > radius / 2.6:
                result += "*"
            else:
                if (di == -2) and (abs(dj) == 4): # Eyes
                    result += "O"
                elif (di == -3) and (abs(dj) == 4): # Brows
                    result += "="
                elif (di == 1) and (dj == 0): # Nose or
                    result += "@"
                elif (di == 4) and (abs(dj) < 3): # Mouth
                    result += "_"
                elif ((di == 2) and (abs(dj) < 3)) or ((di == 1) and (abs(dj) == 3)): # Moustachio
                    result += "#"
                else:
                    result += " " 
        result += "\n"

    print result

def TriangleMethod(_size):
    result = ""
    for i in range(0, _size):
        for j in range(0, _size):
            if i > j:
                result += "*"
            else:
                result += " "
        result += "\n"

    print result

def PiramideMethod(_size):
    result = ""
    for i in range(0, _size):
        for j in range(0, _size * 2):
            if ((_size + i) > j) and ((_size - i) < j):
                result += "*"
            else:
                result += " "
        result += "\n"

    print result

FaceMethod()
TriangleMethod(10)
SquareMethod(18, 18, False)
PiramideMethod(20)