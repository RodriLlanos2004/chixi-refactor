# Plan del proyecto (PP)

## Alcance

Nucleo de gameplay de Ch'ixi en C#: estados del jugador, antorcha, luz y
plataformas reflejadas. El juego completo en Unity queda fuera de este
repositorio; aqui se valida la logica que despues se integra al motor.

## Responsable

Proyecto individual: Rodrigo Sebastian Llanos Yanez cubre los roles de
desarrollo, pruebas, gestion de configuracion y aseguramiento de calidad.

## Hitos y entregables

| Hito | Entregable | Fecha | Linea base |
|---|---|---|---|
| H1 | Diagnostico de deuda tecnica y version original | 2026-09-24 | `v0` |
| H2 | Refactorizacion con State, Adapter y Factory Method | 2026-09-24 | `v1.0` |
| H3 | Pruebas unitarias automatizadas | 2026-10-01 | sin linea base |
| H4 | Expediente de auditoria CMMI: proceso, metricas, pruebas sobre el codigo real e integracion continua | 2026-11-12 | `v1.1` |

H1 a H3 se registran a posteriori a partir del historial de git: no hubo un plan
escrito antes de ejecutarlos. H4 es el primer hito planeado antes de empezar.

## Estimacion

Sin datos historicos de esfuerzo (ver seguimiento.md), la estimacion se hace por
tamano: cada hito se limita a un cambio que se pueda revisar en un solo commit
por area (un patron, una suite de pruebas, un flujo de CI).

## Riesgos

Ver [riesgos.md](riesgos.md).
