using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Chixi
{
    // Medicion del tiempo de actualizacion por cuadro (practica MA de CMMI).
    // Ejecuta la misma secuencia de 44 cuadros en v0 y v1.0 con 400 plataformas
    // y guarda 25 subgrupos de 5 muestras por version para el grafico de control X-R.
    // recolecciones_gc cuenta las pausas del recolector de basura durante la muestra,
    // para distinguir causas especiales (pausas del entorno) de cambios del proceso.
    public static class Benchmark
    {
        private const int Platforms = 400;
        private const int Subgroups = 25;
        private const int SubgroupSize = 5;
        private const int Warmup = 30;

        private static readonly string[] Sequence = BuildSequence();

        private static string[] BuildSequence()
        {
            List<string> s = new List<string> { "idle", "torch", "move", "move", "idle", "probe", "probe", "idle", "attack", "idle", "jump" };
            for (int i = 0; i < 29; i++) s.Add("idle");
            s.AddRange(new[] { "move", "torch", "move", "idle" });
            return s.ToArray();
        }

        public static void Run(string path)
        {
            for (int i = 0; i < Warmup; i++) { RunV0(out _); RunV1(out _); }

            CultureInfo inv = CultureInfo.InvariantCulture;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            using (StreamWriter w = new StreamWriter(path))
            {
                w.WriteLine("version,subgrupo,muestra,us_por_cuadro,recolecciones_gc");
                for (int g = 1; g <= Subgroups; g++)
                {
                    for (int m = 1; m <= SubgroupSize; m++)
                    {
                        double a = RunV0(out int ga);
                        w.WriteLine("v0," + g + "," + m + "," + a.ToString("0.000", inv) + "," + ga);
                        double b = RunV1(out int gb);
                        w.WriteLine("v1.0," + g + "," + m + "," + b.ToString("0.000", inv) + "," + gb);
                    }
                }
            }
            Console.WriteLine("Datos guardados en " + path);
        }

        private static double PerFrameMicros(Stopwatch sw)
        {
            return sw.Elapsed.TotalMilliseconds * 1000.0 / Sequence.Length;
        }

        private static double RunV0(out int gc)
        {
            Before.PlayerController player = new Before.PlayerController();
            Before.PlatformManager manager = new Before.PlatformManager();
            manager.torch = player.torch;
            for (int i = 0; i < Platforms; i++) manager.platforms.Add(new Before.MirrorPlatform("p" + i));
            player.platforms = manager;

            int g0 = GC.CollectionCount(0);
            Stopwatch sw = Stopwatch.StartNew();
            foreach (string input in Sequence) player.Update(input);
            sw.Stop();
            gc = GC.CollectionCount(0) - g0;
            return PerFrameMicros(sw);
        }

        private static double RunV1(out int gc)
        {
            After.GameSession session = new After.GameSession(Platforms, new After.ReflectedPlatformFactory());
            int g0 = GC.CollectionCount(0);
            Stopwatch sw = Stopwatch.StartNew();
            foreach (string input in Sequence) session.Tick(input);
            sw.Stop();
            gc = GC.CollectionCount(0) - g0;
            return PerFrameMicros(sw);
        }
    }
}
