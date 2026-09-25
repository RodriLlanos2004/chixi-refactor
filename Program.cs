using System;

namespace Chixi
{
    public static class Program
    {
        public static void Main()
        {
            string[] inputs = { "idle", "torch", "move", "move", "jump", "idle", "idle", "idle" };
            Before.PlayerController player = new Before.PlayerController();
            Before.PlatformManager platforms = new Before.PlatformManager();
            platforms.torch = player.torch;
            for (int i = 0; i < 40; i++) platforms.platforms.Add(new Before.MirrorPlatform("p" + i));
            player.platforms = platforms;

            foreach (string input in inputs) Console.WriteLine(input + " -> " + player.Update(input));
            Console.WriteLine("Comprobaciones de plataforma: " + Before.Metrics.PlatformChecks);
        }
    }
}
