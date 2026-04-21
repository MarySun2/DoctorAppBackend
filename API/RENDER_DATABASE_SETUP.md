# Configuración de base de datos para Render

Este backend quedó ajustado para poder funcionar con **PostgreSQL en Render** sin cambiar la lógica principal de la app.

## Variables de entorno en Render

Crea estas variables en tu servicio web:

- `DatabaseProvider` = `PostgreSQL`
- `ConnectionStrings__DefaultConnection` = tu cadena de conexión PostgreSQL completa
- `TokenKey` = tu clave JWT
- `ASPNETCORE_ENVIRONMENT` = `Production`

## Cadena de conexión ejemplo

```text
Host=dpg-xxxx.render.com;Port=5432;Database=doctorapp;Username=doctorapp_user;Password=tu_password;SSL Mode=Require;Trust Server Certificate=true
```

## Desarrollo local con SQL Server

No se rompió el modo local: puedes seguir usando `appsettings.Development.json` con SQL Server.

## Nota técnica

- En PostgreSQL, al iniciar en Render, el backend usa `EnsureCreated()` para crear las tablas si no existen.
- En SQL Server, sigue usando migraciones (`Migrate()`).
