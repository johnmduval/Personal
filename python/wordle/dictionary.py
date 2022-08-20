from wordfrequency import WordFrequency


class Dictionary:
    def __init__(self, filename = 'wordlists\dictionary.txt'):
          self.words = []
          with open(filename) as file:
              while (line := file.readline().rstrip()):
                  if "'" in line:
                      continue
                  if len(line) == 5:
                    self.words.append(line.lower())
          
          #remove dupes
          self.words = list(set(self.words))
          self.words.sort()

          remove_words = [ 'plath', 'mckay', 'milan', 'milne', 'golda', 'volga', 'pablo', 'gallo', 'thant', 'thanh', 'upton', 'lydia',
           'brady', 'prada', 'bragg', 'grady', 'lizzy', 'lippi', 'bligh', 'adele', 'edith', 'keith', 'bizet', 'kieth', 'timex', 'tibet',
           'ernie', 'irene', 'emile', 'lille', 'wilde', 'clive', 'kiowa', 'naomi', 'poona', 'fiona', 'debra', 'jared', 'edgar', 'vader',
           'bloch', 'leroy', 'bruno', 'bryon' ]
          for w in remove_words:
              if w in self.words: self.words.remove(w)

    def BestMatches(self, letterInfoList, forceLetters):
        
        gray = []
        for info in letterInfoList.letterInfoList:
            if not info.isInWord:
                gray.append(info.letter)

        matchesByCount = {1: [], 2: [], 3: [], 4: [], 5: []}
        for word in self.words:
            if (len(word) != 5):
                continue

            skip_word = False
            for l in gray:
                if (l in word):
                    skip_word = True
                    break
            
            if (skip_word):
                continue

            # find # of matching letters
            matchingLetterCount = 0
            for l in forceLetters:
            # for l in word:
                if l in word:
                    matchingLetterCount += 1

            # add to results
            if (matchingLetterCount > 1):
                matchesByCount[matchingLetterCount].append(word)

        # get list of words with highest # of matching letters (5 is best, then 4...)
        matchesList = None
        for i in range(5, 1, -1):
            matchesList = matchesByCount[i]
            if (len(matchesList) > 0):
                return matchesList
        

    def FindMatches(self, letterInfoList, wordFrequency: WordFrequency, minWordFrequency):
        
        matches = []

        for word in self.words:
            if (len(word) != 5):
                continue

            if (wordFrequency is not None):
                if word not in wordFrequency.frequencyByWord:
                    continue
                if (wordFrequency.frequencyByWord[word] < minWordFrequency):
                    continue

            isMatch = True

            for letterInfo in letterInfoList.letterInfoList:
                if (not letterInfo.isInWord):
                    if letterInfo.letter in word:
                        isMatch = False
                        break

                else:
                    # isInWord = True here, so word must contain letter or it's not a match
                    if letterInfo.letter not in word:
                        isMatch = False
                        break
                    
                    # indexInWord = word.find(letterInfo.letter)
                    indexInWordList = [i for i, ltr in enumerate(word) if ltr == letterInfo.letter]

                    # check required positions
                    for requiredIdx in letterInfo.requiredPositions:
                        if requiredIdx not in indexInWordList:
                            isMatch = False
                            break

                    # check restricted positions
                    for indexInWord in indexInWordList:
                        if indexInWord in letterInfo.restrictedPositions:
                            isMatch = False
                            break
                
            if isMatch:
                matches.append(word)
        
        return matches

    def CountByEnding(self, wordList):
        
        results = {}

        # self_words = ["apple", "could", "would", "liner", "gould", "mould", "bould", "dould", "fould"]
        for word in wordList:
            if (len(word) != 5):
                continue
            last4 = word[1:]

            if last4 not in results:
                results[last4] = 1
            else:
                results[last4] += 1
        
        for k in results:
            if results[k] > 5:
                print(f"{k}: {results[k]}")
        return results
