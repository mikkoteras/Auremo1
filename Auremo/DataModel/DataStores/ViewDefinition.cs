using Auremo.DataModel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auremo.DataModel.DataStores
{
    /// Created by ViewableDataStores and saved in the view history.
    /// Work as "bookmarks" that the ViewableDataStores can interpret
    /// and restore earlier states.
    public class ViewDefinition
    {
        public ViewDefinition(ViewMode view, RestorableDataStore creator)
        {
            ViewMode = view;
            Creator = creator;
            
        }

        public ViewMode ViewMode
        {
            get;
            private set;
        }

        public RestorableDataStore Creator
        {
            get;
            private set;
        }
    }
}
