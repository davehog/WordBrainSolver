using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordBrain.Data;
using WordBrain.Data.Models;
using WordBrain.Data.Services;

namespace WordBrain.Tests
{
    [TestClass]
    public class LettersTest
    {
        [TestMethod]
        public void TestLetters()
        {
            var letters = new LettersModel(4,4);
            letters[0, 0] = new CellModel(letters, 0, 0) { Value = "t" };
            letters[0, 1] = new CellModel(letters, 0, 1) { Value = "h" };
            letters[0, 2] = new CellModel(letters, 0, 2) { Value = "i" };
            letters[0, 3] = new CellModel(letters, 0, 3) { Value = "s" };
            letters[1, 0] = new CellModel(letters, 1, 0) { Value = "c" };
            letters[1, 1] = new CellModel(letters, 1, 1) { Value = "a" };
            letters[1, 2] = new CellModel(letters, 1, 2) { Value = "n" };
            letters[1, 3] = new CellModel(letters, 1, 3) { Value = "h" };
            letters[2, 0] = new CellModel(letters, 2, 0) { Value = "e" };
            letters[2, 1] = new CellModel(letters, 2, 1) { Value = "p" };
            letters[2, 2] = new CellModel(letters, 2, 2) { Value = "l" };
            letters[2, 3] = new CellModel(letters, 2, 3) { Value = "e" };
            letters[3, 0] = new CellModel(letters, 3, 0) { Value = "d" };
            letters[3, 1] = new CellModel(letters, 3, 1) { Value = "o" };
            letters[3, 2] = new CellModel(letters, 3, 2) { Value = "u" };
            letters[3, 3] = new CellModel(letters, 3, 3) { Value = "r" };

            var service = new WordsService();
            var combos = service.GetAllCharacterCombos(letters);

            Assert.IsNotNull(combos);
        }

        [TestMethod]
        public void TestPermute()
        {
            var stringInput = new List<string>{ "d", "f", "e", "r", "x", "a", "o", "a", "t", "r" };
            var service = new WordsService();
            var perms = service.GetPermutations(stringInput, new List<int> {6, 4});
            Assert.IsNotNull(perms);
        }

        private static void ShowPermutations<T>(IEnumerable<T> input, int count)
        {
            var permuteService = new PermuteService();
            foreach (var permutation in permuteService.Permute(input, count))
            {
                foreach (var i in permutation)
                {
                    Console.Write(i);
                }
                Console.WriteLine();
            }
        }
    }
}
