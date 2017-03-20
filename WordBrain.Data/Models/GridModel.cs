using System.Collections.Generic;
using System.Linq;

namespace WordBrain.Data.Models
{
    public class GridModel
    {
        public int GridHeight { get; set; }
        public int GridWidth { get; set; }
        public string HideWords { get; set; }

        public List<string> HideWordsList => new List<string>(HideWords.Split(','));

        public List<int> WordLengths { get; set; }
        public List<RowModel> Rows { get; set; }

        public List<ColumnModel> Columns
        {
            get
            {
                var columns = new List<ColumnModel>();
                for (var i = 0; i < GridWidth; i++)
                {
                    var column = new ColumnModel(this, i);
                    for (var x = 0; x < GridHeight; x++)
                    {
                        column.Add(this[x, i]);
                    }
                    columns.Add(column);
                }
                return columns;
            }
        }

        public List<string> AllLetters
        {
            get
            {
                var allLetters = new List<string>();
                foreach (var row in Rows)
                {
                    allLetters.AddRange(row.Select(col => col.Used ? null : col.Value));
                }
                return allLetters;
            }
        }

        public List<string> CharacterCombos { get; set; }

        public List<List<string>> ValidWords { get; set; }

        public CellModel this[int rowIndex, int columnIndex]
        {
            get { return Rows[rowIndex][columnIndex]; }
            set { Rows[rowIndex][columnIndex] = value; }
        }

        public GridModel()
        {
            Rows = new List<RowModel>();
            ValidWords = new List<List<string>>();
            WordLengths = new List<int>();
        }

        public GridModel(int gridHeight, int gridWidth):this()
        {
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            for (var i = 0; i < gridHeight; i++)
            {
                Rows.Add(new RowModel(this, i));
            }
        }
    }

}