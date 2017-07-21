using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WordBrain.Data.Models;
using WordBrain.Data.Services;

namespace WordBrain.Controllers
{
    public class WordsController : Controller
    { 
        public ActionResult Index(int gridHeight = 3, int gridWidth = 3) 
        {
            var model = new GridModel(gridHeight, gridWidth){WordLengths = new List<int>{3}};
            if (Request.HttpMethod == "POST")
            {
                model.WordLengths = new List<int>{Convert.ToInt32(Request["wordLength1"])};
                for (var i = 0; i < gridHeight; i++)
                {
                    var currentRow = model.Rows[i];
                    for (var n = 0; n < gridWidth; n++)
                    {
                        currentRow[n] = new CellModel(model, i, n) {Value = Request.Form[$"col{i}_{n}"].ToLower()};
                    }
                }
                var service = new WordsService();
                var combos = service.GetAllCandidateWords(model, 0);
                model.ValidWords.Add(combos.Children.ToList());

            }
            return View(model);
        }
        
    }
}