namespace RoDSStar
{
    internal class Solution
    {
        public Result Result { get; set; }

        public int[] Order { get; set; }

        public override string ToString()
        {
            return $"{Result}; Order: {string.Join(" ", Order)}";
        }
    }
}
