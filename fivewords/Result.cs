namespace fivewords
{
    public struct Result
    {
        public List<Tuple<string, int>> words = new List<Tuple<string, int>>();

        private string tostr = "";

        public Result()
        {
        }

        public override string ToString()
        {
            return tostr;
        }

        public void SetWords(Tuple<string, int> w1, Tuple<string, int> w2, Tuple<string, int> w3, Tuple<string, int> w4,
            Tuple<string, int> w5)
        {
            words = new List<Tuple<string, int>> { w1, w2, w3, w4, w5 };

            words = words.OrderBy(item => item.Item1).ToList();

            tostr = String.Join('-', words.Select(item => item.Item1));
        }

    }
}
