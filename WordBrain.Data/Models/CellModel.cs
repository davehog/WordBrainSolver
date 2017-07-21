using System;
using System.Collections.Generic;

namespace WordBrain.Data.Models
{

    public class CellModel : IEquatable<CellModel>
    {
        public GridModel Parent { get; set; }
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

        /// <summary>
        /// Gets the next cell in a clockwise rotation (excluding <see cref="blackouts"/>)
        /// </summary>
        /// <param name="blackouts"></param>
        /// <returns></returns>
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

        public CellModel(GridModel parent, int rowIndex, int columnIndex)
        {
            Parent = parent;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Value = GetLetter().ToString();
        }

        /// <summary>
        /// Returns a List of KeyValuePairss of two-letter combos that exist from this cell (excluding <see cref="blackouts"/>)
        /// </summary>
        /// <param name="blackouts">Cells to omit from the return</param>
        /// <returns></returns>
        public List<KeyValuePair<string, List<CellModel>>> GetCombos(List<CellModel> blackouts)
        {
            var combos = new List<KeyValuePair<string, List<CellModel>>>();
            if (HasForward)
            {
                if (HasAbove && !blackouts.Contains(Parent[RowIndex - 1, ColumnIndex + 1]))
                {
                    combos.Add(new KeyValuePair<string, List<CellModel>>($"{Value}{Parent[RowIndex - 1, ColumnIndex + 1].Value}", new List<CellModel> { this, Parent[RowIndex - 1, ColumnIndex + 1] }));
                }
                if (!blackouts.Contains(Parent[RowIndex, ColumnIndex + 1]))
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
                if (HasBelow && !blackouts.Contains(Parent[RowIndex + 1, ColumnIndex - 1]))
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
            return Equals((CellModel)obj);
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
}
