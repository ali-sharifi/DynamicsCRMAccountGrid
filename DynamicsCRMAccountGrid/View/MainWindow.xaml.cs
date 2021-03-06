﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Controls;
using DynamicsCRMAccountGrid.Domain;

namespace DynamicsCRMAccountGrid
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        // Member variables
        private int m_OriginalIndex;
        private DataGridRow m_OldRow;
        private Account m_TargetItem;
        private MainWindowViewModel m_ViewModel;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Static Methods

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            var parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the grid as a drag progresses
        /// </summary>
        private void OnMainGridCheckDropTarget(object sender, DragEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);


            // Set the DragDropEffects 
            if ((row == null) || !(row.Item is Account))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                var currentIndex = row.GetIndex();
                
                // Erase old drop-line
                if (m_OldRow != null) m_OldRow.BorderThickness = new Thickness(0);

                // Draw new drop-line
                var direction = (currentIndex - m_OriginalIndex);
                if (direction < 0) row.BorderThickness = new Thickness(0, 2, 0, 0);
                else if (direction > 0) row.BorderThickness = new Thickness(0, 0, 0, 2);

                // Reset old row
                m_OldRow = row;
            }
        }

        /// <summary>
        /// Gets the view model from the data Context and assigns it to a member variable.
        /// </summary>
        private void OnMainGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = (MainWindowViewModel)this.DataContext;
        }

        /// <summary>
        /// Process a row drop on the DataGrid.
        /// </summary>
        private void OnMainGridDrop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            // Verify that this is a valid drop and then store the drop target
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                m_TargetItem = row.Item as Account;
                if (m_TargetItem != null)
                {
                    e.Effects = DragDropEffects.Move;
                }
            }

            // Erase last drop-line
            if (m_OldRow != null) m_OldRow.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        /// <summary>
        /// Processes a drag in the main grid.
        /// </summary>
        private void OnMainGridMouseMove(object sender, MouseEventArgs e)
        {
            // Exit if shift key and left mouse button aren't pressed
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (Keyboard.Modifiers != ModifierKeys.Shift) return;

            /* We use the m_MouseDirection value in the 
             * OnMainGridCheckDropTarget() event handler. */

            // Find the row the mouse button was pressed on
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as FrameworkElement);
            m_OriginalIndex = row.GetIndex();

            // If the row was already selected, begin drag
            if ((row != null) && row.IsSelected)
            {
                // Get the account item represented by the selected row
                var selectedItem = (Account) row.Item;
                var finalDropEffect = DragDrop.DoDragDrop(row, selectedItem, DragDropEffects.Move);
                if ((finalDropEffect == DragDropEffects.Move) && (m_TargetItem != null))
                {
                    /* A drop was accepted. Determine the index of the item being 
                     * dragged and the drop location. If they are different, then 
                     * move the selectedItem to the new location. */

                    // Move the dragged item to its drop position
                    var oldIndex = m_ViewModel.AccountList.IndexOf(selectedItem);
                    var newIndex = m_ViewModel.AccountList.IndexOf(m_TargetItem);
                    if (oldIndex != newIndex) m_ViewModel.AccountList.Move(oldIndex, newIndex);
                    m_TargetItem = null;
                }
            }
        }

#endregion

    }
}