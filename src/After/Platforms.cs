using System;
using System.Collections.Generic;

namespace Chixi.After
{
    // Producto abstracto.
    public abstract class Platform
    {
        protected Platform(string id) { Id = id; }

        public string Id { get; private set; }
        public bool IsVisible { get; private set; }
        public abstract string Kind { get; }

        // Reaccion puntual a la notificacion de la luz: no recorre nada.
        public void OnLightChanged(bool lit)
        {
            Metrics.PlatformNotifications++;
            IsVisible = lit;
        }
    }

    // Productos concretos.
    public class BridgePlatform : Platform
    {
        public BridgePlatform(string id) : base(id) { }
        public override string Kind { get { return "Puente reflejado"; } }
    }

    public class StepPlatform : Platform
    {
        public StepPlatform(string id) : base(id) { }
        public override string Kind { get { return "Escalon de glifo"; } }
    }

    public class MonolithPlatform : Platform
    {
        public MonolithPlatform(string id) : base(id) { }
        public override string Kind { get { return "Monolito emergente"; } }
    }

    // PATRON CREACIONAL: Factory Method.
    // Create es el metodo fabrica: las subclases deciden la clase concreta.
    // Spawn es la operacion que lo usa y deja la plataforma suscrita a la luz.
    public abstract class PlatformFactory
    {
        public abstract Platform Create(string kind, string id);

        public Platform Spawn(string kind, string id, ILightSource light)
        {
            Platform platform = Create(kind, id);
            light.Changed += platform.OnLightChanged;
            platform.OnLightChanged(light.IsLit);
            return platform;
        }
    }

    public class ReflectedPlatformFactory : PlatformFactory
    {
        public override Platform Create(string kind, string id)
        {
            switch (kind)
            {
                case "bridge": return new BridgePlatform(id);
                case "step": return new StepPlatform(id);
                case "monolith": return new MonolithPlatform(id);
                default: throw new ArgumentException("Tipo de plataforma desconocido: " + kind);
            }
        }
    }

    public class PlatformRegistry
    {
        private readonly List<Platform> _platforms = new List<Platform>();

        public int Count { get { return _platforms.Count; } }

        public void Add(Platform platform) { _platforms.Add(platform); }

        public int VisibleCount()
        {
            int visible = 0;
            foreach (Platform p in _platforms) if (p.IsVisible) visible++;
            return visible;
        }
    }
}
