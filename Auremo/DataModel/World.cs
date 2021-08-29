using Auremo.DataModel.DataStores;
using System;

namespace Auremo.DataModel
{
    public class World
    {
        private static World m_Singleton = null;

        private World()
        {
        }

        public static World Create()
        {
            if (m_Singleton == null)
            {
                // The initialization sequence below requires that the singleton
                // object already exists, so we can't just put it in the ctor,
                // or use the new World() { ... } syntax.
                m_Singleton = new World();

                // Log is needed first so that we can log errors.
                m_Singleton.Log = new Log();

                // Configuration is needed next, as it contains information about
                // how to set up the rest.
                m_Singleton.Configuration = new Configuration();

                // Create simple data stores as they are simple.
                m_Singleton.InterfaceState = new InterfaceState();
                m_Singleton.DeveloperInfo = new DeveloperInfo();
                m_Singleton.ServerStatus = new ServerStatus();
                m_Singleton.Activity = new Activity();
                m_Singleton.Translator = new Translator("English");

                // Go multi-threaded.
                m_Singleton.CommandRunner = new CommandRunner();
                m_Singleton.QueryRunner = new QueryRunner();
                m_Singleton.ServerStatusUpdater = new ServerStatusUpdater();
                m_Singleton.PlayQueue = new PlayQueue();

                // Create complex data stores that use connectors autonomously.
                m_Singleton.QueryResult = new QueryResult();
                m_Singleton.PlaylistInfo = new PlaylistInfo();

                // Create the view history, which depends on almost everything.
                m_Singleton.ViewHistory = new ViewHistory();
                m_Singleton.ViewHistory.Reset();

                return m_Singleton;
            }
            else
            {
                throw new Exception("Unexpected call to World.Create().");
            }
        }

        public static void Destroy()
        {
            if (m_Singleton != null)
            {
                // Signal threads to stop.
                m_Singleton.CommandRunner.Stop();
                m_Singleton.QueryRunner.Stop();
                m_Singleton.ServerStatusUpdater.Stop();
                m_Singleton.PlayQueue.Stop();
                
                // Wait for the threads to finish.
                m_Singleton.CommandRunner.Join();
                m_Singleton.QueryRunner.Join();
                m_Singleton.ServerStatusUpdater.Join();
                m_Singleton.PlayQueue.Join();

                // End.
                m_Singleton = null;
            }
            else
            {
                throw new Exception("Unexpected call to World.Destroy().");
            }
        }

        public static World Instance => m_Singleton;

        public Log Log
        {
            get;
            private set;
        }

        public Configuration Configuration
        {
            get;
            private set;
        }

        public InterfaceState InterfaceState
        {
            get;
            private set;
        }

        public DeveloperInfo DeveloperInfo
        {
            get;
            private set;
        }

        public ServerStatus ServerStatus
        {
            get;
            private set;
        }

        public Activity Activity
        {
            get;
            private set;
        }

        public Translator Translator
        {
            get;
            private set;
        }

        public CommandRunner CommandRunner
        {
            get;
            private set;
        }

        public QueryRunner QueryRunner
        {
            get;
            private set;
        }

        public ServerStatusUpdater ServerStatusUpdater
        {
            get;
            set;
        }

        public QueryResult QueryResult
        {
            get;
            private set;
        }

        public PlaylistInfo PlaylistInfo
        {
            get;
            private set;
        }

        public PlayQueue PlayQueue
        {
            get;
            private set;
        }

        public ViewHistory ViewHistory
        {
            get;
            private set;
        }
    }
}
