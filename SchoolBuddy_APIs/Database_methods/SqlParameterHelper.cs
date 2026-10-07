namespace SchoolBuddy_APIs.Database_methods
{
    public static class SqlParameterHelper
    {
        ///<summary>
        /// Adds one parameter per value (@prefix0, @prefix1, ...) and returns the
        /// placeholder list to put inside IN ( ... ).
        /// An empty list returns "NULL" so the IN matches no rows.
        ///</summary>
        public static string AddInList(Dictionary<string, object> parameters, string prefix, IEnumerable<object> values)
        {
            List<string> names = new List<string>();
            int i = 0;

            foreach (var value in values)
            {
                string name = $"@{prefix}{i++}";
                parameters[name] = value;
                names.Add(name);
            }

            return names.Count > 0 ? string.Join(", ", names) : "NULL";
        }

        ///<summary>
        /// Same as AddInList, for a comma separated string like "1,2,3".
        ///</summary>
        public static string AddInList(Dictionary<string, object> parameters, string prefix, string? commaSeparated)
        {
            var values = (commaSeparated ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return AddInList(parameters, prefix, values);
        }
    }
}
