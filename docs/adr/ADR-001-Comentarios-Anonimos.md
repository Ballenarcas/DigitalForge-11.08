ADR-001: Abstracción de Persistencia mediante el Patrón Repositorio
Fecha: 15-04-2026
Sprint: S1
Estado: Propuesto

1) Contexto
El repositorio de Votify está estructurado en proyectos claramente separados: `Votify.API` expone una API REST, `Votify.Client` es una SPA Blazor, `Votify.Application` contiene servicios de aplicación y `Votify.Domain` define las entidades centrales.
En el dominio de Votify existen entidades como `Proyecto`, `Votacion`, `Comentario` y `Voto` que se almacenan a través de la infraestructura de datos.

El problema es que los servicios de aplicación de `Votify.Application` no deben depender directamente de detalles de persistencia implementados en `Votify.Infrastructure`.
Si los servicios acceden directamente a la base de datos o a consultas específicas del ORM, se deteriora la capacidad de prueba y la separación entre la lógica de negocio y la infraestructura.

2) Opciones consideradas
Opción A: No hacer nada.
- Mantener acceso directo a la base de datos desde los servicios en `Votify.Application`.
- Usar entidades de dominio y consultas EF Core dentro de servicios como `ProyectoService`, `VotacionService`, `ComentarioService` y `VotoService`.

Opción B: Introducir una abstracción de repositorio.
- Definir interfaces de repositorio en `Votify.Application.Interfaces` o `Votify.Domain.Interfaces`.
- Implementar repositorios concretos en `Votify.Infrastructure.Persistence.Repositories`.

Opción C: Usar el patrón Unidad de Trabajo además de repositorios.
- Encapsular transacciones y repositorios en una unidad de trabajo compartida.
- Esto agrega complejidad adicional en la infraestructura y la configuración del contexto de datos.

3) Criterios de decisión
- Mantenibilidad: mantener la lógica de negocio de `Votify.Application` independiente de `Votify.Infrastructure`.
- Testabilidad: habilitar pruebas unitarias de servicios con repositorios simulados.
- Claridad arquitectónica: preservar la separación de responsabilidades entre dominio, aplicación e infraestructura.
- Complejidad: elegir una solución de infraestructura controlada para el alcance actual.
- Flexibilidad: permitir cambiar el mecanismo de persistencia sin refactorizar la lógica de negocio.

4) Decisión tomada
Se elige la Opción B: introducir una abstracción de repositorio mediante interfaces y sus implementaciones concretas.

Esto significa que `Votify.Application` trabaja con contratos de repositorio alojados en interfaces, mientras que `Votify.Infrastructure` provee las implementaciones basadas en EF Core u otra tecnología de datos. Así, los servicios de aplicación no dependen directamente de los detalles de `Votify.Infrastructure`.

5) Consecuencias
Positivas:
- Refuerza la separación entre `Votify.Application` y `Votify.Infrastructure`.
- Facilita la creación de pruebas unitarias para `ProyectoService`, `VotacionService`, `ComentarioService` y `VotoService`.
- Hace más evidente el rol de cada proyecto dentro de la solución.

Negativas / trade-offs:
- Requiere definir y mantener interfaces adicionales en la arquitectura.
- Introduce código de infraestructura extra en `Votify.Infrastructure` para cada repositorio.

Riesgos y mitigaciones:
- Riesgo: las interfaces de repositorio pueden volverse demasiado genéricas o acopladas a EF Core.
  Mitigación: diseñar interfaces específicas para las operaciones requeridas por el dominio.
- Riesgo: repetición de métodos entre repositorios.
  Mitigación: compartir contratos comunes solo donde sea necesario y evitar sobre-abstraer.

6) Evidencia
- La estructura actual del repositorio ya separa la capa de aplicación de la infraestructura.
- El patrón de repositorio encaja con los proyectos `Votify.Application` y `Votify.Infrastructure` existentes.
- La evidencia concreta se demostrará mediante la creación de interfaces y clases de repositorio dentro de los proyectos correspondientes.
