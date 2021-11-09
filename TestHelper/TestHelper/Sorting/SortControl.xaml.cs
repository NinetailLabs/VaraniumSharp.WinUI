using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.SortModule;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestHelper.Sorting
{
    [AutomaticContainerRegistration(typeof(SortControl))]
    [DisplayComponent("Sort Control", "84dc893c-f7f0-4e74-9922-b059c0d234b7", "Sorting", 100, 100, typeof(SortControl))]
    public sealed partial class SortControl: IDisplayComponent
    {
        #region Constructor

        public SortControl()
        {
            InitializeComponent();
            Entries = new();
            CollectionView = new ExtendedAdvancedCollectionView(Entries, true);
            SortablePropertyModule = new SortablePropertyModule(CollectionView);
            SortablePropertyModule.GenerateSortEntries(typeof(SortableEntry));
            SortablePropertyModule.RemoveSortEntry("AccidentalSort");
            SortablePropertyModule.RequestSort("Title");
            SetupCollection();
        }

        #endregion

        #region Events


        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        public ExtendedAdvancedCollectionView CollectionView { get; }

        public Guid ContentId => Guid.Parse("84dc893c-f7f0-4e74-9922-b059c0d234b7");

        private ObservableCollection<SortableEntry> Entries { get; }
        public bool ShowResizeHandle { get; set; }

        public SortablePropertyModule SortablePropertyModule { get; }
        public bool StartupLoad { get; set; }
        public string Title { get; set; } = "Sort Control";

        #endregion

        #region Public Methods

        public Task InitAsync()
        {
            // Not used for now
            return Task.CompletedTask;
        }

        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private void SetupCollection()
        {
            Entries.Add(new SortableEntry
            {
                Id = 2,
                Title = "Q",
                Position = 2
            });

            Entries.Add(new SortableEntry
            {
                Id = 1,
                Title = "Z",
                Position = 3
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "H",
                Position = 1
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "A",
                Position = 1
            });
        }

        #endregion

        private void ListViewBase_OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // TODO - Convert to use JSON instead with a standard drag entity thing
            var sb = new StringBuilder();
            foreach (var item in e.Items)
            {
                var sortItem = item as SortOrderEntry;
                sb.AppendLine(sortItem.PropertyName);
            }
            
            e.Data.SetText(sb.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {
            // TODO - Should check the type in the data package to decide what to do
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private async void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                // We need to take a Deferral as we won't be able to confirm the end
                // of the operation synchronously
                var def = e.GetDeferral();
                var s = await e.DataView.GetTextAsync();
                var items = s.Split("\r\n");

                // TODO - Figure out how to get the insert index here
                //IInsertionPanel.GetInsertionIndexes(point, out var firstIndex, out var secondIndex);
                var target = (GridView)sender;
                var pos = e.GetPosition(target.ItemsPanelRoot);
                var sampleItem = (GridViewItem)target.ContainerFromIndex(0);
                var itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;
                var index = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

                var targetItem = (GridViewItem)target.ContainerFromIndex(index);

                // Figure out if to insert above or below
                var positionInItem = e.GetPosition(targetItem);
                if (positionInItem.Y > itemHeight / 2)
                {
                    index++;
                }

                // Don't go out of bounds
                index = Math.Min(target.Items.Count, index);

                foreach (var item in items)
                {
                    var entryToMove = SortablePropertyModule.AvailableSortEntries.FirstOrDefault(x => x.PropertyName == item);
                    if (entryToMove != null)
                    {
                        SortablePropertyModule.AvailableSortEntries.Remove(entryToMove);
                        //SortablePropertyModule.EntriesSortedBy.Add(entryToMove);
                        SortablePropertyModule.EntriesSortedBy.Insert(index, entryToMove);
                        index++;
                    }
                }
                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }
    }
}
