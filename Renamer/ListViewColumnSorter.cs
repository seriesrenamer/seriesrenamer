#region SVN Info
/***************************************************************
 * $Author$
 * $Revision$
 * $Date$
 * $LastChangedBy$
 * $LastChangedDate$
 * $URL$
 * 
 * License: GPLv3
 * 
****************************************************************/
#endregion

using System.Collections;
using System.Windows.Forms;
using System;
using Renamer;

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int ColumnToSort;
    private int SecondColumnToSort;
    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder OrderOfSort;
    private SortOrder SecondOrderOfSort;
    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
        // Initialize the column to '0'
        ColumnToSort = 0;
        SecondColumnToSort = 0;
        // Initialize the sort order to 'none'
        OrderOfSort = SortOrder.None;
        SecondOrderOfSort = SortOrder.None;

        // Initialize the CaseInsensitiveComparer object
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
        int compareResult;
        ListViewItem listviewX, listviewY;

        // Cast the objects to be compared to ListViewItem objects
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        // Compare the two items
        if (Helper.IsNumeric(listviewX.SubItems[ColumnToSort].Text) && Helper.IsNumeric(listviewY.SubItems[ColumnToSort].Text))
        {
            compareResult = Convert.ToInt32(listviewX.SubItems[ColumnToSort].Text) - Convert.ToInt32(listviewY.SubItems[ColumnToSort].Text);
        }
        else
        {
            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
        }
        if (OrderOfSort == SortOrder.Descending) compareResult *= -1;
        if (compareResult == 0)
        {
            if (Helper.IsNumeric(listviewX.SubItems[SecondColumnToSort].Text) && Helper.IsNumeric(listviewY.SubItems[SecondColumnToSort].Text))
            {
                compareResult = Convert.ToInt32(listviewX.SubItems[SecondColumnToSort].Text) - Convert.ToInt32(listviewY.SubItems[SecondColumnToSort].Text);
            }
            else
            {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[SecondColumnToSort].Text, listviewY.SubItems[SecondColumnToSort].Text);
            }
            
            if (SecondOrderOfSort == SortOrder.Descending) compareResult *= -1;
        }
        // Calculate correct return value based on object comparison
        if (OrderOfSort == SortOrder.None)
        {
            // Return '0' to indicate they are equal
            return 0;
        }
        else 
        {            
            return compareResult;
        }
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn
    {
        set
        {
            SecondColumnToSort = ColumnToSort;
            SecondOrderOfSort = OrderOfSort;
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }
    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }
    public SortOrder SecondOrder
    {
        set
        {
            SecondOrderOfSort = value;
        }
        get
        {
            return SecondOrderOfSort;
        }
    }
}