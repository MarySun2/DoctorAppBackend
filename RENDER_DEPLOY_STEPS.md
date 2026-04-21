# Backend listo para Render

## Variables de entorno sugeridas
- `DatabaseProvider=PostgreSQL`
- `ConnectionStrings__DefaultConnection=Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true`
- `TokenKey=TU_CLAVE_SECRETA_LARGA`
- `ASPNETCORE_ENVIRONMENT=Production`

## Ruta del Dockerfile en Render
`API/Dockerfile`

## URL del backend para Angular
Cuando Render termine el deploy, coloca la URL en `environment.prod.ts` así:

```ts
export const environment = {
  production: true,
  apiUrl: 'https://TU-BACKEND.onrender.com/api',
};
```
