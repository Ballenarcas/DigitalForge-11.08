ADR-010: Uso del Patrón Observer para Notificaciones de Cambio de Estado de Votación
Fecha: 17-05-2026
Sprint: S4
Estado: Propuesto

1) Contexto
En el dominio de Votify, la entidad `Votacion` transita por estados (`IEstadoVotacion`: Abierta → Pausada → Finalizada). Actualmente, cuando una votación cambia de estado en `VotacionService` (métodos `PausarVotacionAsync`, `DetenerVotacionAsync`, `AbrirVotacionAsync`, `ActualizarEstadosAutomaticosAsync`), no existe ningún mecanismo de notificación hacia otros componentes del sistema que necesiten reaccionar a estos cambios.

Esto genera acoplamiento directo: si en el futuro se quiere enviar notificaciones por correo, actualizar scoring, o trackear analíticas tras un cambio de estado, habría que modificar `VotacionService` directamente, violando el Principio Abierto/Cerrado (OCP).

2) Opciones consideradas
Opción A: No hacer nada.
- Mantener los métodos de transición de estado como están.
- Añadir lógica adicional directamente en `VotacionService` para cada nueva necesidad.

Opción B: Introducir el Patrón Observer básico.
- Definir `IVotacionObserver` en `Votify.Domain.Interfaces` con un método `OnEstadoCambiado(votacion, estadoAnterior, estadoNuevo)`.
- Definir `IVotacionObservable` en `Votify.Domain.Interfaces` con métodos `Attach(observer)` y `Notify()`.
- Crear `VotacionSubject` en `Votify.Application/Services/Observer/` que implemente `IVotacionObservable`.
- Crear observadores concretos: `NotificacionObserver`, `ScoringObserver`, `AnalyticsObserver`.
- Modificar `VotacionService` para usar el subject y notificar en cada transición.

Opción C: Usar eventos de .NET (EventHandler/Delegate).
- Reemplazar el patrón Observer manual con eventos integrados de C#.
- Menos código boilerplate pero más difícil de testar y menos flexible para múltiples observers con prioridades.

3) Criterios de decisión
- Mantenibilidad: poder añadir/eliminar реакции a cambios de estado sin modificar `VotacionService`.
- Extensibilidad (OCP): nuevos observers se añaden sin tocar código existente.
- Compatibilidad: no interferir con el `IEstadoVotacion` existente (State Pattern).
- Testabilidad: cada observer testeable independientemente.
- Simplicidad: evitar boilerplate excesivo.

4) Decisión tomada
Se elige la Opción B: introducir el Patrón Observer con interfaces en Dominio y múltiples observers concretos en Application.

`IVotacionObserver` define el contrato. `IVotacionObservable` (implementado por `VotacionSubject`) gestiona la lista de observers y dispara las notificaciones. Los observers concretos (`NotificacionObserver`, `ScoringObserver`, `AnalyticsObserver`) encapsulan cada reacción. `VotacionService` permanece ajeno a qué observers están registrados.

5) Consecuencias
Positivas:
- Desacopla `VotacionService` de las consecuencias de los cambios de estado.
- Cada observer es independiente y testeable.
- Añadir nuevas reacciones (ej. webhook, logging detallado) solo requiere crear un observer.
- El State Pattern sigue intacto — Observer notifica "que" cambió, State controla "si" se puede cambiar.

Negativas / trade-offs:
- Añade ~6 archivos nuevos.
- Posible orden de ejecución no determinista si varios observers modifican estado compartido.
- Requiere registrar todos los observers en DI.

Riesgos y mitigaciones:
- Riesgo: observers que lancen excepciones rompen la cadena de notificación.
  Mitigación: cada `Notify()` envuelve la llamada en try/catch individual por observer, continuando con los siguientes aunque uno falle.
- Riesgo: memory leaks si un observer no se desregistra.
  Mitigación: usar WeakReferences o un método `Detach()` explícito en `IVotacionObservable`.

6) Evidencia
- Archivos propuestos: `IVotacionObserver.cs`, `IVotacionObservable.cs` en `Votify.Domain.Interfaces`.
- `VotacionSubject.cs`, `NotificacionObserver.cs`, `ScoringObserver.cs`, `AnalyticsObserver.cs` en `Votify.Application/Services/Observer/`.
- `VotacionService` modificado para injectar `IVotacionObservable` y llamar `Notify()` tras cada transición.