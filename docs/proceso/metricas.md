# Medicion y analisis (MA)

Metricas definidas con el enfoque objetivo, pregunta y metrica.

| Objetivo | Pregunta | Metrica | Fuente | Frecuencia |
|---|---|---|---|---|
| Cumplir el presupuesto de tiempo real | ¿Cuanto tarda el nucleo en actualizar un cuadro? | µs por cuadro con 400 plataformas, grafico X-R | `dotnet run -- --bench` | Cada linea base y cada ejecucion de CI |
| Cumplir el presupuesto de tiempo real | ¿El costo crece con el tamano del nivel? | Operaciones sobre plataformas por cuadro | `Program.cs` (comparativa) | Cada linea base |
| Entregar sin defectos | ¿Donde se originan los defectos? | Defectos por tipo y por fase (Pareto) | `registro-defectos.csv` | Cada defecto |
| Verificar lo que se entrega | ¿Las pruebas cubren los requisitos? | Requisitos con al menos una prueba | `requisitos.md` | Cada linea base |
| Mantener el cambio controlado | ¿Que tan grandes son los cambios? | Lineas agregadas y eliminadas por commit | `git log --numstat` | Cada linea base |

## Como regenerar

    dotnet run -- --bench metrics/bench.csv
    python3 tools/metricas.py

El grafico de control usa subgrupos de 5 con A2 = 0.577, D3 = 0 y D4 = 2.114.
La columna `recolecciones_gc` cuenta las pausas del recolector de basura de .NET
durante cada muestra, para separar causas especiales del entorno de cambios en
el codigo.
