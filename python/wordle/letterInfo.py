class LetterInfo:
    def __init__(self, letter, isInWord, requiredPositions = None, restrictedPositions = None):
        self.letter = letter
        self.isInWord = isInWord
        self.requiredPositions = requiredPositions
        self.restrictedPositions = restrictedPositions
    
    def fromBadPositions(letter, restrictedPositionsStr):
        isInWord = True
        restrictedPositions = []
        for char in restrictedPositionsStr:
            restrictedPositions.append(int(char) - 1)
        requiredPositions = []
        return LetterInfo(letter, isInWord, requiredPositions, restrictedPositions)

    def fromGoodPositions(letter, requiredPositionsStr):
        isInWord = True
        requiredPositions = []
        for char in requiredPositionsStr:
            requiredPositions.append(int(char) - 1)
        restrictedPositions = []
        return LetterInfo(letter, isInWord, requiredPositions, restrictedPositions)
