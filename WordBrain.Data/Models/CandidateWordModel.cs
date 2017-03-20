using System;
using System.Collections.Generic;
using System.Linq;

namespace WordBrain.Data.Models
{
    public class CandidateWordModel : IEquatable<CandidateWordModel>
    {
        public string Candidate { get; set; }
        public HashSet<CandidateWordModel> List { get; set; }
        public List<CellModel> Cells { get; set; }

        public HashSet<string> Children
        {
            get { return new HashSet<string>(List.Select(l => l.Candidate)); }
        }

        public bool Equals(CandidateWordModel other)
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
            return Equals((CandidateWordModel)obj);
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
