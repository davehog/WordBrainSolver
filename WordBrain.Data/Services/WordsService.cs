using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WordBrain.Data.Models;
// ReSharper disable PossibleMultipleEnumeration

namespace WordBrain.Data.Services
{
    public class WordsService
    {

        private HashSet<string> allWords;
        public HashSet<string> AllWords
        {
            get
            {
                if (allWords == null)
                {
                    using (var context = new wordsEntities())
                    {
                        var rx = new Regex("[aeiouy]", RegexOptions.IgnoreCase);
                        allWords = new HashSet<string>(context.WordLists.ToList().Where(w => rx.IsMatch(w.Word)).Select(w => w.Word).ToList());
                    }
                }
                return allWords;
            }
        }

        public HashSet<string> GetValidWords(HashSet<string> checkWords)
        {
            checkWords.IntersectWith(AllWords);
            return checkWords;
        }

        public CandidateWordModel GetAllValidWords(GridModel grid, int wordLengthIndex)
        {
            var words = new HashSet<CandidateWordModel>();
            foreach (var row in grid.Rows)
            {
                foreach (var cell in row)
                {
                    var combos = GetValidWords(grid, cell, wordLengthIndex);
                    foreach (var combo in combos)
                    {
                        words.Add(combo);
                    }
                }
            }
            return new CandidateWordModel {List = words};
        }

        public List<GridModel> GetLettersMinusWords(GridModel grid, List<string> words)
        {
            var accumulatedLetters = words.Aggregate(string.Empty, (current, word) => current + word);
            var letterInstances = accumulatedLetters.Select(letter => FindCellsWithLetter(grid, letter, new List<CellModel>())).ToList();
            var validCombos = letterInstances.CartesianProduct();
            return validCombos.Select(validCombo => SubtractCells(grid, validCombo)).ToList();
        }

        public GridModel SubtractCells(GridModel source, IEnumerable<CellModel> removals)
        {
            var letters = new GridModel(source.GridHeight, source.GridWidth) { WordLengths = source.WordLengths };
            for (var i = 0; i < source.GridHeight; i++)
            {
                for (var x = 0; x < source.GridWidth; x++)
                {
                    if (removals.Contains(source[i, x]))
                    {
                        letters[i, x].Value = null;
                        letters[i, x].Used = true;
                    }
                    else
                    {
                        letters[i, x].Value = source[i, x].Value;
                        letters[i, x].Used = source[i, x].Used;
                    }
                }
            }
            return Tetrisify(letters);
        }

        public GridModel Tetrisify(GridModel source)
        {
            var letters = new GridModel(source.GridHeight, source.GridWidth) { WordLengths = source.WordLengths };
            for (var r = source.GridWidth-1; r >= 0; r--)
            {
                for (var x = source.GridHeight-1; x >= 0; x--)
                {
                    var i = x;
                    while (i > 0 && source[i, r] != null && source[i, r].Used)
                    {
                        i--;
                    }
                    if (source[i, r] != null && !source[i,r].Used)
                    {
                        letters[x, r].Value = source[i, r].Value;
                        source[i, r].Used = true;
                    }
                    else
                    {
                        letters[x, r].Value = "0";
                        letters[x, r].Used = true;
                    }
                }
            }
            return letters;
        }

        public List<CellModel> FindCellsWithLetter(GridModel grid, char letter, List<CellModel> skipCells)
        {
            var matchingCells = new List<CellModel>();
            foreach (var row in grid.Rows)
            {
                matchingCells.AddRange(row.Where(c => !skipCells.Contains(c) && string.CompareOrdinal(letter.ToString(), c.Value) == 0));
            }
            return matchingCells;
        }

        public List<CandidateWordModel> GetValidWords(GridModel grid, CellModel startCell, int wordIndex = 0)
        {
            var doneList = new Dictionary<int, HashSet<CellModel>>();
            var currentCell = startCell;
            var words = new List<CandidateWordModel>();
            var currentList = new List<CellModel> { currentCell };
            var completedCells = new List<CellModel>();

            var currentString = currentCell.Value;
            for (var i = 1; i < grid.WordLengths[wordIndex] - 1; i++)
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
                if (i == grid.WordLengths[wordIndex] - 2)
                {
                    foreach (var combo in nextCell.GetCombos(currentList))
                    {
                        var candidate = $"{currentString}{combo.Key}";
                        if (IsValidWord(candidate))
                        {
                            words.Add(new CandidateWordModel { Candidate = candidate, Cells = currentList.Concat(combo.Value).ToList()});
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

        public bool IsValidWord(string word)
        {
            return AllWords.Contains(word);
        }
        
    }

}
