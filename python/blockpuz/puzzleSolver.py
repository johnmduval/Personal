
from puzzleDrawing import PuzzleDrawing
from puzzleModel import PuzzleModel
from collections import deque

class PuzzleSolver:
    def __init__(self, puzzleModel: PuzzleModel):
        self.queue = deque()
        self.queue.append(puzzleModel)
        self.evaluatedSet = set()
        self.skipped = 0

    def getDepth(self, puzzleModel : PuzzleModel):
        depth = 0
        current = puzzleModel
        while current is not None:
            depth += 1
            current = current.parent
        return depth

    def solve(self) -> PuzzleModel:
        while True:

            # pop model from queue
            puzzleModel = self.queue.popleft()

            hash = puzzleModel.getModelHash()
            if self.evaluatedSet.__contains__(hash):
                self.skipped += 1
                continue
            self.evaluatedSet.add(hash)
            
            print(f"Depth: {self.getDepth(puzzleModel)}, Attempts: {len(self.evaluatedSet)}")

            # puzDraw = PuzzleDrawing()
            # puzDraw.drawBlocks(puzzleModel)

            # if model is solved, we are done
            if puzzleModel.isSolved():
                return puzzleModel

            # otherwise, find children (moves which create new models) and put on queue
            moves = puzzleModel.findMoves()
            childModelList = puzzleModel.applyMoves(moves)

            for childModel in childModelList:
                self.queue.append(childModel)
