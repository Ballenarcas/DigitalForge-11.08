ADR-007: Uso del Patrón de Diseño Builder para la Construcción de Entidades de Dominio
Fecha: 14-05-2026
Sprint: S3
Estado: Aceptado

1) Contexto
A medida que el dominio de Votify ha crecido, las entidades principales han acumulado constructores con un número elevado de parámetros posicionales, dificultando su instanciación correcta y propiciando errores sutiles. Se han identificado los siguientes problemas:

- `Votacion` (y sus subclases `VotacionEstandar` y `VotacionMulticriterio`) poseen constructores con 8–9 parámetros posicionales. Esto produce un "telescoping constructor" difícil de leer y propenso a errores de ordenamiento.
- `VotacionMulticriterio` presenta un bug latente: su constructor recibe `comentarios` y `comentariosObligatorios` pero los descarta silenciosamente, hardcodeando `true, true` en la llamada a la clase base.
- `VotacionService.CreateEntityFromDto` contiene una lógica de construcción procedural de ~10 pasos (validar fechas, elegir factory, parsear GUID, pasar 8 argumentos posicionales, invocar condicionalmente `Pausar()`). Esta lógica de orquestación de construcción debería estar encapsulada.
- `Proyecto` tiene 6 parámetros posicionales con tipos ambiguos en secuencia (dos `string`, un `string?`, un `Guid`, dos `string?` opcionales), lo que facilita errores de ordenamiento.
- `Evento` tiene 6 parámetros con argumentos opcionales mixtos que reducen la legibilidad en el sitio de llamada.
- En los tests, las entidades se instancian ~15 veces con listas largas de argumentos posicionales idénticos, oscureciendo la intención real de cada test. Además, algunos tests usan `DateTime.Now` en lugar de `DateTime.UtcNow`, causando fallos en máquinas con desplazamiento horario.

Estos problemas dificultan la mantenibilidad, la legibilidad y la testabilidad del código de creación de entidades.

2) Opciones consideradas
Opción A: Mantener los constructores actuales y usar object initializers.
- Requeriría añadir constructores sin parámetros y setters públicos a las entidades de dominio.
- Violaría el principio de inmutabilidad deseado en el dominio y permitiría la creación de entidades en estado inválido.

Opción B: Introducir el Patrón de Diseño Builder.
- Crear builders dedicados en `Votify.Domain/Builders/` que ofrezcan una API fluida para configurar entidades paso a paso.
- Encapsular las reglas de validación y post-construcción (como el auto-pausado) dentro del builder.
- Crear builders de prueba en `Votify.Tests/Builders/` que proporcionen valores seguros por defecto y métodos de conveniencia.

Opción C: Patrón Object Mother.
- Crear clases estáticas que devuelvan instancias preconfiguradas para escenarios comunes.
- Menos flexible que Builder porque no permite personalizar selectivamente atributos sin proliferar métodos de fábrica.

3) Criterios de decisión
- Legibilidad: eliminar listas de 6–9 argumentos posicionales y reemplazarlas por una API fluida auto-documentada.
- Seguridad: centralizar la validación de invariantes en un único punto (`Build()`) antes de que la entidad exista.
- Corrección: eliminar el bug de parámetros descartados en `VotacionMulticriterio`.
- Compatibilidad: no alterar los patrones existentes (Facade, State, Factory, Repository).
- Testabilidad: reducir el ruido en los tests y garantizar que todos los builders de prueba usen `DateTime.UtcNow`.
- Inmutabilidad: las entidades seguirán teniendo propiedades de solo lectura (o setters privados); el builder no expone estado mutable tras `Build()`.

4) Decisión tomada
Se elige la Opción B: introducir el Patrón de Diseño Builder para la construcción de entidades de dominio.

Builders de producción (en `Votify.Domain/Builders/`):
- `VotacionBuilder`: construye `VotacionEstandar` o `VotacionMulticriterio` según el tipo, encapsulando la selección de subclase, la validación de invariantes y el auto-pausado via `Pausar()`.
- `ProyectoBuilder`: construye `Proyecto` eliminando la ambigüedad de ordenamiento de parámetros.
- `EventoBuilder`: construye `Evento` con configuración nominal de parámetros opcionales.

Builders de prueba (en `Votify.Tests/Builders/`):
- `VotacionTestBuilder`: envuelve `VotacionBuilder` con valores por defecto seguros (`UtcNow`, GUIDs válidos, flags `false`).
- `ProyectoTestBuilder` y `EventoTestBuilder`: análogos para sus respectivas entidades.

Reglas de integración con patrones existentes:
- Facade: Sin cambios. Los servicios usan builders internamente; las fachadas permanecen como delegación pura.
- State Pattern: El builder es el único componente autorizado a invocar `Pausar()` tras la construcción. Nunca establece `Estado` directamente.
- Factory Pattern: Coexistencia. Las factories existentes (`VotacionFactory`, `VotacionEstandarFactory`, `VotacionMulticriterioFactory`) se convierten en detalle de implementación interno del `VotacionBuilder`. Se recomienda marcarlas `[Obsolete]` tras la migración completa.
- Repository Pattern: Límite arquitectónico estricto: los builders son puramente en memoria. Nunca acceden a base de datos ni a repositorios.

Cambios de visibilidad:
- Los constructores de `Votacion`, `VotacionEstandar` y `VotacionMulticriterio` pasan a `protected internal` / `internal` para forzar la creación a través del builder (o del builder de prueba).
- `Proyecto` y `Evento` siguen la misma estrategia (`internal`).

5) Consecuencias
Positivas:
- Se elimina el "telescoping constructor" en `Votacion`, `Proyecto` y `Evento`.
- Se corrige el bug de parámetros descartados en `VotacionMulticriterio`.
- Se encapsula la lógica de auto-pausado en el builder, eliminando la manipulación post-construcción en `VotacionService`.
- Los tests ganan claridad: ~80 % del ruido de construcción de entidades desaparece.
- Se centraliza `DateTime.UtcNow` en los builders de prueba, previniendo fallos por zona horaria.
- La API fluida (`ConNombre()`, `ConPeriodo()`, `Build()`) es auto-documentada y reduce errores de ordenamiento.

Negativas / trade-offs:
- Aumenta el número de clases en el dominio y en tests (+3 builders de producción, +3 builders de prueba).
- Los constructores `internal` rompen la compatibilidad con cualquier código externo que instanciara entidades directamente (no se ha detectado ninguno fuera de services y tests).
- Requiere refactorización coordinada de services y tests; no es un cambio localizado.

Riesgos y mitigaciones:
- Riesgo: Un desarrollador usa `new VotacionEstandar(...)` directamente y la compilación falla.
  Mitigación: Documentar en `patronbuilder.md` que toda creación debe usar builders. La compilación fallará explícitamente, forzando la adopción.
- Riesgo: Los builders de prueba se desincronizan con los builders de producción.
  Mitigación: Los test builders delegan en `VotacionBuilder` internamente; solo añaden defaults y conveniencia.
- Riesgo: `Build()` lanza excepciones en tiempo de ejecución si faltan campos obligatorios.
  Mitigación: La validación holística en `Build()` usa mensajes de error claros. Los tests de dominio cubrirán casos de builder inválido.

6) Evidencia
- Se han definido las ubicaciones de los nuevos builders: `Votify.Domain/Builders/` y `Votify.Tests/Builders/`.
- Se ha producido el documento `docs/patronbuilder.md` con la planificación completa de fases, ejemplos de código y reglas de integración.
- Se han identificado las entidades afectadas: `Votacion`, `VotacionEstandar`, `VotacionMulticriterio`, `Proyecto`, `Evento`.
- Se han identificado los servicios a refactorizar: `VotacionService.CreateEntityFromDto`, `ProyectoService.CrearProyectoAsync`, `EventoService.CrearAsync`.
- Se han identificado las suites de tests a refactorizar: `VotacionServiceTests`, `ProyectoServiceTests`, `ComentarioServiceTests`, `DomainEntitiesTests`.
- El plan detalla una secuencia de 3 fases (Votacion → Proyecto/Evento → Resultado DTOs) con pasos específicos por archivo.
