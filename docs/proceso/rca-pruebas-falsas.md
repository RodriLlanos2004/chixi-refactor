# Analisis de causa raiz: pruebas en verde que no verifican el codigo (DEF-02, DEF-03)

## Desviacion

El commit 3876f22 agrego 5 pruebas Jest que pasan 5 de 5. Al compararlas con el
codigo C# entregado (`src/After`) en la auditoria del 2026-11-11, las pruebas
resultan validar un modelo distinto: ejercen una copia en JavaScript
(`tests/src/`) y no las clases reales.

| # | Regla | Copia JavaScript que prueba Jest | Codigo C# entregado |
|---|---|---|---|
| 1 | Saltar desde tanteo o ataque | Pasa a `jumping` | Regresa a `Idle` |
| 2 | Accion `stop` | Lleva a `idle` | No existe |
| 3 | Estados Probe y Attack | No existen | Existen |
| 4 | Nombres de estado | `walking`, `jumping` | `Walk`, `Jump` |
| 5 | Estado inicial de la antorcha | Encendida | Apagada |
| 6 | Consumo de combustible | Cantidad arbitraria por llamada | 0.5 por cuadro |
| 7 | Encendido | No hay forma de encenderla | `Toggle` |

## Cinco porques

1. ¿Por que las pruebas pasan si el codigo se comporta distinto? Porque ejercen
   una copia en JavaScript y no las clases de `src/After`.
2. ¿Por que se probo una copia? Porque las pruebas se escribieron con Jest, que
   ejecuta JavaScript y no puede cargar ensamblados de .NET.
3. ¿Por que no se uso un marco de pruebas de .NET? Porque el repositorio no tenia
   proyecto de pruebas y su `NuGet.config` elimina todas las fuentes de paquetes,
   asi que un marco como xUnit no se podia restaurar sin cambiar la configuracion.
4. ¿Por que la copia pudo inventar reglas sin que nada lo detectara? Porque no
   existian requisitos escritos con ID: la unica fuente de verdad era el propio
   codigo, y nadie comparo la copia contra el.
5. ¿Por que nadie lo comparo? Porque no habia un proceso de verificacion definido:
   ni integracion continua, ni revision por pares (proyecto individual), ni
   trazabilidad entre requisito, codigo y prueba.

**Causa raiz:** ausencia de un proceso de verificacion con trazabilidad. La suite
media "pruebas en verde", no "requisitos verificados sobre el codigo entregado".

## Acciones

| Tipo | Accion | Evidencia | Estado |
|---|---|---|---|
| Correctiva | 14 pruebas en C# sobre las clases reales, con runner sin paquetes externos | `tests/Chixi.Tests/Program.cs` | Cerrada |
| Correctiva | Requisitos con ID y trazabilidad a codigo y pruebas | `requisitos.md` | Cerrada |
| Preventiva | CI que compila y ejecuta la suite C# en cada push y bloquea si falla | `.github/workflows/ci.yml` | Cerrada |
| Preventiva | Regla: una prueba solo cuenta si cita un requisito y ejecuta codigo de `src/` | `gestion-configuracion.md` | Cerrada |
| Preventiva | La suite Jest queda como referencia historica y fuera de la puerta de calidad | `README.md` | Cerrada |

## Verificacion de eficacia

La suite C# detecto en su primera ejecucion una ambiguedad real del requisito de
salto (DEF-06): la prueba esperaba 28 cuadros y el codigo usa 29. La copia en
JavaScript no habria podido detectarla porque no modela el salto por cuadros.
