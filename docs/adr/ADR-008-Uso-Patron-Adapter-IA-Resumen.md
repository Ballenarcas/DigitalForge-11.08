ADR-008: Uso del Patron de Diseno Adapter para Integracion con API de IA Externa
Fecha: 15-05-2026
Sprint: S4
Estado: Propuesto

1) Contexto
Los competidores de Votify reciben multiples comentarios de diferentes jurados y organizadores sobre sus proyectos. Actualmente, estos comentarios se muestran como una lista individual sin consolidacion. Se necesita una funcionalidad que unifique todos los comentarios en un solo resumen de texto, facilitando al competidor la comprension del feedback recibido.

La integracion con una API de IA externa (ej. OpenAI) presenta los siguientes desafios:
- La API externa puede no estar disponible (caidas, limites de tasa, timeouts).
- El formato de request/response de la API es especifico del proveedor y puede cambiar.
- Se necesita un mecanismo de fallback para cuando la IA no este disponible.
- La aplicacion debe funcionar correctamente en desarrollo sin acceso a la API.
- La capa de aplicacion no debe conocer detalles internos del proveedor de IA.

2) Opciones consideradas
Opcion A: Llamada directa al API de IA desde los servicios.
- Acoplar la logica de IA directamente en ComentarioService o ProyectoFachada.
- Simple de implementar pero viola el Principio de Responsabilidad Unica.
- Dificil de probar, mantener y cambiar de proveedor.
- Sin fallback: si la API falla, la funcionalidad queda rota.

Opcion B: Patron Adapter con interfaz en Domain y adaptadores en Infrastructure.
- Definir IAICommentSummarizer en Votify.Domain/Interfaces/.
- Implementar AIClientAdapter en Votify.Infrastructure/Adapters/ (Adapter).
- Implementar FallbackCommentSummarizer como alternativa segura (Null Object).
- Implementar ResilientCommentSummarizer como decorador (Decorator).
- Registrar en DI con Polly para reintentos y circuit breaker.
- Seguir la convencion del proyecto: interfaces en Domain, implementaciones en Infrastructure.

Opcion C: Patron Strategy con multiples proveedores.
- Definir multiples implementaciones de IAICommentSummarizer (OpenAI, Azure AI, LLM local).
- Seleccionar el proveedor via DI o configuracion.
- Mayor complejidad innecesaria en este momento; la Opcion B ya permite cambiar de proveedor facilmente.

3) Criterios de decision
- Desacoplamiento: la capa de aplicacion no debe conocer detalles de la API externa.
- Resiliencia: el sistema debe funcionar incluso cuando la IA no este disponible.
- Testabilidad: poder probar la logica de resumen sin llamar a la API real.
- Extensibilidad: poder cambiar de proveedor de IA sin modificar la logica de negocio.
- Consistencia: seguir los patrones existentes (Repository, Facade, State, Factory).
- Simplicidad: evitar complejidad innecesaria en la primera iteracion.

4) Decision tomada
Se elige la Opcion B: Patron Adapter con fallback y decorador de resiliencia.

Componentes:
- IAICommentSummarizer (Target): interfaz en Domain/Interfaces/. Define el contrato que la aplicacion espera.
- AIClientAdapter (Adapter): adaptador en Infrastructure/Adapters/ que envuelve la API externa de IA. Transforma los comentarios de Votify al formato del proveedor y mapea la respuesta.
- FallbackCommentSummarizer (Null Object): implementacion que produce resumenes concatenados simples. Siempre disponible, nunca falla.
- ResilientCommentSummarizer (Decorator): orquesta AI + fallback con manejo de errores. Si el adapter falla, delega al fallback automaticamente.
- AISummarizerOptions: configuracion tipada para URL, API key, modelo, timeouts y habilitacion.
- Polly: reintentos con backoff exponencial y circuit breaker para llamadas HTTP.

Reglas de integracion con patrones existentes:
- Repository Pattern: IAICommentSummarizer sigue la misma convencion que IComentarioRepository. Interfaz en Domain, implementacion en Infrastructure.
- Facade Pattern: ProyectoFachada se extiende con ObtenerResumenComentariosAsync(), orquestando la obtencion de comentarios y la llamada al adapter.
- Factory Pattern: La seleccion del adaptador (IA vs fallback) se resuelve en DI via Program.cs, no en runtime.
- State Pattern: Sin impacto. El resumen de comentarios no depende del estado de la votacion.

Configuracion:
- AISummarizer.Enabled = false por defecto. La app funciona sin IA configurada.
- Variables de entorno AI_SUMMARIZER_BASE_URL y AI_SUMMARIZER_API_KEY para secretos en produccion.
- Model, MaxTokens, TimeoutSeconds configurables via appsettings.json.

5) Consecuencias
Positivas:
- La capa de aplicacion (ProyectoFachada) depende solo de IAICommentSummarizer, no de detalles de la API.
- Se puede cambiar de proveedor de IA creando un nuevo adaptador sin modificar la logica de negocio.
- El sistema funciona correctamente sin IA (FallbackCommentSummarizer produce resumenes simples).
- Los tests unitarios pueden mockear IAICommentSummarizer facilmente.
- La resiliencia (reintentos, circuit breaker) esta centralizada en la configuracion DI.
- El patron Adapter es familiar para el equipo (ya usan Repository, Factory, Facade, State).
- El fallback automatico garantiza que el usuario siempre vea un resumen, sin importar el estado de la API.

Negativas / trade-offs:
- Aumenta la complejidad con 3 nuevas clases de adaptador + 1 decorador + configuracion.
- Requiere anadir Polly como dependencia al proyecto API.
- El fallback produce resumenes menos utiles que la IA (concatenacion vs. analisis semantico).
- La construccion del prompt esta hardcodeada en AIClientAdapter; si se necesitan prompts mas complejos, habra que refactorizar.

Riesgos y mitigaciones:
- Riesgo: Un desarrollador olvida configurar la API y espera resumenes de IA.
  Mitigacion: Enabled = false por defecto. El badge en la UI indica "Resumen simple" vs "Generado por IA".
- Riesgo: La API de IA cambia su formato de response.
  Mitigacion: ParseResponse() esta aislado en AIClientAdapter. Solo hay que modificar un metodo.
- Riesgo: Costo inesperado de llamadas a la API en produccion.
  Mitigacion: AISummarizerOptions.Enabled permite desactivar sin cambiar codigo. Polly limita reintentos.
- Riesgo: Timeout largos degradan la experiencia del usuario.
  Mitigacion: TimeoutSeconds configurable (default 30s). El fallback es instantaneo.

6) Evidencia
- Se han definido las ubicaciones de los nuevos archivos: Votify.Domain/Interfaces/, Votify.Infrastructure/Adapters/, Votify.Infrastructure/Configuration/.
- Se ha producido el documento docs/patronadapter.md con la guia completa de implementacion (10 pasos).
- Se han identificado los archivos a crear: IAICommentSummarizer.cs, AIClientAdapter.cs, FallbackCommentSummarizer.cs, ResilientCommentSummarizer.cs, AISummarizerOptions.cs, ComentarioResumenItem.cs, ResumenComentarioDto.cs.
- Se han identificado los archivos a modificar: IProyectoFachada.cs, ProyectoFachada.cs, ComentariosController.cs, Program.cs, ProyectosService.cs (Client), Comentarios.razor.
- Se han identificado los paquetes NuGet necesarios: Microsoft.Extensions.Http.Polly, Polly.
- Se han identificado los tests a crear: AIClientAdapterTests.cs, FallbackCommentSummarizerTests.cs.
- El patron sigue la misma estructura que el Repository Pattern ya establecido en el proyecto.
