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
        if x_right_value is not None and y_bottom_value is not None:
            self._x_left = x_left_value
            self._y_top = y_top_value
            self._x_right = x_right_value
            self._y_bottom = y_bottom_value
        elif size is not None:
            self._x_left = x_left_value
            self._y_top = y_top_value
            self._x_right = x_left_value + size
            self._y_bottom = y_top_value + size
        else:
            raise ArgumentException("Invalid constructor parameters")

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
        self._steps: List[Step] = []

    def get_steps(self) -> List[Step]:
        return self._steps

    def add_move(self, move_or_x, y=None):
        if isinstance(move_or_x, Move):
            if move_or_x is None:
                raise ArgumentNullException("move")
            self._steps.append(move_or_x)
        elif isinstance(move_or_x, int) and isinstance(y, int):
            self._steps.append(Move(move_or_x, y))
        else:
            raise ArgumentException("Invalid arguments")

    def add_color(self, color_or_value):
        if isinstance(color_or_value, Color):
            if color_or_value is None:
                raise ArgumentNullException("color")
            self._steps.append(color_or_value)
        elif isinstance(color_or_value, str):
            if color_or_value is None:
                raise ArgumentNullException("color")
            self._steps.append(Color(color_or_value))
        else:
            raise ArgumentException("Invalid arguments")

    def add_point(self, point_or_x, y=None):
        if isinstance(point_or_x, Point):
            if point_or_x is None:
                raise ArgumentNullException("point")
            self._steps.append(point_or_x)
        elif isinstance(point_or_x, int) and isinstance(y, int):
            self._steps.append(Point(point_or_x, y))
        else:
            raise ArgumentException("Invalid arguments")

    def add_rectangle(self, rectangle_or_x_left, y_top=None, x_right=None, y_bottom=None, size=None):
        if isinstance(rectangle_or_x_left, Rectangle):
            if rectangle_or_x_left is None:
                raise ArgumentNullException("rectangle")
            self._steps.append(rectangle_or_x_left)
        elif isinstance(rectangle_or_x_left, int) and isinstance(y_top, int) and isinstance(x_right, int) and isinstance(y_bottom, int):
            self._steps.append(Rectangle(rectangle_or_x_left, y_top, x_right, y_bottom))
        elif isinstance(rectangle_or_x_left, int) and isinstance(y_top, int) and isinstance(size, int):
            self._steps.append(Rectangle(rectangle_or_x_left, y_top, size=size))
        else:
            raise ArgumentException("Invalid arguments")

    def add_circle(self, circle_or_x_center, y_center=None, radius=None):
        if isinstance(circle_or_x_center, Circle):
            if circle_or_x_center is None:
                raise ArgumentNullException("circle")
            self._steps.append(circle_or_x_center)
        elif isinstance(circle_or_x_center, int) and isinstance(y_center, int) and isinstance(radius, int):
            self._steps.append(Circle(circle_or_x_center, y_center, radius))
        else:
            raise ArgumentException("Invalid arguments")

class IPainter:
    def paint(self, steps: List[Step]) -> None:
        raise NotImplementedError

class SimplePainter(IPainter):
    def paint(self, steps: List[Step]) -> None:
        if steps is None:
            raise ArgumentNullException("steps")
        for step in steps:
            print(step.get_representation())

class ArgumentNullException(Exception):
    pass

class ArgumentException(Exception):
    pass

class InvalidOperationException(Exception):
    pass

class Program:
    @staticmethod
    def main(args: List[str]) -> None:
        storage = StepStorage()
        storage.add_move(3, 5)
        storage.add_color("red")
        storage.add_point(10, 10)
        storage.add_move(100, 100)
        storage.add_rectangle(12, 14, size=55)
        storage.add_circle(16, 16, 13)
        simple_painter = SimplePainter()
        simple_painter.paint(storage.get_steps())

if __name__ == "__main__":
    Program.main(sys.argv)