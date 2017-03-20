using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WordBrain.Data.Models;
using WordBrain.Data.Services;

namespace WordBrain.Controllers
{
    public class CombinationsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post()
        {
            var gridHeight = Convert.ToInt32(getFormKeyValue("GridHeight"));
            var gridWidth = Convert.ToInt32(getFormKeyValue("GridWidth"));
            var wordCount = Convert.ToInt32(getFormKeyValue("wordCount"));
            var wordLengths = new List<int>();
            var words = new List<string>();
            for (var i = 1; i <= wordCount+1; i++)
            {
                if (!string.IsNullOrWhiteSpace(getFormKeyValue($"wordLength{i}")))
                {
                    wordLengths.Add(Convert.ToInt32(getFormKeyValue($"wordLength{i}")));
                }
                if (!string.IsNullOrWhiteSpace(getFormKeyValue($"word{i}")))
                {
                    words.Add(getFormKeyValue($"word{i}"));
                }
            }

            var model = new LettersModel(gridHeight, gridWidth) { WordLengths = wordLengths };
            for (var i = 0; i < gridHeight; i++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    model[i, x].Value = getFormKeyValue($"col{i}_{x}");
                }
            }
            var service = new WordsService();
            var combos = service.GetLettersMinusWords(model, words);
            var results = new HashSet<string>();
            foreach (var combo in combos)
            {
                var result = service.GetAllCharacterCombos(combo, wordLengths.Count-1);
                if (result.Children.Any())
                {
                    foreach (var word in result.Children)
                    {
                        results.Add(word);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
        public string getFormKeyValue(string key)
        {
            var value = string.Empty;
            try
            {
                var values = HttpContext.Current.Request.Form.GetValues(key);
                if (values != null && values.Length >= 1)
                    value = values[0];
            }
            catch  { /* do something with this */ }

            return value;
        }
    }
}
