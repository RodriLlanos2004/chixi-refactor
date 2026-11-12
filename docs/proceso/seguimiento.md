# Seguimiento del proyecto (PMC)

## Avance por hito

| Hito | Fecha | Resultado | Evidencia | Desviacion |
|---|---|---|---|---|
| H1 | 2026-09-24 | Version original funcional con seis olores de codigo | commit 298f07b, tag `v0` | Ninguna |
| H2 | 2026-09-24 | Tres patrones; 1760 a 164 operaciones sobre plataformas | commits 7c0073d, 87918ae, f75506e, 7572971, tag `v1.0` | Ninguna |
| H3 | 2026-10-01 | 5 pruebas Jest en verde | commit 3876f22 | Las pruebas no verifican el codigo C# (DEF-02, DEF-03) |
| H4 | 2026-11-11 | Proceso documentado, 14 pruebas C# en verde, CI y metricas | tag `v1.1` | Rama ajena en el remoto (DEF-04) |

## Limitaciones de la medicion

- Los commits de H1 y H2 se registraron en un solo lote de 20 segundos: separan
  los cambios logicos, pero no reflejan cuanto tardo cada uno. No hay datos de
  esfuerzo por tarea y no se estiman.
- El proyecto tiene un solo responsable, asi que no existen metricas de equipo
  como velocidad o carga por integrante.

## Acciones correctivas

| Origen | Accion | Estado |
|---|---|---|
| DEF-02, DEF-03 | Suite C# sobre el codigo real (`tests/Chixi.Tests`) | Cerrada |
| DEF-05 | Integracion continua en cada push (`.github/workflows/ci.yml`) | Cerrada |
| DEF-06 | Linea base de requisitos con trazabilidad | Cerrada |
| DEF-04 | Eliminar la rama `claude/sharp-darwin-vhinn5` del remoto | Abierta |
