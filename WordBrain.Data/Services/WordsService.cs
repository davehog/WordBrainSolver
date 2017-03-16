using System.Collections.Generic;
using System.Linq;
using WordBrain.Data.Models;
// ReSharper disable PossibleMultipleEnumeration

namespace WordBrain.Data.Services
{
    public class WordsService
    {
        private readonly PermuteService permuteService;

        public WordsService()
        {
            permuteService = new PermuteService();
        }

        private HashSet<string> allWords;
        public HashSet<string> AllWords
        {
            get
            {
                if (allWords == null)
                {
                    using (var context = new wordsEntities())
                    {
                        allWords = new HashSet<string>(context.WordLists.Select(w => w.Word).ToList());
                    }
                }
                return allWords;
            }
        }

        public HashSet<string> GetValidWords(HashSet<string> checkWords)
        {
            checkWords.IntersectWith(allWords);
            return checkWords;
        }

        public ListCandidate GetAllCharacterCombos(LettersModel letters)
        {
            var words = new HashSet<ListCandidate>();
            foreach (var row in letters.Rows)
            {
                foreach (var cell in row)
                {
                    var combos = GetCharacterCombos(letters, cell);
                    foreach (var combo in combos)
                    {
                        words.Add(combo);
                    }
                }
            }
            return new ListCandidate {List = words};
        }

        public List<ListCandidate> GetCharacterCombos(LettersModel letters, CellModel startCell)
        {
            var doneList = new Dictionary<int, HashSet<CellModel>>();
            var currentCell = startCell;
            var words = new List<ListCandidate>();
            var currentList = new List<CellModel> { currentCell };
            var completedCells = new List<CellModel>();

            var currentString = currentCell.Value;
            for (var i = 1; i < letters.WordLengths[0] - 1; i++)
            {
                if (!doneList.ContainsKey(i))
                {
                    doneList.Add(i, new HashSet<CellModel>());
                }
                var nextCell = currentCell.GetNextCell(currentList.Concat(completedCells).ToList());
                if (string.IsNullOrEmpty(nextCell?.Value))
                {
                    var done = false;
                    var last = currentList.Last();
                    currentList.Remove(last);
                    completedCells = new List<CellModel>();
                    doneList[i].Add(last);
                    if (currentList.Count == 0)
                    {
                        continue;
                    }
                    currentCell = currentList.Last().GetNextCell(doneList[i].Concat(currentList).ToList());
                    var n = i;
                    while (currentCell == null)
                    {
                        n--;
                        doneList[n].Add(currentList.Last());
                        for (var x = n + 1; x <= doneList.Count; x++)
                        {
                            doneList[x] = new HashSet<CellModel>();
                        }
                        currentList.Remove(currentList.Last());
                        if (!currentList.Any())
                        {
                            done = true;
                            break;
                        }
                        currentCell = currentList.Last().GetNextCell(doneList[n].Concat(currentList).ToList());
                    }
                    if (done) break;
                    currentString = string.Concat(currentList.Select(c => c.Value));
                    i = currentString.Length;
                    nextCell = currentCell;
                }
                if (i == letters.WordLengths[0] - 2)
                {
                    foreach (var combo in nextCell.GetCombos(currentList))
                    {
                        var candidate = $"{currentString}{combo}";
                        if (IsValidWord(candidate))
                        {
                            words.Add(new ListCandidate { Candidate = candidate });
                        }
                    }
                    i = currentString.Length - 1;
                    completedCells.Add(nextCell);
                    currentCell = currentList.Last();
                }
                else
                {
                    currentString += nextCell.Value;
                    currentList.Add(nextCell);
                    currentCell = nextCell;
                }
            }


            return words;
        }

        public HashSet<ListCandidate> GetPermutations(List<string> remainingLetters, List<int> sizes)
        {
            var permutations = new HashSet<ListCandidate>();
            var allPermutations = permuteService.Permute(remainingLetters, sizes[0]);
            foreach (var permutation in allPermutations)
            {
                var word = string.Join(null, permutation);
                if (IsValidWord(word))
                {
                    var newRemainingLetters = new List<string>(remainingLetters);
                    foreach (var l in permutation)
                    {
                        newRemainingLetters.Remove(l);
                    }
                    var candidate = new ListCandidate {Candidate = string.Join(null, permutation)};
                    if (sizes.Count > 1 && newRemainingLetters.Any())
                    {
                        var newSizes = new List<int>(sizes);
                        newSizes.RemoveAt(0);
                        var perms = GetPermutations(newRemainingLetters, newSizes);
                        if (perms.Any())
                        {
                            permutations.Add(candidate);
                        }
                        permutations.Add(candidate);
                    }
                }
            }
            return permutations;
        }

        public bool IsValidWord(string word)
        {
            return AllWords.Contains(word);
        }
        
    }

}
