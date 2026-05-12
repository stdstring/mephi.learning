import sys
from typing import List, Union, Optional

class Step:
    def _check(self) -> bool:
        raise NotImplementedError()

    def get_representation(self) -> str:
        raise NotImplementedError()

class Move(Step):
    def __init__(self, x_pos_value: int, y_pos_value: int):
        self.__x_pos = x_pos_value
        self.__y_pos = y_pos_value

    @property
    def x_pos(self) -> int:
        return self.__x_pos

    @property
    def y_pos(self) -> int:
        return self.__y_pos

    def _check(self) -> bool:
        return (self.__x_pos >= 0) and (self.__y_pos >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"move({self.__x_pos},{self.__y_pos})"

class Color(Step):
    def __init__(self, color_value: str):
        if not isinstance(color_value, str):
            raise TypeError("color_value must be str")
        self.__color = color_value

    @property
    def color(self) -> str:
        return self.__color

    def _check(self) -> bool:
        return (self.__color is not None) and (len(self.__color) > 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"color({self.__color})"

class Point(Step):
    def __init__(self, x_pos_value: int, y_pos_value: int):
        if not isinstance(x_pos_value, int) or not isinstance(y_pos_value, int):
            raise TypeError("x_pos_value and y_pos_value must be int")
        self.__x_pos = x_pos_value
        self.__y_pos = y_pos_value

    @property
    def x_pos(self) -> int:
        return self.__x_pos

    @property
    def y_pos(self) -> int:
        return self.__y_pos

    def _check(self) -> bool:
        return (self.__x_pos >= 0) and (self.__y_pos >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"point({self.__x_pos},{self.__y_pos})"

class Rectangle(Step):
    def __init__(self, x_left_value: int, y_top_value: int, x_right_value: int = None, y_bottom_value: int = None, size: int = None):
        if not isinstance(x_left_value, int) or not isinstance(y_top_value, int):
            raise TypeError("x_left_value and y_top_value must be int")
        if size is not None:
            if not isinstance(size, int):
                raise TypeError("size must be int")
            self.__x_left = x_left_value
            self.__y_top = y_top_value
            self.__x_right = x_left_value + size
            self.__y_bottom = y_top_value + size
        elif x_right_value is not None and y_bottom_value is not None:
            if not isinstance(x_right_value, int) or not isinstance(y_bottom_value, int):
                raise TypeError("x_right_value and y_bottom_value must be int")
            self.__x_left = x_left_value
            self.__y_top = y_top_value
            self.__x_right = x_right_value
            self.__y_bottom = y_bottom_value
        else:
            raise ValueError("Invalid constructor arguments")

    @property
    def x_left(self) -> int:
        return self.__x_left

    @property
    def y_top(self) -> int:
        return self.__y_top

    @property
    def x_right(self) -> int:
        return self.__x_right

    @property
    def y_bottom(self) -> int:
        return self.__y_bottom

    def _check(self) -> bool:
        return (self.__x_left >= 0) and (self.__y_top >= 0) and (self.__x_right >= 0) and (self.__y_bottom >= 0) and (self.__x_right > self.__x_left) and (self.__y_bottom > self.__y_top)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"rectangle({self.__x_left},{self.__y_top},{self.__x_right},{self.__y_bottom})"

class Circle(Step):
    def __init__(self, x_center_value: int, y_center_value: int, radius_value: int):
        if not isinstance(x_center_value, int) or not isinstance(y_center_value, int) or not isinstance(radius_value, int):
            raise TypeError("x_center_value, y_center_value, radius_value must be int")
        self.__x_center = x_center_value
        self.__y_center = y_center_value
        self.__radius = radius_value

    @property
    def x_center(self) -> int:
        return self.__x_center

    @property
    def y_center(self) -> int:
        return self.__y_center

    @property
    def radius(self) -> int:
        return self.__radius

    def _check(self) -> bool:
        min_x = self.__x_center - self.__radius
        min_y = self.__y_center - self.__radius
        return (self.__x_center >= 0) and (self.__y_center >= 0) and (self.__radius > 0) and (min_x >= 0) and (min_y >= 0)

    def get_representation(self) -> str:
        if not self._check():
            raise InvalidOperationException("Bad data")
        return f"circle({self.__x_center},{self.__y_center},{self.__radius})"

class StepStorage:
    def __init__(self):
        self.__steps = []

    def get_steps(self) -> List[Step]:
        return self.__steps

    def add_move(self, param: Union[Move, int, tuple]) -> None:
        if isinstance(param, Move):
            if param is None:
                raise ArgumentNullException("move")
            self.__steps.append(param)
        elif isinstance(param, tuple) and len(param) == 2:
            x, y = param
            if not isinstance(x, int) or not isinstance(y, int):
                raise TypeError("x and y must be int")
            self.__steps.append(Move(x, y))
        else:
            raise TypeError("Invalid parameter type for add_move")

    def add_color(self, param: Union[Color, str]) -> None:
        if isinstance(param, Color):
            if param is None:
                raise ArgumentNullException("color")
            self.__steps.append(param)
        elif isinstance(param, str):
            if param is None:
                raise ArgumentNullException("color")
            self.__steps.append(Color(param))
        else:
            raise TypeError("Invalid parameter type for add_color")

    def add_point(self, param: Union[Point, int, tuple]) -> None:
        if isinstance(param, Point):
            if param is None:
                raise ArgumentNullException("point")
            self.__steps.append(param)
        elif isinstance(param, tuple) and len(param) == 2:
            x, y = param
            if not isinstance(x, int) or not isinstance(y, int):
                raise TypeError("x and y must be int")
            self.__steps.append(Point(x, y))
        else:
            raise TypeError("Invalid parameter type for add_point")

    def add_rectangle(self, *args) -> None:
        if len(args) == 1:
            rectangle = args[0]
            if rectangle is None:
                raise ArgumentNullException("rectangle")
            if not isinstance(rectangle, Rectangle):
                raise TypeError("Expected Rectangle object")
            self.__steps.append(rectangle)
        elif len(args) == 4:
            x_left, y_top, x_right, y_bottom = args
            if not all(isinstance(arg, int) for arg in args):
                raise TypeError("All arguments must be int")
            self.__steps.append(Rectangle(x_left, y_top, x_right, y_bottom))
        elif len(args) == 3:
            x_left, y_top, size = args
            if not all(isinstance(arg, int) for arg in args):
                raise TypeError("All arguments must be int")
            self.__steps.append(Rectangle(x_left, y_top, size=size))
        else:
            raise TypeError("Invalid number of arguments for add_rectangle")

    def add_circle(self, param: Union[Circle, int, tuple]) -> None:
        if isinstance(param, Circle):
            if param is None:
                raise ArgumentNullException("circle")
            self.__steps.append(param)
        elif isinstance(param, tuple) and len(param) == 3:
            x_center, y_center, radius = param
            if not all(isinstance(arg, int) for arg in param):
                raise TypeError("x_center, y_center, radius must be int")
            self.__steps.append(Circle(x_center, y_center, radius))
        else:
            raise TypeError("Invalid parameter type for add_circle")

class IPainter:
    def paint(self, steps: List[Step]) -> None:
        raise NotImplementedError()

class SimplePainter(IPainter):
    def paint(self, steps: List[Step]) -> None:
        if steps is None:
            raise ArgumentNullException("steps")
        if not isinstance(steps, list):
            raise TypeError("steps must be list")
        for step in steps:
            if not isinstance(step, Step):
                raise TypeError("Each element in steps must be Step instance")
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