using System;
using System.Collections.Generic;
using System.Linq;

namespace WordBrain.Data.Models
{
    public class LettersModel
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

        public LettersModel()
        {
            Rows = new List<RowModel>();
            ValidWords = new List<List<string>>();
            WordLengths = new List<int>();
        }

        public LettersModel(int gridHeight, int gridWidth):this()
        {
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            for (var i = 0; i < gridHeight; i++)
            {
                Rows.Add(new RowModel(this, i));
            }
        }
    }

    public class RowModel : List<CellModel>
    {
        public int RowIndex { get; set; }
        public int CharCount { get; set; }
        public LettersModel Parent { get; set; }

        public RowModel(LettersModel parent, int rowIndex)
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

    public class ColumnModel : List<CellModel>
    {
        public int ColumnIndex { get; set; }
        public int CharCount { get; set; }
        public LettersModel Parent { get; set; }

        public ColumnModel(LettersModel parent, int columnIndex)
        {
            ColumnIndex = columnIndex;
            CharCount = parent.GridHeight;
            foreach (var row in parent.Rows)
            {
                Add(row[columnIndex]);
            }
        }
    }

    public class CellModel : IEquatable<CellModel>
    {
        public LettersModel Parent { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Value { get; set; }
        public bool HasForward => ColumnIndex < Parent.Columns.Count - 1;
        public bool HasBelowForward => HasBelow && HasForward;
        public bool HasBelow => RowIndex < Parent.Rows.Count - 1;
        public bool HasBelowAft => HasBelow && HasAft;
        public bool HasAft => ColumnIndex > 0;
        public bool HasAboveAft => HasAbove && HasAft;
        public bool HasAbove => RowIndex > 0;
        public bool HasAboveForward => HasAbove && HasForward;
        public bool Used { get; set; }

        public CellModel Forward => HasForward ? Parent[RowIndex, ColumnIndex + 1] : null;
        public CellModel BelowForward => HasBelow && HasForward ? Parent[RowIndex + 1, ColumnIndex + 1] : null;
        public CellModel Below => HasBelow ? Parent[RowIndex + 1, ColumnIndex] : null;
        public CellModel BelowAft => HasBelow && HasAft ? Parent[RowIndex + 1, ColumnIndex - 1] : null;
        public CellModel Aft => HasAft ? Parent[RowIndex, ColumnIndex - 1] : null;
        public CellModel AboveAft => HasAbove && HasAft ? Parent[RowIndex - 1, ColumnIndex - 1] : null;
        public CellModel Above => HasAbove ? Parent[RowIndex - 1, ColumnIndex] : null;
        public CellModel AboveForward => HasAbove && HasForward ? Parent[RowIndex - 1, ColumnIndex + 1] : null;

        public CellModel GetNextCell(List<CellModel> blackouts)
        {
            if (HasForward && !blackouts.Contains(Forward))
            {
                return Forward;
            }
            if (HasBelowForward && !blackouts.Contains(BelowForward))
            {
                return BelowForward;
            }
            if (HasBelow && !blackouts.Contains(Below))
            {
                return Below;
            }
            if (HasBelowAft && !blackouts.Contains(BelowAft))
            {
                return BelowAft;
            }
            if (HasAft && !blackouts.Contains(Aft))
            {
                return Aft;
            }
            if (HasAboveAft && !blackouts.Contains(AboveAft))
            {
                return AboveAft;
            }
            if (HasAbove && !blackouts.Contains(Above))
            {
                return Above;
            }
            if (HasAboveForward && !blackouts.Contains(AboveForward))
            {
                return AboveForward;
            }
            return null;
        }

        public CellModel(LettersModel parent, int rowIndex, int columnIndex)
        {
            Parent = parent;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Value = GetLetter().ToString();
        }

        public List<KeyValuePair<string, List<CellModel>>> GetCombos(List<CellModel> blackouts)
        {
            var combos = new List<KeyValuePair<string, List<CellModel>>>();
            if (HasForward)
            {
                if (HasAbove && !blackouts.Contains(Parent[RowIndex - 1, ColumnIndex + 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex - 1, ColumnIndex + 1].Value}", new List<CellModel>{this, Parent[RowIndex - 1, ColumnIndex + 1] }));
                }
                if(!blackouts.Contains(Parent[RowIndex, ColumnIndex + 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex, ColumnIndex + 1].Value}", new List<CellModel> { this, Parent[RowIndex, ColumnIndex + 1] }));
                }
                if (HasBelow && !blackouts.Contains(Parent[RowIndex + 1, ColumnIndex + 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex + 1, ColumnIndex + 1].Value}", new List<CellModel> { this, Parent[RowIndex + 1, ColumnIndex + 1] }));
                }
            }
            if (HasBelow && !blackouts.Contains(Parent[RowIndex + 1, ColumnIndex]))
            {
                combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex + 1, ColumnIndex].Value}", new List<CellModel> { this, Parent[RowIndex + 1, ColumnIndex] }));
            }
            if (HasAft)
            {
                if (HasAbove && !blackouts.Contains(Parent[RowIndex - 1, ColumnIndex - 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex - 1, ColumnIndex - 1].Value}", new List<CellModel> { this, Parent[RowIndex - 1, ColumnIndex - 1] }));
                }
                if (!blackouts.Contains(Parent[RowIndex, ColumnIndex - 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex, ColumnIndex - 1].Value}", new List<CellModel> { this, Parent[RowIndex, ColumnIndex - 1] }));
                }
                if (HasBelow && !blackouts.Contains(Parent[RowIndex + 1, ColumnIndex-1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex + 1, ColumnIndex - 1].Value}", new List<CellModel> { this, Parent[RowIndex + 1, ColumnIndex - 1] }));
                }
            }
            if (HasAbove && !blackouts.Contains(Parent[RowIndex - 1, ColumnIndex]))
            {
                combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex - 1, ColumnIndex].Value}", new List<CellModel> { this, Parent[RowIndex - 1, ColumnIndex] }));
            }
            return combos;
        }

        public bool Equals(CellModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CellModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RowIndex * 397) ^ ColumnIndex;
            }
        }


        private static readonly Random Random = new Random();
        public static char GetLetter()
        {
            var num = Random.Next(0, 26); 
            var let = (char)('a' + num);
            return let;
        }
    }

    public class ListCandidate : IEquatable<ListCandidate>
    {
        public string Candidate { get; set; }
        public HashSet<ListCandidate> List { get; set; }
        public List<CellModel> Cells { get; set; }

        public HashSet<string> Children
        {
            get { return new HashSet<string>(List.Select(l => l.Candidate)); }
        }

        public bool Equals(ListCandidate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Candidate, other.Candidate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ListCandidate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Candidate?.GetHashCode() ?? 0) * 397;
            }
        }
    }

}