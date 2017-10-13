using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathPool
{

    class Guess
    {
        public Guess(string character, int points)
        {
            this.Character = character;
            this.Points = points;
        }
        public string Character { get; set; }
        public int Points { get; set; }
    }

    class Player
    {
        public Player()
        {
            this.GuessList = new List<Guess>();
        }
        List<Guess> GuessList { get; set; }
    }

    class Program
    {
        static List<string> Characters = new List<string>
        {
            "Theon",
            "Cersei",
            "Littlefinger",
            "Tormund",
            "Grey Worm",
            "Jamie",
            "Davos",
            "Daenerys",
            "The Hound",
            "Varys",
            "Melisandre",
            "Sansa",
            "Brienne",        
        };

        static List<string> KnownDeadChars = new List<string>
        {
            "Dragon",
        };

        #region players
        static Dictionary<string, List<Guess>> Players = new Dictionary<string,List<Guess>>
        {
            {
                "Mike T.", new List<Guess> {
                    new Guess("Littlefinger", 25),
                    new Guess("Dragon", 15),
                    new Guess("Jamie", 50),
                    new Guess("Davos", 10),
                }
            },
            {
                "Brendan T.", new List<Guess> {
                    new Guess("Grey Worm", 50),
                    new Guess("Cersei", 25),
                    new Guess("Theon", 15),
                    new Guess("Tormund", 10),
                }
            },
            {
                "Scott P.", new List<Guess> {
                    new Guess("Cersei", 40),
                    new Guess("Dragon", 35),
                    new Guess("Tormund", 20),
                    new Guess("Davos", 5),
                }
            },
            {
                "Donavin B.", new List<Guess> {
                    new Guess("Tormund", 40),
                    new Guess("Davos", 30),
                    new Guess("The Hound", 20),
                    new Guess("Grey Worm", 10),
                }
            },        
            {
                "Mike C.", new List<Guess> {
                    new Guess("Theon", 97),
                    new Guess("Littlefinger", 1),
                    new Guess("Sansa", 1),
                    new Guess("Brienne", 1),
                }
            },        
            {
                "John D.", new List<Guess> {
                    new Guess("Theon", 35),
                    new Guess("Littlefinger", 35),
                    new Guess("Cersei", 15),
                    new Guess("Melisandre", 15),
                }
            },        
            {
                "Steve T.", new List<Guess> {
                    new Guess("Dragon", 30),
                    new Guess("Daenerys", 30),
                    new Guess("Tormund", 20),
                    new Guess("Varys", 20),
                }
            },  
            {
                "Kevin T.", new List<Guess> {
                    new Guess("Littlefinger", 40),
                    new Guess("Cersei", 30),
                    new Guess("Daenerys", 15),
                    new Guess("Dragon", 15),
                }
            },  
        };
        #endregion

        public static long LongPow(long x, ulong pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        static long NumberOfSetBits(long i)
        {
            i = i - ((i >> 1) & 0x5555555555555555);
            i = (i & 0x3333333333333333) + ((i >> 2) & 0x3333333333333333);
            return (((i + (i >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56;

        }
        static void Main(string[] args)
        {
            var comboCount = LongPow(2, (ulong)Characters.Count);

            var winnerByScenario = new List<Tuple<long, string>>();
            for (long i = 0; i < comboCount; i++)
            {
                if (NumberOfSetBits(i) > 1)
                    continue;
                var deadChars = new List<string>();
                for (int j = 0; j < Characters.Count; j++)
                {
                    var dead = (i >> j) & 0x1L;
                    if (dead == 1)
                        deadChars.Add(Characters[j]);
                }

                var scoreByPlayer = new Dictionary<string, int>();
                Players.ToList().ForEach(kvp =>
                {
                    var playerName = kvp.Key;
                    var guessList = kvp.Value;
                    int score = 0;
                    foreach (var guess in guessList)
                    {
                        if (deadChars.Contains(guess.Character))
                            score += guess.Points;
                        else if (KnownDeadChars.Contains(guess.Character))
                            score += guess.Points;
                    }
                    scoreByPlayer.Add(playerName, score);
                });

                var maxScore = scoreByPlayer.Max(e => e.Value);
                if (maxScore == 0)
                    continue;   // no points, no winner
                var winners = scoreByPlayer.Where(e => e.Value == maxScore);

                foreach (var winner in winners)
                {
                    var deadChar  = deadChars.Count > 0 ? deadChars[0] : "nobody";
                    Console.WriteLine("{0} wins if {1} dies", winner.Key, deadChar);
                    //if (winner.Key == "Steve T.")
                    //{
                    //    if (!deadChars.Contains("Theon") ||
                    //        !deadChars.Contains("Littlefinger") ||
                    //        !deadChars.Contains("Cersei") ||
                    //        !deadChars.Contains("Melisandre"))
                    //        Debugger.Break();
                    //}
                    winnerByScenario.Add(new Tuple<long, string>(i, winner.Key));
                }
            }

            var groups = winnerByScenario.GroupBy(e => e.Item2).ToList();
            foreach (var group in groups)
            {
                Console.WriteLine("{0}: {1}", group.Key, group.Count());
            }
            Console.ReadLine();
        }
    }
}
