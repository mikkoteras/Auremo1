namespace Auremo.DataModel.DataStores
{
    public abstract class RestorableDataStore
    {
        public virtual void RestoreFromDefinition(ViewDefinition definition)
        {
            World.Instance.InterfaceState.ViewMode = definition.ViewMode;
        }

        #warning TODO create a proper README.

        // Any RestorableDataStore needs to implement some kind of a DisplayView(...)
        // method, which reinitializes the data store (and hence its related view(s)
        // to the desired state, creates a ViewDefinition object that contains the
        // essential setup of the state, and pushes it into ViewHistory.
        //
        // RestoreFromDefinition can then be called with the same ViewDefinition
        // object, and interpreted by the RestorableDataStore (as the definition objects
        // are just black boxes to other classes) to restore the state set earlier
        // with DisplayView.
        //
        // The canonical way to jump from one UI state to another, then, is to
        // find the data store of interest in World.Instance and call its
        // DisplayView. This achieves a few things:
        //
        // 1) All classes can be completely agnostic about the existence of any
        //    xaml.cs classes, including other xaml.cs classes. (Acknowledging
        //    MainWindow is still necessary, though.)
        // 2) A browseable view history that doesn't need to know much about the
        //    views it restores. It is also easy to extend without introducing much
        //    new code.
    }
}
