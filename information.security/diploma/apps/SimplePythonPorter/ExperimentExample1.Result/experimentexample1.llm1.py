class Step:
    def Check(self):
        raise NotImplementedError()

    def GetRepresentation(self):
        raise NotImplementedError()


class Move(Step):
    def __init__(self, xPosValue, yPosValue):
        self._xPos = xPosValue
        self._yPos = yPosValue

    def Check(self):
        return (self._xPos >= 0) and (self._yPos >= 0)

    def GetRepresentation(self):
        if not self.Check():
            raise InvalidOperationException("Bad data")
        return f"move({self._xPos},{self._yPos})"


class Color(Step):
    def __init__(self, colorValue):
        self._color = colorValue

    def Check(self):
        return (self._color is not None) and (len(self._color) > 0)

    def GetRepresentation(self):
        if not self.Check():
            raise InvalidOperationException("Bad data")
        return f"color({self._color})"


class Point(Step):
    def __init__(self, xPosValue, yPosValue):
        self._xPos = xPosValue
        self._yPos = yPosValue

    def Check(self):
        return (self._xPos >= 0) and (self._yPos >= 0)

    def GetRepresentation(self):
        if not self.Check():
            raise InvalidOperationException("Bad data")
        return f"point({self._xPos},{self._yPos})"


class Rectangle(Step):
    def __init__(self, xLeftValue, yTopValue, xRightValue, yBottomValue=None):
        if yBottomValue is None:
            size = xRightValue
            self._xLeft = xLeftValue
            self._yTop = yTopValue
            self._xRight = xLeftValue + size
            self._yBottom = yTopValue + size
        else:
            self._xLeft = xLeftValue
            self._yTop = yTopValue
            self._xRight = xRightValue
            self._yBottom = yBottomValue

    def Check(self):
        return (self._xLeft >= 0) and (self._yTop >= 0) and (self._xRight >= 0) and (self._yBottom >= 0) and (self._xRight > self._xLeft) and (self._yBottom > self._yTop)

    def GetRepresentation(self):
        if not self.Check():
            raise InvalidOperationException("Bad data")
        return f"rectangle({self._xLeft},{self._yTop},{self._xRight},{self._yBottom})"


class Circle(Step):
    def __init__(self, xCenterValue, yCenterValue, radiusValue):
        self._xCenter = xCenterValue
        self._yCenter = yCenterValue
        self._radius = radiusValue

    def Check(self):
        minX = self._xCenter - self._radius
        minY = self._yCenter - self._radius
        return (self._xCenter >= 0) and (self._yCenter >= 0) and (self._radius > 0) and (minX >= 0) and (minY >= 0)

    def GetRepresentation(self):
        if not self.Check():
            raise InvalidOperationException("Bad data")
        return f"circle({self._xCenter},{self._yCenter},{self._radius})"


class StepStorage:
    def __init__(self):
        self._steps = []

    def GetSteps(self):
        return self._steps

    def AddMove(self, move):
        if isinstance(move, Move):
            if move is None:
                raise ArgumentNullException("move")
            self._steps.append(move)
        else:
            x, y = move
            self._steps.append(Move(x, y))

    def AddColor(self, color):
        if isinstance(color, Color):
            if color is None:
                raise ArgumentNullException("color")
            self._steps.append(color)
        else:
            if color is None:
                raise ArgumentNullException("color")
            self._steps.append(Color(color))

    def AddPoint(self, point):
        if isinstance(point, Point):
            if point is None:
                raise ArgumentNullException("point")
            self._steps.append(point)
        else:
            x, y = point
            self._steps.append(Point(x, y))

    def AddRectangle(self, rectangle):
        if isinstance(rectangle, Rectangle):
            if rectangle is None:
                raise ArgumentNullException("rectangle")
            self._steps.append(rectangle)
        else:
            if len(rectangle) == 3:
                xLeft, yTop, size = rectangle
                self._steps.append(Rectangle(xLeft, yTop, size))
            else:
                xLeft, yTop, xRight, yBottom = rectangle
                self._steps.append(Rectangle(xLeft, yTop, xRight, yBottom))

    def AddCircle(self, circle):
        if isinstance(circle, Circle):
            if circle is None:
                raise ArgumentNullException("circle")
            self._steps.append(circle)
        else:
            xCenter, yCenter, radius = circle
            self._steps.append(Circle(xCenter, yCenter, radius))


class IPainter:
    def Paint(self, steps):
        raise NotImplementedError()


class SimplePainter(IPainter):
    def Paint(self, steps):
        if steps is None:
            raise ArgumentNullException("steps")
        for step in steps:
            print(step.GetRepresentation())


class Program:
    @staticmethod
    def Main(args):
        storage = StepStorage()
        storage.AddMove((3, 5))
        storage.AddColor("red")
        storage.AddPoint((10, 10))
        storage.AddMove((100, 100))
        storage.AddRectangle((12, 14, 55))
        storage.AddCircle((16, 16, 13))
        simplePainter = SimplePainter()
        simplePainter.Paint(storage.GetSteps())


if __name__ == "__main__":
    class InvalidOperationException(Exception):
        pass

    class ArgumentNullException(Exception):
        pass

    Program.Main([])