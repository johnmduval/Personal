
from msilib.schema import Error
from coord import Coord
from direction import Direction


class Block:
    def __init__(self, startCoord: Coord, endCoord: Coord, isRed = False):
        if startCoord.x != endCoord.x and startCoord.y != endCoord.y:
            raise ValueError(f"start/end coordinates invalid: {startCoord}/{endCoord}")

        if (startCoord.x > endCoord.x) or (startCoord.y > endCoord.y):
            startCoord, endCoord = endCoord, startCoord

        self.startCoord = startCoord
        self.endCoord = endCoord
        self.isVertical = self.startCoord.x == self.endCoord.x
        self.isRed = isRed
        self.number = 0

    def __repr__(self) -> str:
        return f"[{self.startCoord}-{self.endCoord}]"
    
    def getAllCoords(self):
        if self.isVertical:
            x = self.startCoord.x
            miny, maxy = min(self.startCoord.y, self.endCoord.y), max(self.startCoord.y, self.endCoord.y)
            coordList = [Coord(x, y) for y in range(miny, maxy + 1)]
        else:
            y = self.startCoord.y
            minx, maxx = min(self.startCoord.x, self.endCoord.x), max(self.startCoord.x, self.endCoord.x)
            coordList = [Coord(x, y) for x in range(minx, maxx + 1)]
        return coordList

    def getCoordOnSide(self, direction: Direction):
        if direction == Direction.up:
            return self.getTopCoord()
        elif direction == Direction.down:
            return self.getBottomCoord()
        elif direction == Direction.right:
            return self.getRightCoord()
        elif direction == Direction.left:
            return self.getLeftCoord()

    def getTopCoord(self) -> Coord:
        if (not self.isVertical):
            raise Error("Why asking for top coord for horizontal block?")
        y = max(self.startCoord.y, self.endCoord.y)
        x = self.startCoord.x # x same for start/end
        return Coord(x, y)

    def getBottomCoord(self) -> Coord:
        if (not self.isVertical):
            raise Error("Why asking for bottom coord for horizontal block?")
        y = min(self.startCoord.y, self.endCoord.y)
        x = self.startCoord.x # x same for start/end
        return Coord(x, y)

    def getRightCoord(self) -> Coord:
        if (self.isVertical):
            raise Error("Why asking for right coord for vertical block?")
        x = max(self.startCoord.x, self.endCoord.x)
        y = self.startCoord.y # y same for start/end
        return Coord(x, y)

    def getLeftCoord(self) -> Coord:
        if (self.isVertical):
            raise Error("Why asking for left coord for vertical block?")
        x = min(self.startCoord.x, self.endCoord.x)
        y = self.startCoord.y # y same for start/end
        return Coord(x, y)
