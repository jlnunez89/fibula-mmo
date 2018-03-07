namespace OpenTibia.Server.Standalone
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Communications;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Handlers;
    using OpenTibia.Server.Handlers.Management;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Monsters;

    public class Program
    {
        private static IOpenTibiaListener _loginListener;
        private static IOpenTibiaListener _gameListener;
        //private static IOpenTibiaListener managementListener;

        static void Main()
        {
            //GCNotifier.GarbageCollected += GCNotifier_GarbageCollected;
            //GCNotifier.Start();

            var cancellationTokenSource = new CancellationTokenSource();

            // Set the loaders to use.
            Game.Instance.Initialize(new MoveUseEventLoader(), new ObjectsFileItemLoader(), new MonFilesMonsterLoader());

            // TODO: load and validate external aux files.

            // Set the persistence storage source (database)


            // Initilize client listening pipeline (but reject game connections)
            _loginListener = new LoginListener(new ManagementHandlerFactory());
            //managementListener = new ManagementListener(new ManagementHandlerFactory());
            _gameListener = new GameListener(new GameHandlerFactory());

            var listeningTask = RunAsync(cancellationTokenSource.Token);

            // Initilize game
            Game.Instance.Begin(cancellationTokenSource.Token);

            // TODO: open up game connections

            listeningTask.Wait(cancellationTokenSource.Token);
        }

        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            _loginListener.BeginListening();
            //managementListener.BeginListening();
            _gameListener.BeginListening();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
