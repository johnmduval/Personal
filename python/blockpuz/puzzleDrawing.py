
from time import sleep
from graphics import *
from block import Block
from blockMove import BlockMove
from coord import Coord
from puzzleModel import PuzzleModel

class PuzzleDrawing:
    def __init__(self):
        self.win = GraphWin(width = 400, height = 400) # create a window
        self.win.setCoords(0, 0, 60, 70) # set the coordinates of the window; bottom left is (0, 0) and top right is (60, 60)
        p1 = Point(0, 0)
        p2 = Point(60, 60)
        boundary = Rectangle(p1, p2)
        boundary.draw(self.win) # draw it to the window
        self.rectFromBlockNum = {}
        self.textFromBlockNum = {}

    def drawBlocks(self, puzzleModel: PuzzleModel):

        for block in puzzleModel.blockList:
            self.drawBlock(block)

    def drawBlock(self, block : Block):
        if self.rectFromBlockNum.__contains__(block.number):
            oldRect : Rectangle = self.rectFromBlockNum[block.number]
            oldRect.undraw()
        if self.textFromBlockNum.__contains__(block.number):
            oldText : Text = self.textFromBlockNum[block.number]
            oldText.undraw()

        minX = min(block.startCoord.x, block.endCoord.x)
        maxX = max(block.startCoord.x, block.endCoord.x)
        minY = min(block.startCoord.y, block.endCoord.y)
        maxY = max(block.startCoord.y, block.endCoord.y)

        startPoint = Point((minX - 1) * 10, (minY - 1) * 10)
        endPoint = Point(maxX * 10, maxY * 10)
        rect = Rectangle(startPoint, endPoint)
        if block.isRed:
            rect.setFill('red')
        else:
            rect.setFill('blue')
        
        # print(f"minx={minX}, minY={minY}, maxX={maxX}, maxY={maxY}, sq={rect}")
        rect.draw(self.win)

        textPoint = Point(startPoint.x + 5, startPoint.y + 5)
        text = Text(textPoint, f"{block.number}")
        text.draw(self.win)

        self.rectFromBlockNum[block.number] = rect
        self.textFromBlockNum[block.number] = text

    def slideBlock(self, block: Block, move: BlockMove):
        # find starting position        
        if self.rectFromBlockNum.__contains__(block.number):
            rect : Rectangle = self.rectFromBlockNum[block.number]
        
        if self.textFromBlockNum.__contains__(block.number):
            text : Text = self.textFromBlockNum[block.number]
        
        startCoord = move.block.getCoordOnSide(move.direction)
        destCoord = move.destinationCoord

        deltaX = destCoord.x - startCoord.x
        deltaY = destCoord.y - startCoord.y

        for i in range(10):
            rect.move(deltaX, deltaY)
            text.move(deltaX, deltaY)
            sleep(0.02)

    def mouse_handler(self, point):
        self.win.checkMouse()
        pass

    def inputBlocks(self):
        blocks = []
        for i in range(6):
            vl = Line(Point(i * 10, 0), Point(i * 10, 60))
            vl.draw(self.win)
            hl = Line(Point(0, i * 10), Point(60, i * 10))
            hl.draw(self.win)

        blockNum = 0
        isRed = True
        while (True):
            if (self.win.isClosed()):
                break
            coord1 = self.win.getMouse()
            coord2 = self.win.getMouse()
            
            c1x, c1y = int(coord1.x / 10) + 1, int(coord1.y / 10) + 1
            c2x, c2y = int(coord2.x / 10) + 1, int(coord2.y / 10) + 1
            try:
                b = Block(Coord(c1x, c1y), Coord(c2x, c2y), isRed)
                isRed = False
                print(b)
            except ValueError:
                break   # outside the bounds, we are done

            b.number = blockNum
            blockNum += 1

            self.drawBlock(b)
            blocks.append(b)

        return blocks
