from copy import deepcopy
from block import Block
from blockMove import BlockMove
from coord import Coord
from direction import Direction

class PuzzleModel:
    def __init__(self, blockList: list, parent = None, move = None):
        self.blockList = blockList
        self.parent = parent
        self.move = move

        # number the blocks
        for i in range(len(blockList)):
            block = blockList[i]
            block.number = i

        self.__checkForOverlap__()
    
    def __checkForOverlap__(self):
        for i in range(len(self.blockList)):
            currentBlock : Block = self.blockList[i]
            currentCoords = currentBlock.getAllCoords()

            for j in range(i + 1, len(self.blockList)):
                nextBlock : Block = self.blockList[j]
                nextCoords = nextBlock.getAllCoords()

                for c in nextCoords:
                    if c in currentCoords:
                        raise ValueError(f"Blocks #{currentBlock.number} and #{nextBlock.number} are overlapping: {currentBlock}, {nextBlock}")


    def findEmptySquares(self):
        allSquares = []
        for x in range(1, 7):
            for y in range(1, 7):
                c = Coord(x, y)
                allSquares.append(c)
        
        for block in self.blockList:
            for x in range(block.startCoord.x, block.endCoord.x + 1):
                for y in range(block.startCoord.y, block.endCoord.y + 1):
                    allSquares = list(filter(lambda sq: sq.x != x or sq.y != y, allSquares))

        # for sq in allSquares:
        #     print(sq)

        return allSquares

    def getModelHash(self):
        stringList = []
        for b in self.blockList:
            redOrBlue = "R" if b.isRed else "B"
            stringList.append(f"{b.number}{redOrBlue}{b.startCoord.x}{b.startCoord.y}{b.endCoord.x}{b.endCoord.y}")
        ret = "_".join(stringList)
        return ret

    def findMoves(self):
        moves = []
        emptySquares = self.findEmptySquares()
        for block in self.blockList:
            # look for empty square(s) in given directions, return possible move if any
            blockMoves = [
                self.findMoveInDirection(block, Direction.up, emptySquares),
                self.findMoveInDirection(block, Direction.down, emptySquares),
                self.findMoveInDirection(block, Direction.right, emptySquares),
                self.findMoveInDirection(block, Direction.left, emptySquares),
            ]
            blockMoves = [x for x in blockMoves if x is not None]
            moves.extend(blockMoves)
        return moves

    def findMoveInDirection(self, block: Block, direction: Direction, emptySquares: list) -> BlockMove:
        
        verticalDirections = [ Direction.up, Direction.down ]
        horizontalDirections = [ Direction.left, Direction.right ]
        if block.isVertical and direction not in verticalDirections:
            return None
        if not block.isVertical and direction not in horizontalDirections:
            return None
        
        blockCoord = block.getCoordOnSide(direction)
        
        lastEmptyCoord = None
        checkCoord = blockCoord.offset(direction)
        while (checkCoord != None):
            if (checkCoord in emptySquares):
                lastEmptyCoord = checkCoord
            else:
                break
            checkCoord = checkCoord.offset(direction)

        if (lastEmptyCoord != None):
            return BlockMove(block, direction, lastEmptyCoord)
        return None

    def applyMoves(self, moveList: list) -> list:
        childMoveList = []
        for move in moveList:
            # print(move)
            childPuzzleModel = self.applyMove(move)
            childMoveList.append(childPuzzleModel)

        return childMoveList
    
    def applyMove(self, move: BlockMove):
        # first create copy of current PuzzleModel
        blockList = []
        block: Block
        for block in self.blockList:
            newBlock : Block
            if (block.number == move.block.number):
                newBlock = move.applyMoveToBlock()
            else:
                newBlock = deepcopy(block)
            blockList.append(newBlock)

        puzzleModel = PuzzleModel(blockList, self, move)

        return puzzleModel

    def isSolved(self) -> bool:
        redBlock : Block
        redBlock = next((x for x in self.blockList if x.isRed == True), None)
        
        emptySquares = self.findEmptySquares()
        checkCoord = redBlock.getCoordOnSide(Direction.right).offset(Direction.right)
        while True:
            if not checkCoord in emptySquares:
                return False
            checkCoord = checkCoord.offset(Direction.right)
            if checkCoord is None:
                break

        return True


