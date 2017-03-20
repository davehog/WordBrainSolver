using System.Collections.Generic;

namespace WordBrain.Data.Models
{
    public class RowModel : List<CellModel>
    {
        public int RowIndex { get; set; }
        public int CharCount { get; set; }
        public GridModel Parent { get; set; }

        public RowModel(GridModel parent, int rowIndex)
        {
            Parent = parent;
            RowIndex = rowIndex;
            CharCount = parent.GridWidth;
            for (var i = 0; i < parent.GridWidth; i++)
            {
                Add(new CellModel(Parent, rowIndex, i));
            }
        }
    }
}
