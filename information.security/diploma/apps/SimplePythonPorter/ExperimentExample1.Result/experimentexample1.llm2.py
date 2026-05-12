import sys
from typing import List

class Step:
    def _check(self) -> bool:
        raise NotImplementedError

    def get_representation(self) -> str:
        raise NotImplementedError

class Move(Step):
    def __init__(self, x_pos_value: int, y_pos_value: int):
        self._x_pos = x_pos_value
        self._y_pos = y_pos_value

    def _check(self) -> bool:
        return (self._x_pos >= 0) and (self._y_pos >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"move({self._x_pos},{self._y_pos})"

class Color(Step):
    def __init__(self, color_value: str):
        self._color = color_value

    def _check(self) -> bool:
        return (self._color is not None) and (len(self._color) > 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"color({self._color})"

class Point(Step):
    def __init__(self, x_pos_value: int, y_pos_value: int):
        self._x_pos = x_pos_value
        self._y_pos = y_pos_value

    def _check(self) -> bool:
        return (self._x_pos >= 0) and (self._y_pos >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"point({self._x_pos},{self._y_pos})"

class Rectangle(Step):
    def __init__(self, x_left_value: int, y_top_value: int, x_right_value: int = None, y_bottom_value: int = None, size: int = None):
        if size is not None:
            self._x_left = x_left_value
            self._y_top = y_top_value
            self._x_right = x_left_value + size
            self._y_bottom = y_top_value + size
        else:
            self._x_left = x_left_value
            self._y_top = y_top_value
            self._x_right = x_right_value
            self._y_bottom = y_bottom_value

    def _check(self) -> bool:
        return (self._x_left >= 0) and (self._y_top >= 0) and (self._x_right >= 0) and (self._y_bottom >= 0) and (self._x_right > self._x_left) and (self._y_bottom > self._y_top)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"rectangle({self._x_left},{self._y_top},{self._x_right},{self._y_bottom})"

class Circle(Step):
    def __init__(self, x_center_value: int, y_center_value: int, radius_value: int):
        self._x_center = x_center_value
        self._y_center = y_center_value
        self._radius = radius_value

    def _check(self) -> bool:
        min_x = self._x_center - self._radius
        min_y = self._y_center - self._radius
        return (self._x_center >= 0) and (self._y_center >= 0) and (self._radius > 0) and (min_x >= 0) and (min_y >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"circle({self._x_center},{self._y_center},{self._radius})"

class StepStorage:
    def __init__(self):
        self._steps = []

    def get_steps(self) -> List[Step]:
        return self._steps

    def add_move(self, move):
        if isinstance(move, Move):
            if move is None:
                raise ArgumentNullException("move")
            self._steps.append(move)
        else:
            x, y = move
            self._steps.append(Move(x, y))

    def add_color(self, color):
        if isinstance(color, Color):
            if color is None:
                raise ArgumentNullException("color")
            self._steps.append(color)
        else:
            if color is None:
                raise ArgumentNullException("color")
            self._steps.append(Color(color))

    def add_point(self, point):
        if isinstance(point, Point):
            if point is None:
                raise ArgumentNullException("point")
            self._steps.append(point)
        else:
            x, y = point
            self._steps.append(Point(x, y))

    def add_rectangle(self, *args):
        if len(args) == 1:
            rectangle = args[0]
            if rectangle is None:
                raise ArgumentNullException("rectangle")
            self._steps.append(rectangle)
        elif len(args) == 4:
            x_left, y_top, x_right, y_bottom = args
            self._steps.append(Rectangle(x_left, y_top, x_right, y_bottom))
        elif len(args) == 3:
            x_left, y_top, size = args
            self._steps.append(Rectangle(x_left, y_top, size=size))

    def add_circle(self, circle):
        if isinstance(circle, Circle):
            if circle is None:
                raise ArgumentNullException("circle")
            self._steps.append(circle)
        else:
            x_center, y_center, radius = circle
            self._steps.append(Circle(x_center, y_center, radius))

class IPainter:
    def paint(self, steps: List[Step]) -> None:
        raise NotImplementedError

class SimplePainter(IPainter):
    def paint(self, steps: List[Step]) -> None:
        if steps is None:
            raise ArgumentNullException("steps")
        for step in steps:
            print(step.get_representation())

class Program:
    @staticmethod
    def main(args: List[str]) -> None:
        storage = StepStorage()
        storage.add_move((3, 5))
        storage.add_color("red")
        storage.add_point((10, 10))
        storage.add_move((100, 100))
        storage.add_rectangle(12, 14, 55)
        storage.add_circle((16, 16, 13))
        simple_painter = SimplePainter()
        simple_painter.paint(storage.get_steps())

class InvalidOperationException(Exception):
    pass

class ArgumentNullException(Exception):
    pass

if __name__ == "__main__":
    Program.main(sys.argv[1:])