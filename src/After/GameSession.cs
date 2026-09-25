namespace Chixi.After
{
    // Coordinador delgado del cuadro. Sustituye al Update monolitico de v0:
    // no decide nada, solo ordena a cada responsable en la misma secuencia.
    public class GameSession
    {
        private static readonly string[] Kinds = { "bridge", "step", "monolith" };

        private readonly PlayerStateMachine _player;
        private readonly Torch _torch;
        private readonly EngineLightAdapter _light;
        private readonly PlatformRegistry _platforms = new PlatformRegistry();

        public PlayerContext Player { get; private set; }
        public PlatformRegistry Platforms { get { return _platforms; } }

        public GameSession(int platformCount, PlatformFactory factory)
        {
            Player = new PlayerContext();
            _player = new PlayerStateMachine(Player);

            EngineLight engineLight = new EngineLight();
            _torch = new Torch(engineLight);
            _light = new EngineLightAdapter(engineLight);

            for (int i = 0; i < platformCount; i++)
                _platforms.Add(factory.Spawn(Kinds[i % Kinds.Length], "p" + i, _light));
        }

        public string Tick(string input)
        {
            string state = _player.Update(input);
            if (input == "torch") _torch.Toggle();
            _torch.ConsumeFuel();
            _light.Poll();
            return state;
        }
    }
}
