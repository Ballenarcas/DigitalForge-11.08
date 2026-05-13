ADR-006: Uso del Patrón de Diseño Facade para Simplificar la Capa de Aplicación
Fecha: 13-05-2026
Sprint: S3
Estado: Aceptado

1) Contexto
A medida que el proyecto Votify ha crecido, los controllers de la API han ido acumulando responsabilidades de orquestación que deberían pertenecer a la capa de Aplicación. Se han identificado los siguientes problemas arquitectónicos:

- El `ParticipantesController` accede directamente a `IParticipanteRepository`, saltándose la capa de servicios y violando la Clean Architecture.
- El `EquiposController` depende de la clase concreta `EquipoService` (sin interfaz), violando el Principio de Inversión de Dependencias (DIP).
- Múltiples controllers (`EventosController`, `VotacionesController`, `ProyectosController`, `ComentariosController`) contienen lógica de orquestación que coordina varios servicios, roles de usuario y permisos.
- El método auxiliar `ObtenerUsuarioId()` está duplicado en 4 controllers diferentes.
- La validación de "organizadores no pueden estar en equipos" estaba dispersa entre servicios y controllers.

Estos problemas hacen que los controllers sean difíciles de probar, mantener y extender. Cada cambio en la lógica de negocio requiere modificar múltiples controllers, aumentando el riesgo de errores.

2) Opciones consideradas
Opción A: Dejar la arquitectura tal cual.
- Mantener los controllers como "fat controllers" que orquestan múltiples servicios.
- Aceptar el acoplamiento directo entre controllers y repositorios/clases concretas.

Opción B: Introducir el Patrón de Diseño Facade.
- Crear fachadas en la capa de Aplicación (`Votify.Application/Services/Fachadas/`) que proporcionen una interfaz unificada de alto nivel.
- Cada fachada coordina múltiples servicios para operaciones complejas, delegando la lógica de negocio a los servicios existentes.
- Los controllers pasan a depender únicamente de las fachadas, reduciendo sus dependencias a 1-2 interfaces como máximo.

3) Criterios de decisión
- Cohesión: reducir la dispersión de la lógica de orquestación entre controllers.
- Acoplamiento: eliminar la dependencia directa de controllers hacia repositorios y clases concretas.
- Testabilidad: facilitar la prueba unitaria de los controllers mediante mocks de las fachadas.
- Compatibilidad: garantizar que los patrones existentes (State, Factory, Repository) no se vean afectados.
- Principio de Responsabilidad Única (SRP): que cada controller se limite a recibir peticiones HTTP y devolver respuestas.

4) Decisión tomada
Se elige la Opción B: introducir el Patrón de Diseño Facade en la capa de Aplicación.

Se crean 6 fachadas que agrupan operaciones cohesivas por dominio:

- `EventoFachada` (`IEventoFachada`): gestión completa de eventos, participación y roles.
- `VotacionFachada` (`IVotacionFachada`): gestión de votaciones, transiciones de estado y resultados.
- `VotoFachada` (`IVotoFachada`): emisión de votos estándar y multicriterio.
- `ProyectoFachada` (`IProyectoFachada`): proyectos y comentarios (siempre usados juntos en la UI).
- `EquipoFachada` (`IEquipoFachada`): gestión de equipos con validación de permisos.
- `ParticipanteFachada` (`IParticipanteFachada`): consulta de participantes (corrige el acceso directo a repositorio).

Como prerrequisitos se han creado:
- `IEquipoService`: interfaz para `EquipoService` (corrige la violación DIP).
- `IParticipanteService` + `ParticipanteService`: nuevo servicio que envuelve el repositorio de participantes.
- `EquipoDto`: DTO para transferir datos de equipos entre capas.

5) Consecuencias
Positivas:
- Los 7 controllers pasan a inyectar únicamente fachadas, eliminando dependencias directas a repositorios y clases concretas.
- Se elimina la duplicación del método `ObtenerUsuarioId()` — aunque sigue en cada controller, la lógica de orquestación ya no está dispersa.
- `ParticipantesController` ya no accede directamente a `IParticipanteRepository`.
- `EquiposController` ya no depende de la clase concreta `EquipoService`.
- Las fachadas delegan 100% en los servicios existentes, por lo que los tests unitarios de servicios siguen siendo válidos y ejecutándose (58 tests pasan).
- Los patrones existentes (State, Factory, Repository) permanecen intactos:
  - State Pattern: las fachadas delegan a `VotacionService`, que internamente usa `Votacion.Pausar()`/`Detener()`/`Abrir()` vía `IEstadoVotacion`.
  - Factory Pattern: las fachadas delegan a los services, que usan `VotacionFactory` y `VotoFactory`.
  - Repository Pattern: sin cambios; las fachadas nunca acceden a repositorios directamente.

Negativas / trade-offs:
- Aumenta el número de clases e interfaces en la capa de Aplicación (+6 fachadas, +2 interfaces de servicio, +1 servicio, +1 DTO).
- Añade una capa adicional de indirección que puede parecer innecesaria para operaciones simples de passthrough.
- Requiere actualizar el registro DI en `Program.cs` para incluir las nuevas interfaces y fachadas.

Riesgos y mitigaciones:
- Riesgo: Confusión sobre si un desarrollador debe usar la fachada o el servicio directamente.
  Mitigación: Documentar en el ADR y en el código que los controllers usan fachadas, mientras que los services siguen existiendo para operaciones granulares y testing unitario.
- Riesgo: Las fachadas se convierten en "god classes" si se les añade lógica de negocio.
  Mitigación: Las fachadas actuales son puramente de delegación (pass-through). Cualquier lógica de negocio nueva debe ir en el servicio correspondiente, no en la fachada.

6) Evidencia
- Se han creado las interfaces `IEventoFachada`, `IVotacionFachada`, `IVotoFachada`, `IProyectoFachada`, `IEquipoFachada`, `IParticipanteFachada` en `Votify.Application/Interfaces/`.
- Se han creado las implementaciones `EventoFachada`, `VotacionFachada`, `VotoFachada`, `ProyectoFachada`, `EquipoFachada`, `ParticipanteFachada` en `Votify.Application/Services/Fachadas/`.
- Se han actualizado los 7 controllers (`AuthController` excluido al no tener dependencias de servicio) para inyectar fachadas en lugar de servicios/repositorios directos.
- Se han registrado las nuevas dependencias en `Votify.API/Program.cs`.
- Todos los tests existentes (58) continúan pasando tras la refactorización.
