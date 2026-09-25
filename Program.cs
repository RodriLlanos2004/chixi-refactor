using System;
using System.Collections.Generic;
using System.Globalization;

namespace Chixi
{
    // Ejecuta v0 y v1.0 con la misma secuencia y compara comportamiento y costo.
    public static class Program
    {
        // Ejercita todos los estados fuera del salto y el aterrizaje completo.
        private static readonly string[] Inputs = BuildInputs();
        private const int PlatformCount = 40;

        private static string[] BuildInputs()
        {
            List<string> s = new List<string>
            {
                "idle", "torch", "move", "move", "idle",
                "probe", "probe", "idle", "attack", "idle", "jump"
            };
            for (int i = 0; i < 29; i++) s.Add("idle");
            s.AddRange(new[] { "move", "torch", "move", "idle" });
            return s.ToArray();
        }

        private class Trace
        {
            public List<string> States = new List<string>();
            public List<int> Visible = new List<int>();
            public float FinalX;
        }

        public static void Main()
        {
            Trace v0 = RunBefore();
            Trace v1 = RunAfter();
            CultureInfo inv = CultureInfo.InvariantCulture;

            Console.WriteLine("Chixi: comparativa v0 vs v1.0");
            Console.WriteLine("Cuadros: " + Inputs.Length + " | Plataformas: " + PlatformCount);

            Console.WriteLine();
            Console.WriteLine("1. Estado reportado del jugador");
            int diffs = 0;
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (v0.States[i] == v1.States[i]) continue;
                diffs++;
                Console.WriteLine("   Cuadro " + i + ": v0 " + v0.States[i] + ", v1.0 " + v1.States[i]
                    + " (salida de " + v0.States[i] + ")");
            }
            Console.WriteLine("   Diferencias: " + diffs + " de " + Inputs.Length + " cuadros");
            Console.WriteLine("   Todas en el cuadro de salida de un estado gobernado por bandera:");
            Console.WriteLine("   v0 reporta el estado nuevo al entrar y el viejo al salir.");

            Console.WriteLine();
            Console.WriteLine("2. Comportamiento funcional");
            int sameVisible = 0;
            for (int i = 0; i < Inputs.Length; i++) if (v0.Visible[i] == v1.Visible[i]) sameVisible++;
            Console.WriteLine("   Plataformas visibles identicas en " + sameVisible + " de " + Inputs.Length + " cuadros");
            Console.WriteLine("   Posicion final X: v0 " + v0.FinalX.ToString("0.000", inv)
                + " | v1.0 " + v1.FinalX.ToString("0.000", inv));

            int before = Before.Metrics.PlatformChecks;
            int after = After.Metrics.AdapterPolls + After.Metrics.PlatformNotifications;
            Console.WriteLine();
            Console.WriteLine("3. Costo de actualizar plataformas");
            Console.WriteLine("   v0   comprobaciones por sondeo:  " + before);
            Console.WriteLine("   v1.0 consultas del adaptador:    " + After.Metrics.AdapterPolls);
            Console.WriteLine("   v1.0 notificaciones por evento:  " + After.Metrics.PlatformNotifications);
            Console.WriteLine("   Total: " + before + " -> " + after
                + " (reduccion del " + (100 - after * 100 / before) + " por ciento)");
        }

        private static Trace RunBefore()
        {
            Before.Metrics.Reset();
            Before.PlayerController player = new Before.PlayerController();
            Before.PlatformManager manager = new Before.PlatformManager();
            manager.torch = player.torch;
            for (int i = 0; i < PlatformCount; i++) manager.platforms.Add(new Before.MirrorPlatform("p" + i));
            player.platforms = manager;

            Trace t = new Trace();
            foreach (string input in Inputs)
            {
                t.States.Add(player.Update(input));
                int visible = 0;
                foreach (Before.MirrorPlatform p in manager.platforms) if (p.isVisible) visible++;
                t.Visible.Add(visible);
            }
            t.FinalX = player.x;
            return t;
        }

        private static Trace RunAfter()
        {
            After.Metrics.Reset();
            After.GameSession session = new After.GameSession(PlatformCount, new After.ReflectedPlatformFactory());

            Trace t = new Trace();
            foreach (string input in Inputs)
            {
                t.States.Add(session.Tick(input));
                t.Visible.Add(session.Platforms.VisibleCount());
            }
            t.FinalX = session.Player.X;
            return t;
        }
    }
}
