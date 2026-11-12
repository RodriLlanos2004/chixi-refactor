using System;
using System.Collections.Generic;
using Chixi.After;

namespace Chixi.Tests
{
    // Pruebas unitarias sobre el codigo C# real (src/After).
    // Accion correctiva de DEF-02 y DEF-03: las pruebas de Jest validaban una
    // copia en JavaScript cuyas reglas no coinciden con este codigo.
    // Runner minimo sin paquetes externos: el repositorio no usa NuGet.
    // Cada prueba indica el requisito que verifica (docs/proceso/requisitos.md).
    public static class Program
    {
        private static readonly List<(string id, string req, string name, Action body)> Tests = new()
        {
            ("T01", "REQ-01", "El jugador inicia en Idle", () =>
            {
                var m = new PlayerStateMachine(new PlayerContext());
                Equal("Idle", m.Current.Name);
            }),
            ("T02", "REQ-02", "Desde Idle cada entrada lleva a su estado", () =>
            {
                Equal("Walk", Fresh().Update("move"));
                Equal("Jump", Fresh().Update("jump"));
                Equal("Probe", Fresh().Update("probe"));
                Equal("Attack", Fresh().Update("attack"));
            }),
            ("T03", "REQ-02", "Saltar durante el tanteo no salta: vuelve a Idle", () =>
            {
                var m = Fresh();
                m.Update("probe");
                Equal("Idle", m.Update("jump"));
            }),
            ("T04", "REQ-02", "El ataque dura un cuadro y vuelve a Idle", () =>
            {
                var m = Fresh();
                m.Update("attack");
                Equal("Idle", m.Update("jump"));
            }),
            ("T05", "REQ-03", "El salto dura 29 cuadros (primer multiplo de 16 ms que supera 0.45 s)", () =>
            {
                var m = Fresh();
                m.Update("jump");
                int frames = 1;
                while (m.Update("idle") == "Jump") frames++;
                Equal(29, frames);
                Equal("Idle", m.Current.Name);
            }),
            ("T06", "REQ-01", "Caminar desplaza al jugador 3.5 m/s", () =>
            {
                var ctx = new PlayerContext();
                var m = new PlayerStateMachine(ctx);
                m.Update("move");
                Near(0.056f, ctx.X);
            }),
            ("T07", "REQ-04", "La antorcha inicia apagada y se enciende con Toggle", () =>
            {
                var light = new EngineLight();
                var torch = new Torch(light);
                False(light.enabled);
                torch.Toggle();
                True(light.enabled);
            }),
            ("T08", "REQ-05", "Encendida consume 0.5 por cuadro y se apaga en el cuadro 200", () =>
            {
                var light = new EngineLight();
                var torch = new Torch(light);
                torch.Toggle();
                int frames = 0;
                while (light.enabled) { torch.ConsumeFuel(); frames++; }
                Equal(200, frames);
                Near(0f, torch.Fuel);
            }),
            ("T09", "REQ-04", "Sin combustible la antorcha no vuelve a encender", () =>
            {
                var light = new EngineLight();
                var torch = new Torch(light);
                torch.Toggle();
                while (light.enabled) torch.ConsumeFuel();
                torch.Toggle();
                False(light.enabled);
            }),
            ("T10", "REQ-06", "Las plataformas son visibles solo con la luz encendida", () =>
            {
                var light = new EngineLight();
                var adapter = new EngineLightAdapter(light);
                var p = new ReflectedPlatformFactory().Spawn("bridge", "p0", adapter);
                False(p.IsVisible);
                light.enabled = true; adapter.Poll();
                True(p.IsVisible);
                light.enabled = false; adapter.Poll();
                False(p.IsVisible);
            }),
            ("T11", "REQ-07", "Sin cambio de luz el adaptador no notifica a las plataformas", () =>
            {
                var light = new EngineLight();
                var adapter = new EngineLightAdapter(light);
                int calls = 0;
                adapter.Changed += _ => calls++;
                for (int i = 0; i < 10; i++) adapter.Poll();
                Equal(0, calls);
                light.enabled = true;
                for (int i = 0; i < 10; i++) adapter.Poll();
                Equal(1, calls);
            }),
            ("T12", "REQ-08", "La fabrica crea los tres tipos y rechaza uno desconocido", () =>
            {
                var f = new ReflectedPlatformFactory();
                True(f.Create("bridge", "a") is BridgePlatform);
                True(f.Create("step", "b") is StepPlatform);
                True(f.Create("monolith", "c") is MonolithPlatform);
                Throws<ArgumentException>(() => f.Create("roca", "d"));
            }),
            ("T13", "REQ-06", "Sesion: encender la antorcha muestra las 40 plataformas", () =>
            {
                var s = new GameSession(40, new ReflectedPlatformFactory());
                Equal(0, s.Platforms.VisibleCount());
                s.Tick("torch");
                Equal(40, s.Platforms.VisibleCount());
            }),
            ("T14", "REQ-01", "Regresion DEF-01: al aterrizar se reporta Idle, no Jump", () =>
            {
                var m = Fresh();
                m.Update("jump");
                string last = "Jump";
                for (int i = 0; i < 40 && last == "Jump"; i++) last = m.Update("idle");
                Equal("Idle", last);
            }),
        };

        private static PlayerStateMachine Fresh() => new PlayerStateMachine(new PlayerContext());

        public static int Main()
        {
            int failed = 0;
            foreach (var t in Tests)
            {
                try
                {
                    Metrics.Reset();
                    t.body();
                    Console.WriteLine("PASA  " + t.id + "  " + t.req + "  " + t.name);
                }
                catch (Exception e)
                {
                    failed++;
                    Console.WriteLine("FALLA " + t.id + "  " + t.req + "  " + t.name + " -> " + e.Message);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Resultado: " + (Tests.Count - failed) + " de " + Tests.Count + " pruebas pasan");
            return failed == 0 ? 0 : 1;
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception("se esperaba " + expected + " y se obtuvo " + actual);
        }

        private static void Near(float expected, float actual)
        {
            if (Math.Abs(expected - actual) > 0.0001f)
                throw new Exception("se esperaba " + expected + " y se obtuvo " + actual);
        }

        private static void True(bool c) { if (!c) throw new Exception("se esperaba verdadero"); }
        private static void False(bool c) { if (c) throw new Exception("se esperaba falso"); }

        private static void Throws<E>(Action a) where E : Exception
        {
            try { a(); }
            catch (E) { return; }
            throw new Exception("se esperaba " + typeof(E).Name);
        }
    }
}
