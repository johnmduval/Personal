class WordFrequency:
    original_file = 'wordlists\word_frequency.txt'
    filtered_file = 'wordlists\word_frequency_5letters.txt'

    def __init__(self, filename = filtered_file):
          print('loading word freq...')
          lineNum = 1
          self.frequencyByWord = {}
          with open(filename, encoding='utf-8') as readfile:
              while (line := readfile.readline()):
                line = line.rstrip()
                parts = line.split(',')
                word = parts[0]
                frequency = int(parts[1])
                self.frequencyByWord[word] = frequency
                lineNum = lineNum + 1
          print('done loading')
          
    def filter(filename = 'wordlists\word_frequency.txt'):
          print('filtering input word freq file...')
          lineNum = 1
          with open(filename, encoding='utf-8') as readfile, open(WordFrequency.filtered_file, 'w', encoding='utf-8') as writefile:
              while (line := readfile.readline()):
                line = line.rstrip()
                parts = line.split(' ')
                word = parts[0]
                frequency = int(parts[2])
                if (len(word) == 5):
                  writefile.write(f"{word},{frequency}\n")
                lineNum = lineNum + 1
          print('done filtering')
