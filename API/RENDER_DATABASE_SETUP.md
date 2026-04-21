# Render + PostgreSQL

## Variables de entorno en Render

- `DatabaseProvider=PostgreSQL`
- `ConnectionStrings__DefaultConnection=Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true`
- `TokenKey=tu_clave_larga`
- `ASPNETCORE_ENVIRONMENT=Production`

## Importante

Este proyecto está preparado para PostgreSQL en Render sin ejecutar `dotnet ef database update` dentro de Render.

Al iniciar:
- si detecta PostgreSQL y no existe `AspNetUsers`, crea el esquema con `EnsureCreatedAsync()`
- luego crea roles y el usuario administrador

## Usuario administrador inicial

- usuario: `administrador`
- contraseña: `Admin123`

## Endpoint útil para verificar

- `/db-status`

Debe devolver `existeAspNetUsers: true` cuando la base ya quedó creada.

## Recomendación

Para evitar estados raros, usa una base PostgreSQL nueva y vacía en Render antes del primer deploy.
