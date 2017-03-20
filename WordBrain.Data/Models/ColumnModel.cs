using System.Collections.Generic;

namespace WordBrain.Data.Models
{
    public class ColumnModel : List<CellModel>
    {
        public int ColumnIndex { get; set; }
        public int CharCount { get; set; }
        public GridModel Parent { get; set; }

        public ColumnModel(GridModel parent, int columnIndex)
        {
            ColumnIndex = columnIndex;
            CharCount = parent.GridHeight;
            foreach (var row in parent.Rows)
            {
                Add(row[columnIndex]);
            }
        }
    }
}
