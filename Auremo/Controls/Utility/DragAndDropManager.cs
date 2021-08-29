using Auremo.Controls.CustomWidgets;
using Auremo.DataModel.AudioObjects;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls.Utility
{
    public class DragAndDropManager
    {
        // The position is the index of the slot being dropped to, but before
        // the selection has been removed from before it, so the end of the
        // list is always m_ItemList.Count (which is not a valid index).
        public delegate void OnDropCallback(int position);

        private readonly ListBoxWithDragDrop m_ListBox = null;
        private readonly IList<PlaylistItem> m_ItemsSource = null;

        OnDropCallback m_Callback = null;

        private bool m_DragStarting = false;
        private Point m_DragStartPoint = new Point();
        PlaylistItem m_DragSourceItem = null;
        int m_DropPosition = -1;
        PlaylistItem m_DropPositionReferenceItem = null;

        public DragAndDropManager(ListBoxWithDragDrop listBox, IList<PlaylistItem> itemsSource, OnDropCallback callback)
        {
            m_ListBox = listBox;
            m_ItemsSource = itemsSource;
            m_Callback = callback;
        }

        public void OnPreviewLeftMouseDown(MouseButtonEventArgs e)
        {
            // Don't try to interpret this is a starting drag gesture is the user is clicking a button.
            if (WidgetUtility.FindAncestor<Button>(Mouse.DirectlyOver as FrameworkElement) == null)
            {
                if (e.ClickCount == 1 && Keyboard.Modifiers == ModifierKeys.None)
                {
                    EndDrag(); // Probably redundant, but whatever

                    if (e.OriginalSource is FrameworkElement elem)
                    {
                        PlaylistItem item = WidgetUtility.FindDataObject<PlaylistItem>(elem);

                        if (item != null)
                        {
                            if (!item.IsSelected)
                            {
                                // Fast drag mode for slick users: just grab without selecting
                                // first. Drags only this item.
                                m_ListBox.UnselectAll();
                                item.IsSelected = true;
                            }

                            m_DragStarting = true;
                            m_DragStartPoint = e.GetPosition(m_ListBox);
                            m_DragSourceItem = item;
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (m_DragStarting && m_DragSourceItem.IsSelected)
            {
                Point currentPoint = e.GetPosition(m_ListBox);
                double xDelta = currentPoint.X - m_DragStartPoint.X,
                       yDelta = currentPoint.Y - m_DragStartPoint.Y,
                       distanceSqr = xDelta * xDelta + yDelta * yDelta;

                if (distanceSqr > 9.0)  // Start drag.
                {
                    m_DragStarting = false;
                    m_ListBox.IsDragActive = true;
                    m_DragStartPoint = new Point();
                    m_ListBox.IsDragActive = true;
                }
            }
            else if (m_ListBox.IsDragActive) // Continue drag.
            {
                if (e.OriginalSource is FrameworkElement elem)
                {
                    PlaylistItem item = WidgetUtility.FindDataObject<PlaylistItem>(elem);
                    DependencyObject container = m_ListBox.ItemContainerGenerator.ContainerFromItem(item);

                    if (container != null && container is ListBoxItem row)
                    {
                        int rowIndex = item.Pos;
                        double offsetFromTop = e.GetPosition(row).Y;

                        if (offsetFromTop < 0.5 * row.ActualHeight)
                        {
                            m_DropPosition = rowIndex;

                            // The mouse is closer to top edge than the bottom one, so the
                            // drop position is above this item.
                            if (rowIndex == 0)
                            {
                                // Special case: there is no item above, so set the top
                                // indicator line, which is only visible for row 0.
                                SetDropPosition(item, true);
                            }
                            else
                            {
                                // Set the indicator line below the item above this one.
                                SetDropPosition(m_ItemsSource[rowIndex - 1]);
                            }
                        }
                        else
                        {
                            // Easiest case: the mouse is closer to the bottom edge of
                            // the row than to the top one, so the drop position is
                            // simply below the item.
                            m_DropPosition = rowIndex + 1;
                            SetDropPosition(item);
                        }
                    }
                    else
                    {
                        EndDrag();
                    }
                }
            }
        }

        public void OnPreviewLeftMouseUp()
        {
            if (m_ListBox.IsDragActive)
            {
                m_Callback?.Invoke(m_DropPosition);
            }

            EndDrag();
        }

        public void OnMouseLeave()
        {
            EndDrag();
        }

        public void EndDrag()
        {
            m_DragStarting = false;
            m_DragStartPoint = new Point();
            m_ListBox.IsDragActive = false;
            m_DragSourceItem = null;
            SetDropPosition(null);
        }

        private void SetDropPosition(PlaylistItem referenceItem, bool dropPositionIsAbove = false)
        {
            if (m_DropPositionReferenceItem != null && referenceItem != m_DropPositionReferenceItem)
            {
                m_DropPositionReferenceItem.IsAboveDropTarget = false;
                m_DropPositionReferenceItem.IsBelowDropTarget = false;
            }

            m_DropPositionReferenceItem = referenceItem;

            if (referenceItem != null)
            {
                // TODO: the above/below stuff gets inverted here, and another time
                // in the style. Improve.
                m_DropPositionReferenceItem.IsAboveDropTarget = !dropPositionIsAbove;
                m_DropPositionReferenceItem.IsBelowDropTarget = dropPositionIsAbove;
            }
        }
    }
}
