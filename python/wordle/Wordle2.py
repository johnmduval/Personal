from ctypes.wintypes import HKEY
#from operator import truediv
import os
from dictionary import Dictionary
from wordfrequency import WordFrequency
from letterInfoList import LetterInfoList
from analysis import Analysis

clear = lambda: os.system('cls')
clear()

letterInfoList = LetterInfoList()

#############################################################
wordlist = "wordle"  # 5k 10k  99k 349k wordle



gray = 'rselinth'
yellow = 'a1o3a3c4'
green = 'c1o2'

force_only = ''
force = ''
minWordFreq = 40



#TODO: forceMatch should eliminate words with gray
#############################################################

if (force_only):
    yellow, green, gray, minWordFreq = '', '', '', 1

letterInfoList.grayLetters(gray)
letterInfoList.yellowLetterList(yellow)
letterInfoList.greenLetterList(green)
letterInfoList.forceLetters(force_only if force_only else force) 

wordFile = f'wordlists\\words_{wordlist}.txt'
dictionary = Dictionary(wordFile)
# WordFrequency.filter()
wordFreq = WordFrequency()
# wordFreq = WordFrequency('C:\\Users\\johnm\\OneDrive\\Documents\\Temp\\foo.txt')

if (force_only):
    matches = dictionary.BestMatches(letterInfoList, force_only)
else:
    matches = dictionary.FindMatches(letterInfoList, wordFreq, minWordFreq)

Analysis.printListGrouped(f'Possible matches={len(matches)}', matches, wordFreq.frequencyByWord, perLine = 8, limit=100)
Analysis.printLettersByPosition(matches)

Analysis.printMostOverlap(matches)
remainingLetters = Analysis.getRemainingLetters(matches)
Analysis.printListGrouped('Remaining letter popularity', remainingLetters)

# use remaining letters to find list of possible next words
goodLetters = [li.letter for li in letterInfoList.letterInfoList if li.isInWord]

force_letters = []
for l in remainingLetters:
    if l[0] in goodLetters:
        continue    # don't include known-good letters
    force_letters.append(l[0])
    # if len(force_letters) == 5:
    #     break

print(f"Finding best matches for '{force_letters}'")
matches = dictionary.BestMatches(letterInfoList, force_letters)
Analysis.printWordsByLetterPopularity(matches, force_letters, remainingLetters)

print('done')
