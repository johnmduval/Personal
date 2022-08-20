
from block import Block
from coord import Coord
from direction import Direction


class BlockMove:
    def __init__(self, block: Block, direction: Direction, destinationCoord: Coord):
        self.block = block
        self.direction = direction
        self.destinationCoord = destinationCoord
    
    def __repr__(self) -> str:
        return f"Block #{self.block.number} {self.direction} to {self.destinationCoord}"
    
    def applyMoveToBlock(self) -> Block:
        deltaX = 0
        deltaY = 0
        startCoord = self.block.getCoordOnSide(self.direction)
        if self.direction == Direction.up or self.direction == Direction.down:
            deltaY = self.destinationCoord.y - startCoord.y
        else:
            deltaX = self.destinationCoord.x - startCoord.x
        
        newStartCoord = Coord(self.block.startCoord.x + deltaX, self.block.startCoord.y + deltaY)
        newEndCoord = Coord(self.block.endCoord.x + deltaX, self.block.endCoord.y + deltaY)
        
        newBlock = Block(newStartCoord, newEndCoord, self.block.isRed)
        return newBlock


