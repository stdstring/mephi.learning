# -*- coding: utf-8 -*-

import abc

class Step(abc.ABC):
    @abc.abstractmethod
    def _Check(self):
        pass

    @abc.abstractmethod
    def GetRepresentation(self):
        pass


class Move(Step):
    def __init__(self, xPosValue, yPosValue):
        self.__xPos = xPosValue
        self.__yPos = yPosValue

    def _Check(self):
        return (self.__xPos >= 0) and (self.__yPos >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise ValueError("Bad data")
        return f"move({self.__xPos},{self.__yPos})"


class Color(Step):
    def __init__(self, colorValue):
        self.__color = colorValue

    def _Check(self):
        return (self.__color != None) and (len(self.__color) > 0)

    def GetRepresentation(self):
        if not self._Check():
            raise system.ValueError("Bad data")
        return f"color({self.__color})"


class Point(Step):
    def __init__(self, xPosValue, yPosValue):
        self.__xPos = xPosValue
        self.__yPos = yPosValue

    def _Check(self):
        return (self.__xPos >= 0) and (self.__yPos >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise ValueError("Bad data")
        return f"point({self.__xPos},{self.__yPos})"


class Rectangle(Step):
    def __init__(self, xLeftValue, yTopValue, size=None, xRightValue=None, yBottomValue=None):
        self.__xLeft = xLeftValue
        self.__yTop = yTopValue
        if size is not None:
            self.__xRight = xLeftValue + size
            self.__yBottom = yTopValue + size
        elif (xRightValue is not None) and (yBottomValue is not None):
            self.__xRight = xRightValue
            self.__yBottom = yBottomValue
        else:
            raise ValueError("Bad data")

    def _Check(self):
        return (self.__xLeft >= 0) and (self.__yTop >= 0) and (self.__xRight >= 0) and (self.__yBottom >= 0) and (self.__xRight > self.__xLeft) and (self.__yBottom > self.__yTop)

    def GetRepresentation(self):
        if not self._Check():
            raise system.InvalidOperationException("Bad data")
        return f"rectangle({self.__xLeft},{self.__yTop},{self.__xRight},{self.__yBottom})"


class Circle(Step):
    def __init__(self, xCenterValue, yCenterValue, radiusValue):
        self.__xCenter = xCenterValue
        self.__yCenter = yCenterValue
        self.__radius = radiusValue

    def _Check(self):
        minX = self.__xCenter - self.__radius
        minY = self.__yCenter - self.__radius
        return (self.__xCenter >= 0) and (self.__yCenter >= 0) and (self.__radius > 0) and (minX >= 0) and (minY >= 0)

    def GetRepresentation(self):
        if not self._Check():
            raise ValueError("Bad data")
        return f"circle({self.__xCenter},{self.__yCenter},{self.__radius})"


class StepStorage:
    def __init__(self):
        self.__steps = []

    def AddCircle(self, circle=None, xCenter=None, yCenter=None, radius=None):
        if circle is not None:
            self.__steps.append(circle)
        elif (xCenter is not None) and (yCenter is not None) and (radius is not None):
            self.__steps.append(Circle(xCenter, yCenter, radius))
        else:
            raise ValueError("Bad data")

    def AddColor(self, color=None, colorString=None):
        if color is not None:
            self.__steps.append(color)
        elif colorString is not None:
            self.__steps.append(Color(colorString))
        else:
            raise ValueError("Bad data")

    def AddMove(self, move=None, x=None, y=None):
        if move is not None:
            self.__steps.append(move)
        elif (x is not None) and (y is not None):
            self.__steps.append(Move(x, y))
        else:
            raise ValueError("Bad data")

    def AddPoint(self, point=None, x=None, y=None):
        if point is not None:
            self.__steps.append(point)
        elif (x is not None) and (y is not None):
            self.__steps.append(Point(x, y))
        else:
            raise ValueError("Bad data")

    def AddRectangle(self, rectangle=None, xLeft=None, yTop=None, size=None, xRight=None, yBottom=None):
        if rectangle is not None:
            self.__steps.append(rectangle)
        elif (xLeft is not None) and (yTop is not None) and (size is not None):
            self.__steps.append(Rectangle(xLeft, yTop, size=size))
        elif (xLeft is not None) and (yTop is not None) and (xRight is not None) and (yBottom is not None):
            self.__steps.append(Rectangle(xLeft, yTop, xRightValue=xRight, yBottomValue=yBottom))
        else:
            raise ValueError("Bad data")

    def GetSteps(self):
        return self.__steps


class IPainter(abc.ABC):
    @abc.abstractmethod
    def Paint(self, steps):
        pass


class SimplePainter(IPainter):
    def Paint(self, steps):
        for step in steps:
            print(step.GetRepresentation())


class Program:
    @staticmethod
    def Main():
        storage = StepStorage()
        storage.AddMove(x=3, y=5)
        storage.AddColor(colorString="red")
        storage.AddPoint(x=10, y=10)
        storage.AddMove(x=100, y=100)
        storage.AddRectangle(xLeft=12, yTop=14, size=55)
        storage.AddCircle(xCenter=16, yCenter=16, radius=13)
        simplePainter = SimplePainter()
        simplePainter.Paint(storage.GetSteps())


if __name__ == "__main__":
    Program.Main()
