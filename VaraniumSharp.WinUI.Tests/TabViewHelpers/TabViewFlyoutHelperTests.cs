using Moq;
using System.Threading.Tasks;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.TabViewHelpers;
using Xunit;

namespace VaraniumSharp.WinUI.Tests.TabViewHelpers
{
    public class TabViewFlyoutHelperTests
    {
        [Fact]
        public async Task CreatingFlyoutReturnsAnInstance()
        {
            // Won't run because of packaging requirements
        }

        private class TabViewFlyoutHelperFixture
        {
            public Mock<IDialogs> DialogsMock { get; } = new Mock<IDialogs>();

            public IDialogs Dialogs => DialogsMock.Object;

            public TabViewItemFlyoutHelper GetInstance()
            {
                return new TabViewItemFlyoutHelper(Dialogs);
            }
        }
    }
}
