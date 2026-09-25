using System;

namespace Chixi.After
{
    // Interfaz objetivo: lo que el dominio necesita saber de una fuente de luz.
    public interface ILightSource
    {
        bool IsLit { get; }
        event Action<bool> Changed;
    }

    // Tipo adaptado. Simula UnityEngine.Light: pertenece al motor, no se puede
    // modificar y no emite eventos. Solo expone su estado como campos.
    public class EngineLight
    {
        public bool enabled;
        public float intensity = 1.2f;
    }

    // PATRON ESTRUCTURAL: Adapter.
    // Traduce la API del motor a ILightSource. Es el unico punto del sistema
    // que consulta la luz por cuadro; las plataformas solo reciben eventos.
    public class EngineLightAdapter : ILightSource
    {
        private const float VisibilityThreshold = 0.1f;
        private readonly EngineLight _light;
        private bool _last;

        public event Action<bool> Changed;

        public EngineLightAdapter(EngineLight light)
        {
            _light = light;
            _last = Evaluate();
        }

        public bool IsLit { get { return Evaluate(); } }

        public void Poll()
        {
            Metrics.AdapterPolls++;
            bool now = Evaluate();
            if (now == _last) return;
            _last = now;
            if (Changed != null) Changed(now);
        }

        private bool Evaluate()
        {
            return _light.enabled && _light.intensity > VisibilityThreshold;
        }
    }

    // Logica de dominio de la antorcha: combustible y encendido.
    // Gobierna la luz del motor igual que un componente de Unity gobierna su Light.
    public class Torch
    {
        private const float FuelPerFrame = 0.5f;
        private readonly EngineLight _light;
        private float _fuel = 100f;

        public Torch(EngineLight light) { _light = light; }

        public float Fuel { get { return _fuel; } }

        public void Toggle()
        {
            _light.enabled = !_light.enabled && _fuel > 0f;
        }

        public void ConsumeFuel()
        {
            if (!_light.enabled) return;
            _fuel -= FuelPerFrame;
            if (_fuel > 0f) return;
            _fuel = 0f;
            _light.enabled = false;
        }
    }
}
