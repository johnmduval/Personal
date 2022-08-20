from block import Block
from blockMove import BlockMove
from coord import Coord
from puzzleDrawing import PuzzleDrawing
from puzzleModel import PuzzleModel
from puzzleSolver import PuzzleSolver

puzDraw = PuzzleDrawing()
blocks = puzDraw.inputBlocks()

# blocks = []
# for str in ["1113", "2526", "2343", "3132", "3646", "4445", "4151", "5355", "5666"]:
#     x1, y1, x2, y2 = int(str[0]), int(str[1]), int(str[2]), int(str[3])
#     blocks.append(Block(Coord(x1, y1), Coord(x2, y2)))

# redStartX = 1
# blocks.append(Block(Coord(redStartX, 4), Coord(redStartX + 1, 4), isRed=True))
puzzleModel = PuzzleModel(blocks)

# draw starting position
# puzDraw = PuzzleDrawing()
# puzDraw.drawBlocks(puzzleModel)
# puzDraw.win.getMouse()
# puzDraw.win.close()

# solve the puzzle
puzzleSolver = PuzzleSolver(puzzleModel)
solvedModel = puzzleSolver.solve()

# result is chain starting with last model, walk back up & put in order
sortedModels = []
m = solvedModel
while m is not None:
    sortedModels.append(m)
    m = m.parent
sortedModels.reverse()

# draw initial position, wait for click

puzDraw = PuzzleDrawing()
puzDraw.drawBlocks(sortedModels[0])
puzDraw.win.getMouse()

# draw each subsequent position (erase/redraw moved block), wait for click
for i in range(1, len(sortedModels)):
    model : PuzzleModel = sortedModels[i]
    move : BlockMove = model.move
    movedBlock : Block = next((x for x in model.blockList if x.number == move.block.number), None)

    puzDraw.slideBlock(movedBlock, move)
    #puzDraw.drawBlock(movedBlock)
    if not puzDraw.win.isClosed():
        puzDraw.win.getMouse()

if not puzDraw.win.isClosed():
    puzDraw.win.close() 