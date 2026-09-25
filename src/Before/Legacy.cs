using System;
using System.Collections.Generic;

// VERSION CON DEUDA TECNICA (v0).
// Se conserva en el repositorio para que la comparativa "antes vs despues"
// sea verificable. No se debe extender: sirve como linea base de medicion.
namespace Chixi.Before
{
    public static class Metrics
    {
        public static int PlatformChecks;
        public static int Allocations;
        public static void Reset() { PlatformChecks = 0; Allocations = 0; }
    }

    // Smell: clase de datos publica sin encapsulamiento.
    public class TorchController
    {
        public bool isOn = false;
        public float fuel = 100f;

        public void TurnOn() { if (fuel > 0f) isOn = true; }
        public void TurnOff() { isOn = false; }
        public void ConsumeFuel() { if (isOn) { fuel -= 0.5f; if (fuel <= 0f) { fuel = 0f; isOn = false; } } }
    }

    public class MirrorPlatform
    {
        public string id;
        public bool isVisible;
        public MirrorPlatform(string id) { this.id = id; }
        public void SetVisible(bool v) { isVisible = v; }
    }

    // Smell: Feature Envy e Inappropriate Intimacy.
    // PlatformManager lee los campos publicos de TorchController en cada cuadro.
    // Smell: sondeo O(n) por cuadro, el costo crece con el tamano del nivel.
    public class PlatformManager
    {
        public List<MirrorPlatform> platforms = new List<MirrorPlatform>();
        public TorchController torch;

        public void Update()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                Metrics.PlatformChecks++;
                if (torch.isOn && torch.fuel > 0f) platforms[i].SetVisible(true);
                else platforms[i].SetVisible(false);
            }
        }
    }

    public class Splash { public Splash() { Metrics.Allocations++; } }

    // Smell: creacion directa con new dentro de la logica de juego.
    public class VFXSpawner
    {
        public List<Splash> live = new List<Splash>();
        public void SpawnSplash() { live.Add(new Splash()); }
    }

    public class AudioManager { public void PlaySfx(string clip) { } }
    public class UIManager { public void ShowFuelBar(float v) { } }

    // Smell principal: Objeto Dios.
    // Entrada, movimiento, estado, antorcha, audio, interfaz y efectos en una clase.
    // Smell: Long Method con condicionales anidados y banderas booleanas.
    // Smell: Magic Numbers dispersos.
    public class PlayerController
    {
        public TorchController torch = new TorchController();
        public PlatformManager platforms;
        public VFXSpawner vfx = new VFXSpawner();
        public AudioManager audio = new AudioManager();
        public UIManager ui = new UIManager();

        public bool isJumping = false;
        public bool isProbing = false;
        public bool isAttacking = false;
        public float x = 0f;
        public float airTime = 0f;

        public string Update(string input)
        {
            string label;

            if (isJumping)
            {
                airTime += 0.016f;
                if (airTime > 0.45f) { isJumping = false; airTime = 0f; vfx.SpawnSplash(); audio.PlaySfx("land"); }
                label = "Jump";
            }
            else if (isAttacking)
            {
                isAttacking = false;
                audio.PlaySfx("attack");
                label = "Attack";
            }
            else if (isProbing)
            {
                if (input != "probe") isProbing = false;
                label = "Probe";
            }
            else
            {
                if (input == "jump") { isJumping = true; audio.PlaySfx("jump"); label = "Jump"; }
                else if (input == "attack") { isAttacking = true; label = "Attack"; }
                else if (input == "probe") { isProbing = true; label = "Probe"; }
                else if (input == "move") { x += 3.5f * 0.016f; audio.PlaySfx("step"); label = "Walk"; }
                else label = "Idle";
            }

            if (input == "torch") { if (torch.isOn) torch.TurnOff(); else torch.TurnOn(); }
            torch.ConsumeFuel();
            ui.ShowFuelBar(torch.fuel);
            platforms.Update();
            return label;
        }
    }
}
