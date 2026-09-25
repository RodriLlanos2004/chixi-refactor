namespace Chixi.After
{
    // PATRON DE COMPORTAMIENTO: State.
    // Handle decide la transicion segun el estado actual.
    // Execute realiza la accion del cuadro en el estado ya resuelto.
    public interface IPlayerState
    {
        string Name { get; }
        void Enter(PlayerContext ctx);
        IPlayerState Handle(string input, PlayerContext ctx);
        void Execute(string input, PlayerContext ctx);
    }

    // Datos del jugador. Los estados no guardan datos propios, por eso
    // pueden compartirse entre transiciones sin reservar memoria.
    public class PlayerContext
    {
        public const float Speed = 3.5f;
        public const float DeltaTime = 0.016f;
        public const float JumpDuration = 0.45f;

        public float X;
        public float AirTime;
    }

    // Instancias unicas y sin estado: cero asignaciones por transicion.
    public static class PlayerStates
    {
        public static readonly IPlayerState Idle = new IdleState();
        public static readonly IPlayerState Walk = new WalkState();
        public static readonly IPlayerState Jump = new JumpState();
        public static readonly IPlayerState Probe = new ProbeState();
        public static readonly IPlayerState Attack = new AttackState();
    }

    public class IdleState : IPlayerState
    {
        public string Name { get { return "Idle"; } }
        public void Enter(PlayerContext ctx) { }
        public IPlayerState Handle(string input, PlayerContext ctx)
        {
            switch (input)
            {
                case "move": return PlayerStates.Walk;
                case "jump": return PlayerStates.Jump;
                case "probe": return PlayerStates.Probe;
                case "attack": return PlayerStates.Attack;
                default: return this;
            }
        }
        public void Execute(string input, PlayerContext ctx) { }
    }

    public class WalkState : IPlayerState
    {
        public string Name { get { return "Walk"; } }
        public void Enter(PlayerContext ctx) { }
        public IPlayerState Handle(string input, PlayerContext ctx)
        {
            switch (input)
            {
                case "move": return this;
                case "jump": return PlayerStates.Jump;
                case "probe": return PlayerStates.Probe;
                case "attack": return PlayerStates.Attack;
                default: return PlayerStates.Idle;
            }
        }
        public void Execute(string input, PlayerContext ctx)
        {
            ctx.X += PlayerContext.Speed * PlayerContext.DeltaTime;
        }
    }

    public class JumpState : IPlayerState
    {
        public string Name { get { return "Jump"; } }
        public void Enter(PlayerContext ctx) { ctx.AirTime = 0f; }
        public IPlayerState Handle(string input, PlayerContext ctx)
        {
            ctx.AirTime += PlayerContext.DeltaTime;
            return ctx.AirTime > PlayerContext.JumpDuration ? PlayerStates.Idle : this;
        }
        public void Execute(string input, PlayerContext ctx) { }
    }

    public class ProbeState : IPlayerState
    {
        public string Name { get { return "Probe"; } }
        public void Enter(PlayerContext ctx) { }
        public IPlayerState Handle(string input, PlayerContext ctx)
        {
            return input == "probe" ? this : PlayerStates.Idle;
        }
        public void Execute(string input, PlayerContext ctx) { }
    }

    public class AttackState : IPlayerState
    {
        public string Name { get { return "Attack"; } }
        public void Enter(PlayerContext ctx) { }
        public IPlayerState Handle(string input, PlayerContext ctx) { return PlayerStates.Idle; }
        public void Execute(string input, PlayerContext ctx) { }
    }

    // Contexto del patron: delega la transicion y la accion al estado vigente.
    public class PlayerStateMachine
    {
        private readonly PlayerContext _ctx;
        private IPlayerState _current = PlayerStates.Idle;

        public PlayerStateMachine(PlayerContext ctx) { _ctx = ctx; }

        public IPlayerState Current { get { return _current; } }

        public string Update(string input)
        {
            IPlayerState next = _current.Handle(input, _ctx);
            if (!ReferenceEquals(next, _current))
            {
                _current = next;
                _current.Enter(_ctx);
            }
            _current.Execute(input, _ctx);
            return _current.Name;
        }
    }
}
