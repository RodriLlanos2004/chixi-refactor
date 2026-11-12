# Plan de gestion de configuracion (CM)

## Elementos de configuracion

| Elemento | Ruta | Control |
|---|---|---|
| Codigo fuente | `src/`, `Program.cs`, `Benchmark.cs` | Git |
| Pruebas | `tests/Chixi.Tests/` | Git |
| Requisitos y planes | `docs/proceso/` | Git |
| Datos de medicion | `metrics/bench.csv` | Git, regenerables |
| Configuracion de compilacion | `Chixi.Refactor.csproj`, `NuGet.config` | Git |
| Flujo de CI | `.github/workflows/ci.yml` | Git |
| Dependencias de Node | `tests/node_modules/` | Fuera de control: se reconstruyen con `npm ci` |

## Lineas base

Una linea base es un tag anotado de git. No se mueve ni se borra.

| Tag | Commit | Contenido | Criterio de aprobacion |
|---|---|---|---|
| `v0` | 298f07b | Version original con deuda tecnica | Compila y ejecuta. Etiquetada a posteriori el 2026-11-11 |
| `v1.0` | 7572971 | Refactorizacion con tres patrones | Compila y la comparativa reproduce el comportamiento |
| `v1.1` | ver CHANGELOG | Proceso, pruebas C#, CI y metricas | CI en verde y 14 de 14 pruebas |

## Ramas

- `main`: unica rama de integracion. Todo lo que llega a `main` compila y pasa
  las pruebas.
- `feature/<tema>` y `fix/<DEF-nn>`: trabajo temporal; se borran al integrarse.
- Cualquier otra rama en el remoto es una no conformidad (ver DEF-04).

## Convencion de commits

`tipo(alcance): descripcion`, con tipo `feat`, `fix`, `refactor`, `test`,
`docs`, `ci` o `chore`. Un commit que corrige un defecto cita su ID.

Una prueba solo cuenta como evidencia de verificacion si cita un requisito y
ejecuta codigo de `src/` (accion preventiva de DEF-02 y DEF-03).

## Control de cambios

1. Todo cambio se registra como defecto o requisito con ID.
2. Se implementa en un commit que cita el ID.
3. CI debe quedar en verde.
4. Al cerrar un hito se crea el tag de linea base y se actualiza el CHANGELOG.

## Auditoria de configuracion

Antes de cada tag se verifica: `git status` limpio, `git branch -r` sin ramas
fuera de la convencion, CI en verde y CHANGELOG actualizado.
