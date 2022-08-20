from letterInfo import LetterInfo

class LetterInfoList:
    def __init__(self) -> None:
        self.letterInfoList = [ ]

    def yellowLetterList(self, list):
        
        while (len(list) > 1):
            letterAndIndex = list[:2]
            list = list[2:]
            letter = letterAndIndex[0]
            index = letterAndIndex[1]
            self.letterInfoList.append(LetterInfo.fromBadPositions(letter, index))

    def greenLetterList(self, list):
        while (len(list) > 1):
            letterAndIndex = list[:2]
            list = list[2:]
            letter = letterAndIndex[0]
            index = letterAndIndex[1]
            self.letterInfoList.append(LetterInfo.fromGoodPositions(letter, index))

    def yellowLetter(self, letterAndIndex):
        letter = letterAndIndex[0]
        index = letterAndIndex[1]
        self.letterInfoList.append(LetterInfo.fromBadPositions(letter, index))
        
    def grayLetters(self, letters):
        for badLetter in letters:
            self.letterInfoList.append(LetterInfo(badLetter, isInWord=False))
    
    def forceLetters(self, letters):
        for l in letters:
            self.letterInfoList.append(LetterInfo.fromBadPositions(l, ''))

    def greenLetters(self, letters):
        for index in range(5):
            if (letters[index] != '_'):
                self.letterInfoList.append(LetterInfo.fromGoodPositions(letters[index], str(index + 1)))