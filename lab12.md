80555555555555in"}

**12**

> **Desarrollo de Aplicaciones Empresariales Avanzadas**
>
> Arévalo Sermeño, Edwin William
>
> Desarrollo de Aplicaciones Empresariales Avanzadas
>
> 6 - C24 - Sección C - D

**[Web API REST SEMANA12]{.underline}**

En este desafío, se te pide que desarrolles una **Web API REST** con
ASP.NET Core y Entity Framework Core sobre un **tema de tu elección**
(por ejemplo, clínica, veterinaria, gimnasio, tickets o taller
mecánico), distinto de la demostración de clase (productos y
categorías), y que la **publiques en internet con su base de datos
también en la nube**. Todo el acceso a datos se resuelve con LINQ y los
controladores usan directamente el DbContext.

**Tema y modelo de datos:**

1.  Define las entidades de tu tema: **al menos dos** con una relación
    **1:N** (clave foránea y propiedad de navegación).

2.  Crea la base de datos con un **script SQL** (incluye datos de
    ejemplo) y genera el modelo con **Scaffold** (base de datos
    primero). El script va en el repositorio.

3.  Agrega Data Annotations a las entidades para validar los datos
    (\[Required\], \[StringLength\], \[Range\]): con \[ApiController\]
    los datos inválidos responden 400 automáticamente.

4.  La propiedad de navegación de la entidad hija debe admitir nulo
    (Categoria?) para que el POST solo envíe la clave foránea. Evita el
    ciclo de JSON sin usar DTOs (\[JsonIgnore\] en la colección o
    ReferenceHandler.IgnoreCycles).

**Web API (ASP.NET Core, .NET 10):**

1.  Crear un proyecto **ASP.NET Core Web API con controladores** (no
    minimal API), con un controlador por entidad, \[ApiController\] y
    rutas api/\[controller\].

2.  **CRUD completo** de las dos entidades (**mínimo 10 endpoints**), un
    GET con **filtros por query string** y una **ruta anidada** (como
    /api/categorias/{id}/productos).

3.  Todas las acciones son async y usan LINQ (ToListAsync, FindAsync,
    AnyAsync, Include, AsNoTracking).

4.  **Swagger** (OpenAPI + SwaggerUI) visible en la API publicada y
    **CORS** configurado con un origen concreto (no usar
    AllowAnyOrigin).

**Según tu editor:**

1.  **Visual Studio:** Crear un proyecto nuevo con la plantilla
    **ASP.NET Core Web API** (no la de \"native AOT\"). En \"Información
    adicional\" selecciona y marca:

  --------------------------------------------------------------------
  **Opción**                    **Valor**
  ----------------------------- --------------------------------------
  Framework                     .NET 10

  Tipo de autenticación         Ninguno

  Configurar para HTTPS         Marcado
  (Configure for HTTPS)         

  **Usar controladores (Use     **Marcado**: si lo dejas sin marcar se
  controllers)**                crea una minimal API y no existe la
                                carpeta Controllers

  **Habilitar compatibilidad    **Marcado**: genera el documento
  con OpenAPI (Enable OpenAPI   OpenAPI en que se basa Swagger
  support)**                    

  No usar instrucciones de      Sin marcar
  nivel superior                
  --------------------------------------------------------------------

Luego, en Administrar paquetes NuGet, instala:
Microsoft.EntityFrameworkCore.SqlServer,
Microsoft.EntityFrameworkCore.Design,
Microsoft.EntityFrameworkCore.Tools (necesario para Scaffold-DbContext)
y Swashbuckle.AspNetCore.SwaggerUI (la pantalla de Swagger).

2.  **VS Code:** en una terminal ejecuta lo siguiente (reemplaza
    \<NombreProyecto\> por el nombre de tu proyecto):

> **dotnet** new webapi -n \<NombreProyecto\> \--use-controllers
>
> **cd** \<NombreProyecto\>
>
> **code** .
>
> **dotnet** add package Microsoft.EntityFrameworkCore.SqlServer
>
> **dotnet** add package Microsoft.EntityFrameworkCore.Design
>
> **dotnet** add package Swashbuckle.AspNetCore.SwaggerUI

\--use-controllers crea el proyecto con controladores en lugar de
minimal API. Para generar el modelo con Scaffold, instala una sola vez
la herramienta con dotnet tool install \--global dotnet-ef.

**Respuestas con ActionResult:**

Cada acción debe devolver el resultado que corresponde. Como mínimo se
deben usar estos seis:

  -------------------------------------------------------------------------------
  **Resultado**             **Código**   **Dónde debe aparecer**
  ------------------------- ------------ ----------------------------------------
  Ok(datos)                 200          GET que devuelve datos

  CreatedAtAction(\...)     201          POST correcto (con la ruta del nuevo
                                         recurso)

  NoContent()               204          PUT y DELETE correctos

  BadRequest() o            400          Id de la ruta distinto del cuerpo, clave
  ValidationProblem(\...)                foránea inexistente o datos inválidos

  NotFound()                404          El recurso solicitado no existe

  Conflict(\...)            409          Regla de negocio incumplida
  -------------------------------------------------------------------------------

Implementa **al menos dos reglas de negocio** con respuesta 409 (por
ejemplo, un valor único repetido o eliminar un registro que tiene
dependientes).

**Despliegue en la nube (obligatorio):**

1.  Publica la **API** y la **base de datos** en servicios en la nube
    con **plan gratuito**. Ambas son obligatorias: no se acepta una API
    en la nube conectada a una base de datos local.

2.  **Investiga tú mismo las alternativas gratuitas** de hosting para
    ASP.NET Core y de base de datos, y justifica tu elección. **No es
    obligatorio usar servicios de pago.**

3.  Si la base de datos gratuita que elijas no es SQL Server, cambia el
    proveedor de EF Core (paquete y método Use\... correspondientes) y
    adapta el script; explícalo en tus conclusiones.

4.  La cadena de conexión **no se sube al repositorio**: en la nube se
    configura como variable de entorno o con el mecanismo de secretos
    del proveedor.

5.  Ejecuta el script en la base de la nube y comprueba que la API
    publicada responde (en planes gratuitos el primer request puede
    tardar).

**Pruebas:**

Prueba **cada endpoint y cada código de respuesta (200, 201, 204, 400,
404 y 409) sobre la API publicada**, no sobre localhost, usando Postman
u otro cliente (Swagger, Thunder Client, archivo .http).

**Entregables:**

- Explicación de tu tema (texto): qué problema resuelve, las entidades,
  su relación y las reglas de negocio:


