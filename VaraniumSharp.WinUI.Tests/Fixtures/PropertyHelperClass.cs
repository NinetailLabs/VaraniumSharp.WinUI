namespace VaraniumSharp.WinUI.Tests.Fixtures
{
    public class PropertyHelperClass
    {
        #region Properties

        public NestedClass NestedProperty { get; set; }

        public string TopProperty { get; set; }

        #endregion
    }

    public class NestedClass
    {
        #region Properties

        public string MyProperty { get; set; }

        #endregion
    }
}