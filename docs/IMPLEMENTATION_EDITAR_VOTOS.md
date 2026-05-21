# Implementación: Editar Votos - Ranking con Asignación Manual

## 📋 Resumen de Cambios

Se ha implementado la funcionalidad completa de "Editar Votos" que permite a los ORGANIZADORES:
- Ver un ranking de resultados en tiempo real
- Asignar manualmente posiciones y votos a proyectos
- Dejar justificación para auditoría
- Marcar resultados como "Manual" o "Auto" en la tabla

## 🔧 Cambios Realizados

### Backend

#### Nuevas Entidades y DTOs
- `ManualVotosAsignacion` (Domain Entity) - Almacena asignaciones manuales
- `ManualVotosAsignacionEntity` (Infrastructure Entity) - Mapeo a BD
- `AsignacionManualVotosDto` - DTO para solicitudes

#### Nuevos Servicios
- `IManualVotosService` interface
- `ManualVotosService` implementación
- `IManualVotosAsignacionRepository` interface
- `ManualVotosAsignacionRepository` implementación

#### Nuevos Endpoints
- `POST /api/votaciones/{id}/resultados/asignar` - Guardar asignación manual
- `GET /api/votaciones/{id}/resultados/manuales` - Obtener asignaciones actuales

#### Modificaciones
- `ResultadoProyectoDto`: Agregados campos `IsManual` y `Justificacion`
- `VotacionesController`: Agregados nuevos endpoints con validación de rol
- `VotifyDbContext`: Agregado DbSet para ManualVotosAsignacion
- `Program.cs`: Registradas inyecciones de dependencia

### Frontend

#### Nuevas Páginas
- `Ranking.razor` - Página completa con:
  - Tabla de resultados (POS, PROYECTO, EQUIPO, VOTOS, MANUAL)
  - Panel de asignación manual (dropdown, campos numéricos, textarea)
  - Validación de acceso (solo ORGANIZADORES)
  - Feedback visual (badges Auto/Manual, selección de filas)

#### Modificaciones en Páginas Existentes
- `Votaciones.razor`: Agregado botón "Editar votos" (solo para ORGANIZADORES, visible cuando votación está activa/pausada/finalizada)

#### Nuevos DTOs Cliente
- `AsignacionManualVotosDto` - DTO para solicitudes al backend
- Extensión de `ResultadoProyectoDto` con campos de asignación manual

#### Extensiones de Servicios
- `VotacionesService`: Agregados métodos `GuardarAsignacionManual()` y `ObtenerAsignacionesManuales()`

## 🗄️ Configuración de Base de Datos

### Tabla Nueva: ManualVotosAsignacion

```sql
CREATE TABLE "ManualVotosAsignacion" (
    id UUID PRIMARY KEY,
    votacion_id UUID NOT NULL,
    proyecto_id UUID NOT NULL,
    posicion_final INTEGER NOT NULL,
    votos_asignados INTEGER NOT NULL,
    justificacion TEXT,
    fecha_creacion TIMESTAMP NOT NULL,
    creado_por VARCHAR(255)
);
```

### Ejecutar Migración

Opción 1: Usar el script SQL directamente (recomendado para Supabase)
```bash
# Ejecutar el archivo de migración en Supabase SQL Editor
docs/migrations/001_add_manual_votos_asignacion.sql
```

Opción 2: Usar EF Core (si está habilitado)
```bash
cd Votify.API
dotnet ef database update
```

## 🔐 Control de Acceso

- Solo usuarios con rol **ORGANIZADOR** pueden:
  - Ver el botón "Editar votos" en la lista de votaciones
  - Acceder a la página de Ranking
  - Crear asignaciones manuales
  - Ver asignaciones existentes

## 🎯 Flujo de Uso

1. **En lista de votaciones**: Usuario ORGANIZADOR ve botón "Editar votos" (junto a "Resultados")
2. **Al hacer clic**: Navega a `/votacion/{id}/editar-votos`
3. **En página Ranking**:
   - Panel izquierdo: Tabla con resultados actuales (Auto/Manual)
   - Panel derecho: Formulario para asignar manualmente
4. **Asignación manual**:
   - Seleccionar proyecto del dropdown
   - Ingresar posición final
   - Ingresar votos a asignar
   - Opcionalmente: agregar justificación
   - Guardar asignación
5. **Confirmación**: Se actualiza la tabla y se marca la fila como "Manual"

## 🧪 Testing

### Casos de Prueba Esenciales

1. **Acceso - Solo Organizadores**
   - [ ] Usuario ORGANIZADOR ve botón "Editar votos"
   - [ ] Usuario JURADO no ve botón
   - [ ] Usuario COMPETIDOR no ve botón
   - [ ] Acceso directo a URL sin permisos muestra error

2. **Tabla de Resultados**
   - [ ] Se cargan correctamente los resultados
   - [ ] Se muestra badge "Auto" o "Manual" según asignación
   - [ ] Las filas seleccionadas se resaltan
   - [ ] Orden es por posición

3. **Formulario de Asignación**
   - [ ] Dropdown se carga con todos los proyectos
   - [ ] Al seleccionar fila, formulario se llena automáticamente
   - [ ] Validación: posición > 0
   - [ ] Validación: proyecto requerido
   - [ ] Guardar sin errores

4. **Persistencia**
   - [ ] Asignación se guarda en BD
   - [ ] Al recargar página, se mantiene la asignación
   - [ ] Justificación se almacena correctamente
   - [ ] Se puede actualizar una asignación existente

## 📁 Archivos Creados/Modificados

### Creados
- `/Votify.Domain/Entities/ManualVotosAsignacion.cs`
- `/Votify.Infrastructure/Persistence/Entities/ManualVotosAsignacionEntity.cs`
- `/Votify.Infrastructure/Repositories/ManualVotosAsignacionRepository.cs`
- `/Votify.Application/Interfaces/IManualVotosService.cs`
- `/Votify.Application/Services/ManualVotosService.cs`
- `/Votify.Domain/Interfaces/IManualVotosAsignacionRepository.cs`
- `/Votify.Client/Pages/Ranking.razor`
- `/Votify.Client/DTOs/AsignacionManualVotosDto.cs`
- `/docs/migrations/001_add_manual_votos_asignacion.sql`

### Modificados
- `/Votify.Application/DTOs/ResultadoProyectoDto.cs` - +2 propiedades
- `/Votify.Application/DTOs/AsignacionManualVotosDto.cs` (nuevo DTO para backend)
- `/Votify.Client/DTOs/ResultadoProyectoDto.cs` - +2 propiedades
- `/Votify.API/Controllers/VotacionesController.cs` - +2 endpoints, inyecciones
- `/Votify.API/Program.cs` - Registradas dependencias
- `/Votify.Client/Pages/Votaciones.razor` - Botón "Editar votos" + método
- `/Votify.Client/Services/VotacionesService.cs` - +2 métodos
- `/Votify.Infrastructure/Persistence/VotifyDbContext.cs` - +DbSet

## 🚀 Próximos Pasos Opcionales

- Agregar historial de cambios de asignación
- Implementar notificaciones cuando se cambian asignaciones
- Agregar filtros/búsqueda en tabla de resultados
- Exportar resultados a Excel/PDF
- Audit log para todas las asignaciones manuales

## ⚠️ Notas Importantes

1. **Migraciones**: Asegúrate de ejecutar la migración SQL antes de usar esta funcionalidad
2. **Permisos**: El control de acceso se valida tanto en frontend como en backend
3. **BD**: La tabla usa índices para optimizar consultas por votación y proyecto
4. **Upsert**: Si existe una asignación para votación+proyecto, se actualiza en lugar de crear duplicada

---

**Fecha de implementación**: 2026-05-21  
**Estado**: ✅ Completo y listo para testing
