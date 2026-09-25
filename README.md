# Chixi: refactorizacion y patrones de diseno (v1.0)

Nucleo de gameplay de Chixi, vertical slice de accion y aventura en el Salar de
Uyuni. El repositorio conserva la version original y la refactorizada para que
la comparativa sea verificable.

| Ruta | Contenido |
|---|---|
| `src/Before/Legacy.cs` | Version v0, funcional pero acoplada |
| `src/After/PlayerStates.cs` | State |
| `src/After/Lighting.cs` | Adapter |
| `src/After/Platforms.cs` | Factory Method |
| `src/After/GameSession.cs` | Coordinador del cuadro |
| `Program.cs` | Ejecuta ambas versiones y compara |

## Ejecutar

Requiere .NET 8. Sin dependencias externas.

    dotnet build
    dotnet run

## Patrones

| Categoria | Patron | Deuda que resuelve |
|---|---|---|
| Comportamiento | State | Objeto Dios, metodo largo, tres banderas booleanas |
| Estructural | Adapter | Envidia de atributos y sondeo O(n) por cuadro |
| Creacional | Factory Method | Instanciacion directa de clases concretas |

## Resultado medido

Secuencia de 44 cuadros con 40 plataformas:

- Operaciones de actualizacion de plataformas: 1760 en v0, 164 en v1.0.
- Plataformas visibles identicas en los 44 cuadros y misma posicion final.
- Tres diferencias en el estado reportado, todas en el cuadro de salida de un
  estado gobernado por bandera: v0 reporta el estado nuevo al entrar y el viejo
  al salir. Es un defecto de la version original, corregido por State.

## Historial

Un commit para la linea base y uno por patron. El tag `v1.0` marca la entrega.
