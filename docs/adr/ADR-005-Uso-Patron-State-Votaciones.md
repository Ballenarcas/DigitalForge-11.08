ADR-005: Uso del Patrón de Diseño State para el Ciclo de Vida de las Votaciones
Fecha: 06-05-2026
Sprint: S2
Estado: Aceptado

1) Contexto
En el dominio de Votify, la entidad central `Votacion` pasa por diferentes fases o estados a lo largo de su ciclo de vida (como Activa, Pausada y Finalizada). Dependiendo del estado en el que se encuentre la votación, las reglas de negocio cambian drásticamente. Por ejemplo:
- Solo se pueden registrar votos si la votación está en estado "Activa".
- No se pueden añadir nuevos participantes si la votación está "Finalizada".
- Una votación "Pausada" no puede recibir votos pero puede reanudarse.

El problema es que gestionar estas reglas de transición y validación utilizando un simple enumerado (`enum`) y múltiples sentencias condicionales (`if` o `switch`) dentro de la entidad `Votacion` o en los servicios de aplicación resultaría en un código frágil, difícil de mantener y propenso a errores al añadir nuevos estados o reglas en el futuro.

2) Opciones consideradas
Opción A: Uso de Enums y lógica condicional.
- Añadir una propiedad de tipo `enum EstadoVotacion` en la entidad.
- Implementar la lógica de negocio y validaciones mediante bloques `switch` o `if` en los métodos de los servicios (`VotacionService`) o en la propia entidad.

Opción B: Introducir el Patrón de Diseño State.
- Definir una interfaz `IEstadoVotacion` que declare los comportamientos permitidos según el estado.
- Crear clases concretas para cada estado (`EstadoActiva`, `EstadoPausada`, `EstadoFinalizado`) que encapsulen sus propias reglas.
- La entidad `Votacion` mantiene una referencia a su estado actual y delega en él las validaciones.

3) Criterios de decisión
- Mantenibilidad: facilitar la comprensión y modificación del ciclo de vida de la votación.
- Extensibilidad (OCP): permitir añadir nuevos estados en el futuro sin modificar el código de los estados existentes ni el de la entidad `Votacion`.
- Responsabilidad Única (SRP): encapsular el comportamiento específico de cada estado en su propia clase en lugar de tener una clase gigante con todas las reglas.
- Complejidad: evaluar si la creación de múltiples clases compensa la eliminación de lógica condicional compleja.

4) Decisión tomada
Se elige la Opción B: introducir el Patrón de Diseño State en la capa de Dominio.

Esto significa que el comportamiento de la votación en cada momento es dictado por un objeto polimórfico de tipo `IEstadoVotacion`. Si se intenta realizar una acción no permitida (como votar estando pausada), el estado actual es el responsable de rechazar la acción, manteniendo la lógica puramente dentro de la capa de Dominio.

5) Consecuencias
Positivas:
- Cumplimiento estricto del Principio Abierto/Cerrado (OCP) y el Principio de Responsabilidad Única (SRP).
- La entidad `Votacion` queda mucho más limpia y cohesiva.
- Elimina los "code smells" de métodos largos con múltiples sentencias `switch/case`.

Negativas / trade-offs:
- Aumenta la cantidad de clases y archivos en el dominio.
- Requiere un manejo especial para la persistencia en base de datos con Entity Framework Core (ya que EF no guarda objetos de estado directamente, se necesita mapear el tipo de estado a un valor simple y reconstruirlo al consultar).

Riesgos y mitigaciones:
- Riesgo: Dificultad para persistir y recuperar el estado polimórfico desde la base de datos con el ORM.
  Mitigación: Implementar Value Conversions (conversiones de valor) en la configuración de Entity Framework (`VotifyDbContext` o `IEntityTypeConfiguration`) para traducir el objeto de estado a una cadena o entero en base de datos y viceversa.

6) Evidencia
- Se han creado las clases `IEstadoVotacion`, `EstadoActiva`, `EstadoPausada` y `EstadoFinalizado` dentro del directorio `Votify.Domain/Estado/`.
- La lógica de transición y validación de las votaciones ya hace uso de este patrón en el dominio.