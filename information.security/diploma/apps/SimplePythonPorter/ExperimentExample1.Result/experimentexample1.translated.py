# -*- coding: utf-8 -*-

import system
import system.collections.generic
import abc


class Step(abc.ABC):
    def __init__(self):
        raise system.InvalidOperationException("Abstract method call")

    @abc.abstractmethod
    def _Check(self):
        raise system.InvalidOperationException("Abstract method call")

    @abc.abstractmethod
    def GetRepresentation(self):
        raise system.InvalidOperationException("Abstract method call")

class Move(Step):
    def __init__(self, xPosValue, yPosValue):
        self.__xPos = 0
        self.__yPos = 0
        if isinstance(xPosValue, int) and isinstance(yPosValue, int):
            self.__xPos = xPosValue
            self.__yPos = yPosValue
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def _Check(self):
        return (self.__xPos >= 0) and (self.__yPos >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"move({self.__xPos},{self.__yPos})"

class Color(Step):
    def __init__(self, colorValue):
        self.__color = ""
        if (isinstance(colorValue, str) or colorValue is None):
            self.__color = colorValue
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def _Check(self):
        return (self.__color != None) and (len(self.__color) > 0)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"color({self.__color})"

class Point(Step):
    def __init__(self, xPosValue, yPosValue):
        self.__xPos = 0
        self.__yPos = 0
        if isinstance(xPosValue, int) and isinstance(yPosValue, int):
            self.__xPos = xPosValue
            self.__yPos = yPosValue
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def _Check(self):
        return (self.__xPos >= 0) and (self.__yPos >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"point({self.__xPos},{self.__yPos})"

class Rectangle(Step):
    def __init__(self, *args):
        self.__xLeft = 0
        self.__yTop = 0
        self.__xRight = 0
        self.__yBottom = 0
        if (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):
            xLeftValue = args[0]
            yTopValue = args[1]
            size = args[2]
            self.__xLeft = xLeftValue
            self.__yTop = yTopValue
            self.__xRight = xLeftValue + size
            self.__yBottom = yTopValue + size
        elif (len(args) == 4) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int) and isinstance(args[3], int):
            xLeftValue = args[0]
            yTopValue = args[1]
            xRightValue = args[2]
            yBottomValue = args[3]
            self.__xLeft = xLeftValue
            self.__yTop = yTopValue
            self.__xRight = xRightValue
            self.__yBottom = yBottomValue
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def _Check(self):
        return (self.__xLeft >= 0) and (self.__yTop >= 0) and (self.__xRight >= 0) and (self.__yBottom >= 0) and (self.__xRight > self.__xLeft) and (self.__yBottom > self.__yTop)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"rectangle({self.__xLeft},{self.__yTop},{self.__xRight},{self.__yBottom})"

class Circle(Step):
    def __init__(self, xCenterValue, yCenterValue, radiusValue):
        self.__xCenter = 0
        self.__yCenter = 0
        self.__radius = 0
        if isinstance(xCenterValue, int) and isinstance(yCenterValue, int) and isinstance(radiusValue, int):
            self.__xCenter = xCenterValue
            self.__yCenter = yCenterValue
            self.__radius = radiusValue
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def _Check(self):
        minX = self.__xCenter - self.__radius
        minY = self.__yCenter - self.__radius
        return (self.__xCenter >= 0) and (self.__yCenter >= 0) and (self.__radius > 0) and (minX >= 0) and (minY >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"circle({self.__xCenter},{self.__yCenter},{self.__radius})"

class StepStorage:
    def __init__(self):
        self.__steps = system.collections.generic.List()

    def AddCircle(self, *args):
        if (len(args) == 1) and (isinstance(args[0], Circle) or args[0] is None):
            circle = args[0]
            if circle == None:
                raise system.ArgumentNullException("circle")
            self.__steps.Add(circle)
        elif (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):
            xCenter = args[0]
            yCenter = args[1]
            radius = args[2]
            self.__steps.Add(Circle(xCenter, yCenter, radius))
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def AddColor(self, *args):
        if (len(args) == 1) and (isinstance(args[0], Color) or args[0] is None):
            color = args[0]
            if color == None:
                raise system.ArgumentNullException("color")
            self.__steps.Add(color)
        elif (len(args) == 1) and (isinstance(args[0], str) or args[0] is None):
            color = args[0]
            if color == None:
                raise system.ArgumentNullException("color")
            self.__steps.Add(Color(color))
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def AddMove(self, *args):
        if (len(args) == 1) and (isinstance(args[0], Move) or args[0] is None):
            move = args[0]
            if move == None:
                raise system.ArgumentNullException("move")
            self.__steps.Add(move)
        elif (len(args) == 2) and isinstance(args[0], int) and isinstance(args[1], int):
            x = args[0]
            y = args[1]
            self.__steps.Add(Move(x, y))
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def AddPoint(self, *args):
        if (len(args) == 1) and (isinstance(args[0], Point) or args[0] is None):
            point = args[0]
            if point == None:
                raise system.ArgumentNullException("point")
            self.__steps.Add(point)
        elif (len(args) == 2) and isinstance(args[0], int) and isinstance(args[1], int):
            x = args[0]
            y = args[1]
            self.__steps.Add(Point(x, y))
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def AddRectangle(self, *args):
        if (len(args) == 1) and (isinstance(args[0], Rectangle) or args[0] is None):
            rectangle = args[0]
            if rectangle == None:
                raise system.ArgumentNullException("rectangle")
            self.__steps.Add(rectangle)
        elif (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):
            xLeft = args[0]
            yTop = args[1]
            size = args[2]
            self.__steps.Add(Rectangle(xLeft, yTop, size))
        elif (len(args) == 4) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int) and isinstance(args[3], int):
            xLeft = args[0]
            yTop = args[1]
            xRight = args[2]
            yBottom = args[3]
            self.__steps.Add(Rectangle(xLeft, yTop, xRight, yBottom))
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

    def GetSteps(self):
        return self.__steps

class IPainter(abc.ABC):
    @abc.abstractmethod
    def Paint(self, steps):
        if (isinstance(steps, system.collections.generic.IList) or steps is None):
            raise system.InvalidOperationException("Abstract method call")
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

class SimplePainter(IPainter):
    def __init__(self):
        pass

    def Paint(self, steps):
        if (isinstance(steps, system.collections.generic.IList) or steps is None):
            if steps == None:
                raise system.ArgumentNullException("steps")
            for step in steps:
                system.Console.WriteLine(step.GetRepresentation())
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

class Program:
    def __init__(self):
        pass

    @staticmethod
    def Main(args):
        if (isinstance(args, list) or args is None):
            storage = StepStorage()
            storage.AddMove(3, 5)
            storage.AddColor("red")
            storage.AddPoint(10, 10)
            storage.AddMove(100, 100)
            storage.AddRectangle(12, 14, 55)
            storage.AddCircle(16, 16, 13)
            simplePainter = SimplePainter()
            simplePainter.Paint(storage.GetSteps())
        else:
            raise system.InvalidOperationException("Unrecognized combination of arguments")

if __name__ == "__main__":
    Program.Main([])