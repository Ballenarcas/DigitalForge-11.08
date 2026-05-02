# ADR-004: Uso de Bcrypt y Jwt para las sesiones.

Fecha: 24-04-2026
Sprint: S2
Estado: Aceptada

## 1) Contexto
En el desarrollo del proyecto "Votify", existe la necesidad de implementar un sistema de autenticación y autorización seguro para gestionar el registro y acceso de los usuarios. 
Dado que nuestra arquitectura consta de una SPA web (Votify.Client) y una API REST (Votify.API) independientes, es imperativo contar con un mecanismo para que la API identifique el origen y los permisos de cada petición sin romper la naturaleza *stateless* (sin estado) del servicio. 
Además, por principios básicos de seguridad informática, las contraseñas no deben almacenarse en texto plano ni con métodos débiles en la base de datos.

## 2) Opciones consideradas
**Opción A: Uso de JSON Web Tokens (JWT) para las sesiones y BCrypt para el hashing de contraseñas.**
- JWT es un estándar robusto que permite mantener un backend sin estado, enviando la información de autorización en los encabezados HTTP. 
- BCrypt es una función de “hashing” diseñada específicamente para proteger credenciales. Incorpora un “salt” aleatorio de forma automática y permite ajustar el "costo" computacional del algoritmo, mitigando eficazmente los ataques por fuerza bruta o de diccionario.

**Opción B: Autenticación tradicional con Cookies y sesiones en el servidor, usando PBKDF2/SHA-256.**
- Es el enfoque tradicional donde el servidor guarda el estado de la sesión en memoria o base de datos y envía una Cookie de seguimiento al cliente.
- El ecosistema .NET incluye utilidades nativas (“PasswordHasher” con PBKDF2) preparadas para este propósito.

**Opción C: Delegación a un proveedor de identidad de terceros (Auth0, Azure AD, Firebase Auth).**
- Evitar desarrollar la lógica de autenticación y hashing en nuestra API delegando toda la responsabilidad y el almacenamiento de credenciales a un servicio externo (OAuth2 / OIDC).

## 3) Criterios de decisión
- **Compatibilidad con arquitectura desacoplada:** La solución debe facilitar la comunicación entre una SPA (Blazor WebAssembly) y una API REST de la manera más limpia posible.
- **Escalabilidad y rendimiento:** La API no debe estar fuertemente acoplada al estado de la sesión, facilitando el despliegue de múltiples instancias de la API.
- **Seguridad en el almacenamiento:** Es obligatorio el uso de algoritmos probados contra ataques modernos.
- **Costo y control:** Preferencia por mantener la gestión de usuarios dentro de la plataforma sin incurrir en costos de licenciamiento de proveedores externos, manteniendo control total sobre la base de datos.

## 4) Decisión tomada
Se ha decidido implementar la **Opción A** (JWT + BCrypt). 

Justificación:
- Dado que Votify separa el cliente de la API, JWT es el modelo que mejor se adapta. La API REST verificará las firmas del token de manera independiente sin consultar la base de datos en cada request ni guardar estado en memoria.
- BCrypt sigue siendo un estándar de la industria altamente recomendado para el almacenamiento de contraseñas gracias a su resistencia a la aceleración por hardware (GPUs), complementando bien un flujo de autenticación propio.
- Se descarta la Opción C debido a que en esta etapa del proyecto deseamos mantener el control de los usuarios y no depender de servicios externos o manejar su configuración / facturación.

## 5) Consecuencias

**Positivas:**
- **Backend *Stateless*:** La API gana en escalabilidad horizontal al no depender de memoria para almacenar sesiones.
- **Seguridad robusta:** BCrypt asegura las credenciales en la base de datos de manera altamente confiable.
- **Flexibilidad:** El JWT permite enviar reclamos (*claims*) del usuario directamente en el token (ej: ID del usuario, roles), lo cual ahorra consultas adicionales a la base de datos.

**Negativas / trade-offs:**
- **Revocación de JWT:** A diferencia de las sesiones por servidor, revocar un JWT antes de que expire es complejo. Requiere implementar listas negras (blacklisting) o lógicas complejas no nativas del protocolo.
- **Seguridad en el cliente (SPA):** Almacenar el JWT del lado del cliente (ej. LocalStorage) puede ser vulnerable a ataques XSS (Cross-Site Scripting).

**Riesgos y mitigaciones:**
- **Riesgo (Robo de token XSS):** Mitigación propensando el uso de un tiempo de vida (expiration/TTL) muy corto para el token JWT complementado con *Refresh Tokens* almacenados de forma segura (por ejemplo, en cookies `HttpOnly`).
- **Riesgo (Carga de CPU por BCrypt):** BCrypt es lento por diseño. Mitigación: balancear el factor de trabajo (Work Factor / Salting rounds) para que sea seguro pero no bloquee de forma perceptible el inicio de sesión de los usuarios bajo alta concurrencia. Un valor entre 10 y 12 es el estándar aceptable.

## 6) Evidencia
- **Controlador de Autenticación:** Implementación de endpoints expuestos en `Votify.API/Controllers/AuthController.cs`.
- **Lógica de Generación de JWT y Validación (Bcrypt):** Definida en el contrato `Votify.Application/Interfaces/IAuthService.cs` y su respectiva implementación en la capa de servicios (`Votify.Application/Services/...`).
- **Configuración de JWT en la API:** Inyección de dependencias y configuración del esquema *Bearer* en `Votify.API/Program.cs` leyendo desde `Votify.API/appsettings.json`.