# Plan de riesgos (PP SP 2.2 y PMC SP 1.3)

Escala: probabilidad e impacto de 1 (bajo) a 3 (alto). Exposicion = P x I.
Revision: en cada linea base.

| ID | Riesgo | P | I | Exp. | Mitigacion | Disparador | Estado |
|---|---|---|---|---|---|---|---|
| R1 | Las pruebas pasan pero no verifican el codigo entregado | 3 | 3 | 9 | Pruebas en C# sobre `src/After` y CI que las ejecuta en cada cambio | Suite en otro lenguaje o con copia de la logica | Materializado (DEF-02, DEF-03), mitigado en v1.1 |
| R2 | Cambios ajenos al alcance entran al remoto | 2 | 2 | 4 | Solo `main` y ramas `feature/` o `fix/`; revisar `git branch -r` en cada linea base | Rama con prefijo no autorizado | Materializado (DEF-04), abierto |
| R3 | El reflejo planar duplica el costo de dibujo y rompe los 16.6 ms | 2 | 3 | 6 | Medir el nucleo en cada linea base (REQ-09) y presupuestar en el motor | Media del grafico X fuera de control o mayor a 166 µs | Vigilado |
| R4 | Un solo responsable sin revision por pares | 3 | 2 | 6 | CI como revisor automatico y lista de verificacion de calidad antes de etiquetar | Commit a main sin ejecucion verde | Mitigado parcialmente |
| R5 | Perdida del trabajo local | 1 | 3 | 3 | Repositorio remoto en GitHub y tags de linea base | Trabajo sin subir mas de un dia | Vigilado |
| R6 | Requisitos implicitos en el codigo y no escritos | 3 | 2 | 6 | Linea base de requisitos con trazabilidad (requisitos.md) | Prueba que contradice al codigo sin requisito que decida | Materializado (DEF-06), mitigado en v1.1 |
