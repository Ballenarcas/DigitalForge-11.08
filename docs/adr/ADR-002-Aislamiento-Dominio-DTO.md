ADR-002: Aislamiento del Dominio mediante Data Transfer Objects (DTO)
Fecha: 15-04-2026
Sprint: S1
Estado: Propuesto

1) Contexto
El repositorio de Votify contiene `Votify.API`, `Votify.Client`, `Votify.Application` y `Votify.Domain`.
La API REST expone controladores como `ProyectosController`, `VotacionesController`, `ComentariosController` y `VotoController`, mientras que la interfaz Blazor en `Votify.Client` consume DTO definidos en `Votify.Client.DTOs`.

El problema es que exponer entidades de dominio como `Proyecto`, `Votacion`, `Comentario` o `Voto` directamente a la API o al cliente puede filtrar detalles internos del dominio y hacer que la lógica de presentación se mezcle con el modelo de negocio.

2) Opciones consideradas
Opción A: No hacer nada.
- Exponer las entidades de `Votify.Domain` directamente en las respuestas de `Votify.API`.
- Reutilizar el mismo modelo de dominio en la UI y la API.

Opción B: Usar Data Transfer Objects (DTO).
- Crear DTO específicos en `Votify.Application.DTOs` y `Votify.Client.DTOs` para cada contrato de entrada y salida.
- Mapear entre entidades del dominio y DTO dentro de `Votify.Application` o adaptadores dedicados.

Opción C: Usar View Models compartidos entre API y UI.
- Definir modelos de presentación que se reutilicen en la API y la SPA.
- Esta opción puede introducir dependencias directas entre la capa de presentación y la estructura del dominio.

3) Criterios de decisión
- Acoplamiento: desacoplar el dominio de los contratos de API y la UI.
- Seguridad: prevenir fugas de campos internos de la entidad de dominio.
- Extensibilidad: admitir cambios en la UI sin afectar las entidades del dominio.
- Testabilidad: mantener los contratos de API verificables de forma independiente.
- Complejidad: no introducir una carga de mapeo excesiva.

4) Decisión tomada
Se elige la Opción B: aislar el dominio mediante Data Transfer Objects.

Los DTO se usarán como contratos bien definidos entre `Votify.API`, `Votify.Client` y `Votify.Application`. La capa de aplicación será responsable del mapeo entre las entidades de `Votify.Domain` y los DTO, evitando que las entidades internas se expongan directamente fuera del dominio.

5) Consecuencias
Positivas:
- Protege la estructura interna del dominio frente a cambios en la presentación.
- Permite que `Votify.Client` y `Votify.API` evolucionen independientemente.
- Facilita la definición de contratos de API claros y estables.

Negativas / trade-offs:
- Añade mapeo entre entidades y DTO en `Votify.Application`.
- Requiere mantener clases DTO adicionales en la solución.

Riesgos y mitigaciones:
- Riesgo: el mapeo puede volverse repetitivo o inconsistente.
  Mitigación: centralizar mapeos en servicios de aplicación y considerar bibliotecas de mapping cuando sea apropiado.
- Riesgo: los DTO podrían crecer y convertirse en un modelo paralelo de negocio.
  Mitigación: usar DTO solamente para transporte de datos, no para lógica del dominio.

6) Evidencia
- La existencia de `Votify.Application.DTOs` y `Votify.Client.DTOs` en la solución respalda esta decisión.
- El patrón es coherente con la separación de `Votify.API` y `Votify.Client`.
- La evidencia se refleja en el diseño de capas y en los contratos de datos entre proyectos.
