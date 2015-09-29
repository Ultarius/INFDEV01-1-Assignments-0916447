import turtle



from msvcrt import getch


import math
import time
from turtle import Vec2D

t = turtle.Turtle()


def down():
    t._orient = Vec2D(1.0, 1.0)

def forward (n):
    t.forward(n * 10)

class Direction:
    Up = 1
    Down = 2
    Left = 3
    Right = 4

class Color:
    Red = "red"
    Blue = "blue"
    Black = "black"
    Green = "green"

Red = Color.Red
Blue = Color.Blue
Black = Color.Black
Green = Color.Green

def change_color_to (color):
    t.pencolor(color)
    

    

def turn (n):
    if not (type(n) is Direction.Up):
        t._rotate(n * -1)
    else:
        if n == Direction.Up:
            t._orient = Vec2D(0, 1.0)
        if n == Direction.Left:
            t._orient = Vec2D(-1.0, 0)
        if n == Direction.Down:
            t._orient = Vec2D(0, -1.0)
        if n == Direction.Right:
            t._orient = Vec2D(1.0, 0)


def curry_sum (x):
    def sum_aux(y):
        return (x + y)
    return sum_aux


def get(): return ord(getch())





def run(a):
    
    #loop
    while(True):
        a()