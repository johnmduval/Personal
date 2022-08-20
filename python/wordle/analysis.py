class Analysis:

    def getRemainingLetters(matches):
        remainingLetters = {}
        for m in matches:
            mdistinct = list(set(m))
            for letter in mdistinct:
                if letter in remainingLetters.keys():
                    remainingLetters[letter] += 1
                else:
                    remainingLetters[letter] = 1


        remainingLettersSorted = sorted(remainingLetters.items(), key=lambda kv: kv[1], reverse=True)
        return remainingLettersSorted

    def printListGrouped(header, list, frequencyByWord = None, perLine = 5, limit = 5000):
        print(header)
        if list is None:
            return

        s = ''
        count = min(len(list), limit)
        print(f'___ {header} (showing {count}/{len(list)}) ___')
        for i in range(0, count):
            s += f"\t{list[i]}"
            if (frequencyByWord is not None and list[i] in frequencyByWord):
                freq = frequencyByWord[list[i]]
                s += f"({freq})"
            else:
                s += "(?)"
            if ((i + 1) % perLine == 0):
                print(s)
                s = ""

        print(s)

    def printLettersByPosition(matches):
        positionList = [ {}, {}, {}, {}, {} ]
        for word in matches:
            for i in range(0, 5):
                letter = word[i]
                dict = positionList[i]
                if letter in dict.keys():
                    dict[letter] += 1
                else:
                    dict[letter] = 1
        
        print('___Letters by position___')
        for i in range(0, 5):
            positionDict = positionList[i]
            positionDictSorted = [(k, positionDict[k]) for k in sorted(positionDict, key=positionDict.get, reverse=True)]
            print(f"pos#{i}:", end='')
            for kv in positionDictSorted: #[:10]:
                print(f"{kv},", end='')
            print("")


    def printMostOverlap(matches):
        listOfLetterCounts = [{},{},{},{},{}]
        for word in matches:
            for i in range(0, 5):
                dict = listOfLetterCounts[i]
                letter = word[i]

                if letter in dict.keys():
                    dict[letter] += 1
                else:
                    dict[letter] = 1

        scoreFromWord = {}
        for word in matches:
            score = 0
            for i in range(0, 5):
                letter = word[i]
                dict = listOfLetterCounts[i]
                score += dict[letter]
            scoreFromWord[word] = score
        
        scoreFromWordSorted = [(k, scoreFromWord[k]) for k in sorted(scoreFromWord, key=scoreFromWord.get, reverse=True)]
        print('___Letter overlap___')
        for kv in scoreFromWordSorted[:10]:
            print(kv)

    def printWordsByLetterPopularity(wordList, force_letters, letterPopularity):
        
        # rank words by letter popularity
        if wordList is None:
            return
        
        # build lookup letter -> score
        scoreFromLetter = {}
        for tuple in letterPopularity:
            letter = tuple[0]
            if letter in force_letters:
                score = tuple[1]
                scoreFromLetter[letter] = score

        scoreFromWord = {}
        for word in wordList:
            score = 0
            for i in range(0, 5):
                letter = word[i]
                if letter in scoreFromLetter.keys():
                    score += scoreFromLetter[letter]
            scoreFromWord[word] = score

        scoreFromWordSorted = [(k, scoreFromWord[k]) for k in sorted(scoreFromWord, key=scoreFromWord.get, reverse=True)]
        
        Analysis.printListGrouped('Words by score', scoreFromWordSorted, limit=10)
