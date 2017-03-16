using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration
namespace WordBrain.Data.Services
{
    public class PermuteService
    {

        public IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                var startingElementIndex = 0;
                foreach (var startingElement in list)
                {
                    var remainingItems = AllExcept(list, startingElementIndex);

                    foreach (var permutationOfRemainder in Permute(remainingItems, count - 1))
                    {
                        yield return Concat(
                            new[] {startingElement},
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }


        public IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (var item in a) yield return item;
            foreach (var item in b) yield return item;
        }
        
        public IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
        {
            var index = 0;
            foreach (var item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }
    }
}