using System;
using System.Linq;
using System.Web.Mvc;
using WordBrain.Data;
using WordBrain.Data.Models;
using WordBrain.Data.Services;

namespace WordBrain.Controllers
{
    public class WordsController : Controller
    { 
        public ActionResult Index(int gridHeight = 3, int gridWidth = 3) 
        {
            var model = new LettersModel(gridHeight, gridWidth);
            if (Request.HttpMethod == "POST")
            {
                model.WordLengths[0] = Convert.ToInt32(Request["wordLength"]);
                for (var i = 0; i < gridHeight; i++)
                {
                    var currentRow = model.Rows[i];
                    for (var n = 0; n < gridWidth; n++)
                    {
                        currentRow[n] = new CellModel(model, i, n) {Value = Request.Form[$"col{i}_{n}"].ToLower()};
                    }
                }
                var service = new WordsService();
                var combos = service.GetAllCharacterCombos(model);
                model.ValidWords = combos.Children.ToList();

            }
            return View(model);
        }

        
    }
}