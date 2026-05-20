ADR-009: Uso del Patrón Strategy para Tipos de Votación
Fecha: 17-05-2026
Sprint: S4
Estado: Propuesto

1) Contexto
En el dominio de Votify existen dos tipos de votación: "ESTANDAR" (voto simple por proyecto) y "MULTICRITERIO" (votación ponderada por criterios). Actualmente, la lógica que varía según el tipo está dispersa en múltiples lugares usando sentencias `if`/`switch`:
- `VotoService.VotarAsync` vs `VotarMulticriterioAsync` vs `VotarMulticriterioAnonimoAsync` tienen lógica de validación duplicada.
- `VotacionService.ObtenerResultadosAsync` verifica `EsMulticriterio()` (línea ~149) con lógica branching.
- `VotacionService.ValidarCriterios` tiene early-return para tipos no-multicriterio.
- `VotacionRepository.MapToDomain` tiene un `switch` sobre `dto.Tipo` (líneas ~121-127) para seleccionar factories.
- `EsMulticriterio()` y `EsMulticriterioPublico()` están duplicados en `VotoService` y `VotacionService` con implementaciones inconsistentes.

Esto viola el Principio Abierto/Cerrado (OCP): al añadir un nuevo tipo de votación, habría que modificar múltiples archivos.

2) Opciones consideradas
Opción A: No hacer nada.
- Mantener los `if`/`switch` dispersos en `VotoService`, `VotacionService` y `VotacionRepository`.
- Añadir nuevos tipos añadiendo más branches condicionales.

Opción B: Estrategia simple con interfaz y resolutor.
- Definir `IVotacionStrategy` en `Votify.Domain.Interfaces` con métodos: `ValidarVoto()`, `ProcesarVoto()`, `CalcularResultados()`.
- Crear `VotacionEstandarStrategy`, `VotacionMulticriterioStrategy`, `VotacionMulticriterioPublicoStrategy` en `Votify.Application/Services/Estrategia/`.
- Crear `VotacionStrategyResolver` que recibe el `Tipo` string y devuelve la estrategia correcta.
- Reemplazar todos los `switch` por delegación a la estrategia injectada.

Opción C: Usar el patrón Strategy con estado compuesto (Strategy + State).
- Combinar `IVotacionStrategy` con el `IEstadoVotacion` existente: `IVotacionStrategy` maneja el tipo, `IEstadoVotacion` maneja el ciclo de vida.
- Más flexible pero más complejo de implementar.

3) Criterios de decisión
- Mantenibilidad: eliminar la duplicación de lógica `EsMulticriterio()` en múltiples servicios.
- Extensibilidad (OCP): poder añadir nuevos tipos de votación sin modificar servicios existentes.
- Responsabilidad Única (SRP): cada estrategia tiene una sola responsabilidad.
- Complejidad: minimizar cambios en archivos ya estables (`VotacionRepository`, factories).
- Compatibilidad: no romper el State Pattern existente ni los tests actuales.

4) Decisión tomada
Se elige la Opción B: introducir el Patrón Strategy con `IVotacionStrategy` y un `VotacionStrategyResolver`.

Esto permite que `VotoService` y `VotacionService` deleguen la lógica específica de cada tipo a una implementación concreta de `IVotacionStrategy`, eliminando todos los `switch`/`if` sobre `Tipo` y manteniendo la complejidad controlada.

5) Consecuencias
Positivas:
- Elimina duplicación de `EsMulticriterio()` y lógica branching en servicios.
- Cada estrategia es testeable independientemente.
- Añadir un nuevo tipo de votación solo requiere crear una nueva clase estrategia + registrarla en el resolver.
- Los servicios se vuelven más pequeños y enfocados.

Negativas / trade-offs:
- Aumenta el número de clases (~5 archivos nuevos).
- Requiere modificar la firma de algunos métodos de `VotoService` para injectar la estrategia.
- Los tests actuales de `VotoService` necesitan actualizarse para usar los nuevos constructores con estrategia.

Riesgos y mitigaciones:
- Riesgo: generar dependencia circular entre `Votify.Application` (estrategias) y `Votify.Domain` (interfaz).
  Mitigación: definir `IVotacionStrategy` en `Votify.Domain.Interfaces`, no en Application.
- Riesgo: el `VotacionStrategyResolver` se convierte en un nuevo lugar con lógica `switch`.
  Mitigación: el switch solo existe en el resolver y es el único lugar donde cambia al añadir tipos.

6) Evidencia
- Archivos propuestos en `Votify.Domain/Interfaces/IVotacionStrategy.cs` y `Votify.Application/Services/Estrategia/`.
- Los servicios `VotoService` y `VotacionService` reducirán su tamaño significativamente.
- Tests unitarios para cada estrategia concreta.