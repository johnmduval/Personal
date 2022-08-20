
from direction import Direction


class Coord:
    def __init__(self, x, y):
        if x < 1 or y < 1 or x > 6 or y > 6:
            raise ValueError(f"invalid coordinate: ({x},{y})")

        self.x = x
        self.y = y

    def __repr__(self) -> str:
        return f"({self.x},{self.y})"
    
    def __eq__(self, obj):
        return isinstance(obj, Coord) and obj.x == self.x and obj.y == self.y

    def offset(self, direction: Direction):
        x = self.x
        y = self.y
        match direction:
            case Direction.up:
                y = y + 1
            case Direction.down:
                y = y - 1
            case Direction.right:
                x = x + 1
            case Direction.left:
                x = x - 1
            case _:
                raise ValueError("direction not specified")

        if x < 1 or y < 1 or x > 6 or y > 6:
            return None
        return Coord(x, y)