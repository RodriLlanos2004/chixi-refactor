# Requisitos del nucleo de gameplay (REQM)

Linea base de requisitos: v1.1. Cada requisito se rastrea en ambos sentidos:
hacia el codigo que lo implementa y hacia la prueba que lo verifica.

| ID | Requisito | Implementacion | Verificacion |
|---|---|---|---|
| REQ-01 | El jugador esta siempre en uno solo de cinco estados: Idle, Walk, Jump, Probe o Attack, e inicia en Idle | `src/After/PlayerStates.cs` | T01, T06, T14 |
| REQ-02 | La transicion depende del estado actual y de la entrada; desde Probe o Attack, saltar regresa a Idle | `PlayerStates.cs` (`Handle`) | T02, T03, T04 |
| REQ-03 | El salto dura el primer multiplo de cuadro de 16 ms que supera 0.45 s: 29 cuadros | `JumpState` | T05 |
| REQ-04 | La antorcha inicia apagada; `torch` la enciende o apaga solo si hay combustible | `src/After/Lighting.cs` (`Torch`) | T07, T09 |
| REQ-05 | Encendida consume 0.5 de combustible por cuadro y se apaga sola al llegar a 0 | `Torch.ConsumeFuel` | T08 |
| REQ-06 | Las plataformas son visibles si y solo si la luz esta encendida | `EngineLightAdapter`, `Platform` | T10, T13 |
| REQ-07 | Si la luz no cambia, ninguna plataforma se actualiza: costo de una consulta por cuadro | `EngineLightAdapter.Poll` | T11, `metrics/bench.csv` |
| REQ-08 | Las plataformas se crean por tipo logico (bridge, step, monolith); un tipo desconocido es un error | `src/After/Platforms.cs` | T12 |
| REQ-09 | La actualizacion del nucleo por cuadro debe usar menos del 1 % del presupuesto de 16.6 ms (166 µs) con 400 plataformas | `GameSession.Tick` | `metrics/grafico-control.png` |

## Control de cambios de requisitos

| Fecha | Requisito | Cambio | Motivo |
|---|---|---|---|
| 2026-11-11 | REQ-03 | "dura 0.45 s" se precisa a "29 cuadros" | DEF-06: el valor no es multiplo del cuadro de 16 ms |
| 2026-11-11 | REQ-01 a REQ-09 | Primera linea base escrita | Antes los requisitos solo existian en el GDD y en el codigo |
