ADR-003: Separación física de la Interfaz de Usuario (SPA) y la API REST
Fecha: 15-04-2026
Sprint: S1
Estado: Propuesto

1) Contexto
La solución de Votify agrupa proyectos distintos: `Votify.Client` como SPA Blazor y `Votify.API` como backend ASP.NET Core.
Aunque ambos proyectos conviven en la misma solución `Votify.sln`, es necesario decidir si deben ser tratados y desplegados como unidades independientes.

El problema es asegurar una arquitectura que soporte despliegues separados, desarrollo paralelo y escalado independiente sin perder coherencia en la experiencia del usuario.

2) Opciones consideradas
Opción A: Unificar SPA y API en un único proyecto.
- Servir `Votify.Client` desde el mismo host y proyecto que `Votify.API`.
- Compartir configuración y despliegue en una sola aplicación.

Opción B: Separar físicamente la SPA y la API.
- Mantener `Votify.Client` y `Votify.API` como proyectos distintos con despliegues independientes.
- El cliente consume la API mediante llamadas HTTP explícitas.

Opción C: Mantener la SPA independiente pero con un proxy de desarrollo integrado.
- Usar un proxy local para que la SPA acceda a `Votify.API` durante el desarrollo.
- Requiere configuración adicional de rutas, CORS y entornos.

3) Criterios de decisión
- Despliegue: permitir que `Votify.Client` y `Votify.API` se actualicen y escalen por separado.
- Independencia de desarrollo: habilitar trabajo paralelo en frontend y backend.
- Acoplamiento: minimizar dependencias entre la entrega de la UI y el backend.
- Complejidad operativa: evaluar el coste de administrar proyectos o artefactos separados.
- Experiencia de usuario: garantizar que la SPA consume la API de forma consistente.

4) Decisión tomada
Se elige la Opción B: separar físicamente la SPA y la API.

Esto significa que `Votify.Client` y `Votify.API` permanecen como proyectos distintos y se despliegan como aplicaciones separadas. La UI de Blazor consume los endpoints de `Votify.API`, mientras que `Votify.API` es responsable del dominio y el acceso a datos.

5) Consecuencias
Positivas:
- Facilita despliegues independientes de interfaz y backend.
- Permite que equipos o tareas de desarrollo se enfoquen en `Votify.Client` o `Votify.API` sin interferencia.
- Simplifica la adopción de herramientas y pipelines específicos para cada proyecto.

Negativas / trade-offs:
- Requiere gestionar CORS y configuración de rutas entre la SPA y la API.
- Incrementa el número de artefactos y servicios a desplegar.

Riesgos y mitigaciones:
- Riesgo: configuración incorrecta de CORS o endpoints entre `Votify.Client` y `Votify.API`.
  Mitigación: establecer políticas de CORS claras y entornos de desarrollo bien definidos.
- Riesgo: mayor complejidad de despliegue al operar dos aplicaciones separadas.
  Mitigación: documentar el flujo de despliegue y automatizar con scripts o pipelines.

6) Evidencia
- La estructura actual del repositorio ya separa `Votify.Client` y `Votify.API` en proyectos distintos.
- La decisión se apoya en la arquitectura existente de la solución `Votify.sln`.
- El diseño es consistente con la separación de la interfaz de usuario y el backend en una SPA + API REST.
