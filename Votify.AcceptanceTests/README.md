# Pruebas de Aceptación - Votify

Este directorio contiene las **pruebas de aceptación** para la aplicación Votify, enfocadas en validar que las funcionalidades críticas funcionan correctamente desde una perspectiva de usuario.

## 📋 Estructura

```
Votify.AcceptanceTests/
├── Features/
│   └── VotarAcceptanceTests.cs       # Pruebas de aceptación para la actividad de votar
├── Helpers/
│   ├── AcceptanceTestBase.cs         # Clase base con BD en memoria
│   └── TestDataFactory.cs            # Factory para crear datos de prueba
└── Votify.AcceptanceTests.csproj    # Proyecto de pruebas
```

## 🎯 Funcionalidad Probada: Actividad de Votar

### Casos de Éxito ✅

1. **Votar_UsuarioEstandarEnVotacionEstandar_DebeRegistrarVotoExitosamente**
   - Valida que un usuario estándar puede votar en una votación
   - El voto se registra con la identidad del votante
   - **Relación con la historia de usuario**: "Como votante, quiero votar en una votación para expresar mi preferencia"

2. **Votar_UsuarioAnonimoEnVotacionAnonima_DebeRegistrarVotoExitosamente**
   - Valida que un usuario anónimo puede votar sin revelar identidad
   - El voto se registra sin información de votante
   - **Criterio de aceptación**: El sistema permite votos anónimos cuando VotanteId es null

3. **Votar_UsuarioPuedeMúltiplesVotosHastaLimite_DebeRegistrarTodos**
   - Valida que un usuario puede votar múltiples veces hasta alcanzar el límite
   - La votación tiene un `LimiteProyectos` que define cuántos votos puede hacer un usuario
   - **Criterio de aceptación**: Usuario con límite 2 puede votar 2 veces

4. **Votar_ConsultarResultadosPorVotacion_DebeRetornarVotosOrdenados**
   - Valida que se pueden obtener los resultados de una votación
   - Los proyectos se ordenan por cantidad de votos (descendente)
   - **Criterio de aceptación**: Un proyecto con 3 votos aparece antes que uno con 1 voto

### Casos de Error ❌

5. **Votar_CuandoSeAlcanzaLimite_LanzaInvalidOperationException**
   - Valida que NO se permite votar si se alcanzó el límite
   - Lanza `InvalidOperationException` con mensaje descriptivo
   - **Criterio de error**: "No puedes votar. Has alcanzado el límite de X votos"

6. **Votar_ConVotacionInexistente_LanzaArgumentException**
   - Valida que NO se puede votar en una votación que no existe
   - Lanza `ArgumentException`
   - **Criterio de error**: "La votación especificada no existe"

7. **Votar_VariosUsuariosAnonimosPuedenVotar_SinConflictos**
   - Valida que múltiples usuarios anónimos pueden votar en la misma votación
   - Cada voto anónimo es independiente (VotanteId nulo)
   - **Criterio de aceptación**: 3 usuarios anónimos pueden registrar 3 votos diferentes

## 🧪 Cómo Ejecutar las Pruebas

### Ejecutar todas las pruebas de aceptación
```bash
dotnet test Votify.AcceptanceTests/Votify.AcceptanceTests.csproj
```

### Ejecutar una prueba específica
```bash
dotnet test Votify.AcceptanceTests/Votify.AcceptanceTests.csproj --filter "MethodName=Votar_UsuarioEstandarEnVotacionEstandar_DebeRegistrarVotoExitosamente"
```

### Ejecutar con verbosidad detallada
```bash
dotnet test Votify.AcceptanceTests/Votify.AcceptanceTests.csproj --verbosity detailed
```

## 📊 Resultados

**Estado**: ✅ **TODAS LAS PRUEBAS PASAN**

```
Resumen de pruebas: total: 7; con errores: 0; correcto: 7; omitido: 0
```

## 🏗️ Arquitectura de las Pruebas

### Base de Datos en Memoria
- Se utiliza `DbContext` con proveedor `InMemoryDatabase`
- Cada prueba obtiene su propia BD aislada
- Los datos persisten durante la prueba y se limpian al finalizar

### Factory Pattern para Datos de Prueba
```csharp
// Crear votación de prueba
var votacion = TestDataFactory.CrearVotacionEstandar(
    nombre: "Votación Test",
    limiteProyectos: 2
);

// Crear proyecto de prueba
var proyecto = TestDataFactory.CrearProyecto(
    nombre: "Proyecto A"
);
```

### Inicialización Automática con IAsyncLifetime
```csharp
public abstract class AcceptanceTestBase : IAsyncLifetime
{
    public virtual async Task InitializeAsync()  // Ejecuta ANTES de cada prueba
    public virtual async Task DisposeAsync()     // Ejecuta DESPUÉS de cada prueba
}
```

## 🔄 Flujo de Votación Validado

```
┌─────────────────────────────────────────────────────┐
│ Usuario intenta votar (VotarDto)                    │
├─────────────────────────────────────────────────────┤
│ 1. ¿Votación existe?                                │
│    └─ NO → ArgumentException                        │
├─────────────────────────────────────────────────────┤
│ 2. ¿Usuario alcanzó límite de votos?                │
│    └─ SÍ → InvalidOperationException                │
├─────────────────────────────────────────────────────┤
│ 3. Crear voto (Estándar o Anónimo según VotanteId) │
├─────────────────────────────────────────────────────┤
│ 4. Guardar en Base de Datos                         │
├─────────────────────────────────────────────────────┤
│ 5. ✅ Voto registrado exitosamente                   │
└─────────────────────────────────────────────────────┘
```

## 📝 Dependencias

```xml
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
```

## 🚀 Próximas Pruebas de Aceptación

Se pueden agregar pruebas adicionales para:
- ✅ Crear votación (CrearVotacion)
- ✅ Gestionar comentarios (CrearComentario)
- ✅ Consultar proyectos (ObtenerProyectos)
- ✅ Editar votación (EditarVotacion)

## 📖 Recursos

- [xUnit Documentation](https://xunit.net/docs/getting-started)
- [Entity Framework Core - InMemory Provider](https://learn.microsoft.com/en-us/ef/core/testing/)
- [Acceptance Test Driven Development (ATDD)](https://en.wikipedia.org/wiki/Acceptance_test%E2%80%93driven_development)
