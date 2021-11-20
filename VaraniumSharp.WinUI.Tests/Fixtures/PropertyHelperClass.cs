namespace VaraniumSharp.WinUI.Tests.Fixtures
{
    public class PropertyHelperClass
    {
        #region Constructor

        public PropertyHelperClass()
        {
            NestedProperty = new();
            TopProperty = "I'm on top";
        }

        #endregion

        #region Properties

        public NestedClass NestedProperty { get; set; }

        public string TopProperty { get; set; }

        #endregion
    }

    public class NestedClass
    {
        #region Constructor

        public NestedClass()
        {
            MyProperty = "It's mine";
        }

        #endregion

        #region Properties

        public string MyProperty { get; set; }

        #endregion
    }
}