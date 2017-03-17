﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            var letters = new LettersModel(3,3){WordLengths = new List<int>{3,6}};
            letters[0, 0] = new CellModel(letters, 0, 0) { Value = "t" };
            letters[0, 1] = new CellModel(letters, 0, 1) { Value = "h" };
            letters[0, 2] = new CellModel(letters, 0, 2) { Value = "e" };
            letters[1, 0] = new CellModel(letters, 1, 0) { Value = "d" };
            letters[1, 1] = new CellModel(letters, 1, 1) { Value = "e" };
            letters[1, 2] = new CellModel(letters, 1, 2) { Value = "a" };
            letters[2, 0] = new CellModel(letters, 2, 0) { Value = "s" };
            letters[2, 1] = new CellModel(letters, 2, 1) { Value = "h" };
            letters[2, 2] = new CellModel(letters, 2, 2) { Value = "t" };

            var service = new WordsService();
            var combos = service.GetLettersMinusWords(letters, new List<string>{"the"});

            var results = new HashSet<string>();
            foreach (var combo in combos)
            {
                var result = service.GetAllCharacterCombos(combo, 1);
                if (result.Children.Any())
                {
                    foreach (var word in result.Children)
                    {
                        results.Add(word);
                    }
                }
            }

            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestPermute()
        {
            var stringInput = new List<string>{ "d", "f", "e", "r", "x", "a", "o", "a", "t", "r" };
            var service = new WordsService();
            var perms = service.GetPermutations(stringInput, new List<int> {6, 4});
            Assert.IsNotNull(perms);
        }

        [TestMethod]
        public void TestWords()
        {
            var input = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("a", 1),
                new KeyValuePair<string, int>("n", 2),
                new KeyValuePair<string, int>("d", 3),
                new KeyValuePair<string, int>("k", 4),
                new KeyValuePair<string, int>("n", 5),
                new KeyValuePair<string, int>("n", 6),
                new KeyValuePair<string, int>("a", 7),
                new KeyValuePair<string, int>("a", 8),
                new KeyValuePair<string, int>("t", 9),
                new KeyValuePair<string, int>("r", 10)
            };
            var find = "and";
            var results = new List<List<KeyValuePair<string, int>>>();
            foreach (var letter in find)
            {
                var letterResult = new List<KeyValuePair<string, int>>();
                foreach (var found in input.Where(s => s.Key == letter.ToString()))
                {
                    letterResult.Add(found);
                }
                results.Add(letterResult);
            }
            Assert.IsNotNull(results);

            var super = results.CartesianProduct();
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
