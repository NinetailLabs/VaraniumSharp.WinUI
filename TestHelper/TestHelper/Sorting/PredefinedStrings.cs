using System.Collections.Generic;

namespace TestHelper.Sorting
{
    public static class PredefinedStrings
    {
        #region Constructor

        static PredefinedStrings()
        {
            PredefinedStringsCollection = new()
            {
                "Value 1",
                "Value 2",
                "Value 3"
            };
        }

        #endregion

        #region Properties

        public static List<string> PredefinedStringsCollection { get; }

        #endregion
    }
}