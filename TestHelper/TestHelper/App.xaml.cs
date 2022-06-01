using Microsoft.UI.Xaml;
using System;
using System.Reflection;
using VaraniumSharp.DryIoc;
using VaraniumSharp.WinUI.TabWindow;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestHelper
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            AppDomain.CurrentDomain.Load(new AssemblyName("VaraniumSharp.WinUI"));
            var containerSetup = new ContainerSetup();
            containerSetup.RetrieveClassesRequiringRegistration(true);
            containerSetup.RetrieveConcretionClassesRequiringRegistration(true);

            var tabWindow = containerSetup.Resolve<TabWindow>();
            tabWindow.MinWidth = 750;
            m_window = tabWindow;
            m_window.Activate();
        }

        private Window m_window;
    }
}
