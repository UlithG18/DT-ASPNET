# DT-ASPNET — Plataforma de Rentas Cortas

API REST para gestión de rentas cortas con validación de identidad mediante IA, sistema de notificaciones omnicanal y dashboard de rendimiento para propietarios.

---

## Requisitos previos

| Herramienta | Versión mínima |
|---|---|
| Docker | 24.x |
| Docker Compose | 2.x (incluido en Docker Desktop) |
| .NET SDK | 10.0 (solo si corres fuera de Docker) |
| dotnet-ef | 10.x (solo para migraciones manuales) |

---

## Variables de entorno

Crea un archivo `.env` en la raíz del proyecto (al lado de `docker-compose.yml`):

```env
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxx
```

> ⚠️ Este archivo **no debe subirse al repositorio**. Ya está en `.gitignore`.

Para el envío de emails, edita `src/Api/appsettings.json` y completa la sección `Email` con tus credenciales SMTP (Gmail, Mailgun, etc.).

---

## Levantar el proyecto con Docker

```bash
# 1. Clonar el repositorio
git clone <url-del-repo>
cd DT-ASPNET

# 2. Crear el archivo de variables de entorno
echo "OPENAI_API_KEY=sk-proj-tu-clave-aqui" > .env

# 3. Construir y levantar todos los servicios
docker compose up --build
```

La primera vez Docker descarga las imágenes base y compila el proyecto — puede tomar 2-3 minutos.

Una vez levantado:

| Servicio | URL |
|---|---|
| API + Swagger | http://localhost:8081/swagger |
| PostgreSQL | localhost:5433 |

---

## Migraciones de base de datos

Las migraciones deben correrse **una sola vez** después de levantar Docker por primera vez, desde la raíz del proyecto:

```bash
# Instalar dotnet-ef si no lo tienes
dotnet tool install --global dotnet-ef

# Crear la migración inicial
dotnet ef migrations add Init \
  --project src/Infrastructure \
  --startup-project src/Api

# Aplicar a la base de datos
dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/Api
```

> El `appsettings.json` ya apunta a `localhost:5433` que es el puerto expuesto por el contenedor de Postgres.

---

## Comandos útiles

```bash
# Ver logs en tiempo real
docker compose logs -f api

# Detener todo
docker compose down

# Detener y borrar la base de datos (volumen)
docker compose down -v

# Reconstruir solo la API (tras cambios en código)
docker compose up --build api
```

---

## Flujo de prueba rápida en Swagger

1. `POST /api/auth/register` — crear usuario
2. `POST /api/auth/login` — obtener `accessToken`
3. Click en **Authorize** 🔒 → escribir `Bearer {accessToken}` → Authorize
4. `POST /api/users/me/become-owner` — habilitar rol de propietario
5. `POST /api/properties` — publicar un inmueble
6. `POST /api/kyc` — subir foto de documento (requerido antes de reservar)
7. `POST /api/reservations` — crear reserva
8. `GET /api/reports/excel` — descargar reporte en Excel (como propietario)

---

## Arquitectura

El proyecto sigue **Clean Architecture** dividida en cuatro capas:

```
src/
├── Domain/          # Entidades, interfaces de repositorios, enums
├── Application/     # Casos de uso, servicios, interfaces de servicios, DTOs
├── Infrastructure/  # EF Core, repositorios, Email (MailKit), KYC (OpenAI)
└── Api/             # Controllers, Program.cs, configuración
```

### Decisiones técnicas clave

**Anti double-booking**
La validación de solapamiento se hace con una consulta atómica en `ReservationRepository.HasOverlapAsync` usando lógica de intervalos: una reserva conflicta si `CheckIn < nuevaCheckOut AND CheckOut > nuevoCheckIn`. Solo se evalúan reservas no canceladas.

**Horarios estandarizados**
El factory method `Reservation.Create()` en el dominio normaliza automáticamente check-in a las 14:00 UTC y check-out a las 12:00 UTC, sin importar lo que envíe el cliente. Es imposible crear una reserva con horarios distintos.

**KYC con IA**
Se usa GPT-4o (Vision) de OpenAI para extraer datos del documento de identidad (nombres, apellidos, número de documento, fecha de nacimiento). El archivo de imagen **nunca se persiste en disco ni en base de datos** — se procesa en memoria y se descarta. Solo se almacenan los datos extraídos, cumpliendo el requerimiento de privacidad.

**Autenticación diferida**
Los endpoints de búsqueda y detalle de propiedades (`GET /api/properties`) son públicos. El login se exige únicamente al intentar reservar, guardar en wishlist o acceder al perfil — reduciendo la fricción para usuarios explorando el catálogo.

**Notificaciones omnicanal**
Cada evento clave (reserva confirmada, cancelación) dispara simultáneamente una notificación in-app (persistida en BD, consultable vía `GET /api/notifications`) y un email HTML via MailKit. La interfaz `IEmailService` vive en Application, manteniendo la inversión de dependencias — Infrastructure implementa, Application consume.

**JWT + Refresh Token**
Access token con expiración de 2 horas. Refresh token de 30 días almacenado en BD, renovable via `POST /api/auth/refresh` sin necesidad de re-autenticarse.

**Export Excel**
ClosedXML genera el reporte en memoria como `byte[]` y se devuelve como descarga directa. Soporta filtro por propiedad y rango de fechas. El archivo incluye encabezados formateados, datos del huésped, inmueble y totales calculados.

**Fotos de inmuebles**
Se almacenan como `text[]` nativo de PostgreSQL — sin tabla intermedia para el MVP. El propietario provee URLs de imágenes ya alojadas externamente.

### Stack tecnológico

| Capa | Tecnología |
|---|---|
| Core | .NET 10 / ASP.NET Core |
| ORM | Entity Framework Core 10 + Npgsql |
| Base de datos | PostgreSQL 16 |
| Autenticación | JWT Bearer |
| Hash de contraseñas | BCrypt.Net-Next |
| KYC / IA | OpenAI GPT-4o Vision |
| Email | MailKit / MimeKit |
| Excel | ClosedXML |
| Documentación | Swashbuckle (Swagger UI) |
| Contenedores | Docker + Docker Compose |

---

## Estructura de endpoints

### Auth
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/auth/register` | ❌ | Registro de usuario |
| POST | `/api/auth/login` | ❌ | Login, devuelve tokens |
| POST | `/api/auth/refresh` | ❌ | Renovar access token |

### Usuarios
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/users/me` | ✅ | Ver perfil |
| PUT | `/api/users/me` | ✅ | Editar perfil |
| POST | `/api/users/me/become-owner` | ✅ | Activar rol propietario |

### Propiedades
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/properties` | ❌ | Buscar por ciudad y fechas |
| GET | `/api/properties/{id}` | ❌ | Detalle de inmueble |
| GET | `/api/properties/mine` | ✅ | Mis propiedades |
| POST | `/api/properties` | ✅ | Publicar inmueble |
| PUT | `/api/properties/{id}` | ✅ | Editar inmueble |

### Reservas
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/reservations` | ✅ | Mis reservas |
| POST | `/api/reservations` | ✅ | Crear reserva (requiere KYC) |
| DELETE | `/api/reservations/{id}` | ✅ | Cancelar reserva |
| GET | `/api/reservations/property/{id}` | ✅ | Reservas de mi propiedad |

### KYC
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/kyc` | ✅ | Subir documento de identidad |

### Wishlist
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/wishlist` | ✅ | Ver favoritos |
| POST | `/api/wishlist/{propertyId}` | ✅ | Agregar/quitar favorito |

### Notificaciones
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/notifications` | ✅ | Ver notificaciones |
| PATCH | `/api/notifications/{id}/read` | ✅ | Marcar como leída |

### Reportes
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| GET | `/api/reports/excel` | ✅ | Descargar Excel de reservas |