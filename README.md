# Ejemplos-Net

Licencia: The MIT License

Ejemplos de **networking** con **cliente en Unity** y **back-end en .NET** (más variantes equivalentes en **Node.js** y **Python**).  
Material de apoyo para la asignatura *Juegos para Web y Redes Sociales*.

> **Objetivo de este README:** cumplir la consigna de que *“el software incluye instrucciones sobre cómo usarse (o estas están en material de acceso abierto utilizado en la asignatura)”* y dejar documentado cómo ejecutar **cada ejemplo**, cómo conectarlo desde **Unity**, y qué **problemas comunes** pueden surgir.

---

## Índice

1. Requisitos  
2. Estructura del repositorio  
3. Puesta en marcha rápida  
   - 3.1. Servidores .NET  
   - 3.2. Cliente en Unity  
4. Ejemplos y uso paso a paso  
   - Hello World HTTP  
   - Scoreboard (API REST)  
   - Guardado persistente en fichero  
   - WebSocket (comunicación bidireccional)  
   - Alternativas: Node.js y Python  
5. Conceptos usados (resumen de teoría)  
6. Problemas comunes y soluciones  
7. Checklist de evaluación / Rúbrica  
8. Créditos y licencia

---

## Requisitos

- **.NET SDK** (versión LTS recomendada)  
- **Visual Studio** con la carga *Desarrollo de ASP.NET y web* **o** cualquier editor + CLI `dotnet`  
- **Unity** (misma versión que la usada en clase), con soporte C#  
- **Opcional** (para ejemplos alternativos):  
  - **Node.js** ≥ 18  
  - **Python** ≥ 3.9

---

## Estructura del repositorio

```
BackEnd_servers/        # Proyectos .NET: HTTP básico, REST/scoreboard, persistencia, WebSocket
EjemplosUnityProj/      # Proyecto(s) Unity que consumen los back-end
LICENSE                 # Archivo de licencia del repo (si procede)
README.md               # Este archivo
```

> Los nombres de carpetas/proyectos pueden variar ligeramente; dentro de `BackEnd_servers/` encontrarás cada proyecto del servidor.

---

## Puesta en marcha rápida

### 3.1. Servidores .NET

1) Clona el repo y entra en la carpeta:
```bash
git clone https://github.com/niki122121/Ejemplos-Net.git
cd Ejemplos-Net
```

2) Abre el proyecto de servidor que quieras (dentro de `BackEnd_servers/`) y ejecútalo:

**Con CLI**
```bash
cd BackEnd_servers/<NombreDelProyecto>
dotnet restore
dotnet run
```

**Con Visual Studio**  
Abre la solución/proyecto, selecciona perfil (IIS Express o Kestrel) y pulsa **Run**.

> Al arrancar, la consola mostrará las **URLs/puertos**. Si necesitas cambiarlos, revisa `Properties/launchSettings.json` o define `ASPNETCORE_URLS=http://localhost:PORT`.

---

### 3.2. Cliente en Unity

1) Abre **Unity Hub** → **Open** → `EjemplosUnityProj/`  
2) Carga la escena del ejemplo que corresponda (p.ej., `Ejemplo_http` para el caso HTTP).  
3) En el script del cliente, **ajusta la URL** (y puerto) del servidor .NET que tengas en marcha.  
4) Pulsa **Play** y observa la **Console** de Unity para ver la respuesta.

**Snippet base con `UnityWebRequest`** (copia/pega en tu script):
```csharp
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HttpClientExample : MonoBehaviour
{
    public void Ping() => StartCoroutine(GetHelloWorld());

    IEnumerator GetHelloWorld()
    {
        string url = "http://localhost:5140/hello"; // ¡ajusta el puerto!
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error: " + www.error);
            else
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
        }
    }
}
```

---

## Ejemplos y uso paso a paso

### Hello World HTTP

**Servidor (.NET)**
- Ejecuta el proyecto “Hello World”.
- Prueba en el navegador: `http://localhost:<PUERTO>/hello` → debería devolver **Hello World**.

**Cliente (Unity)**
- Abre la escena `Ejemplo_http`, pulsa el botón del ejemplo (o llama a `Ping()`) y comprueba la **Console**.

---

### Scoreboard (API REST)

**End-points sugeridos del ejemplo**
- `GET /getScores?top=X` → devuelve las **X mejores** puntuaciones (JSON o texto).  
- `GET /getScoresPretty?top=X` → devuelve HTML formateado para navegador.  
- `POST /addScore/{user}/{num}` (o variante `GET`) → añade una puntuación.

**Probar desde navegador o curl**
```bash
# Añadir puntuación
curl http://localhost:<PUERTO>/addScore/Ana/1234

# Ver top 5 en texto/JSON
curl http://localhost:<PUERTO>/getScores?top=5

# Ver en navegador con formato
# http://localhost:<PUERTO>/getScoresPretty?top=5
```

**Consumir desde Unity**
```csharp
IEnumerator GetTopScores(int top = 5)
{
    string url = $"http://localhost:5140/getScores?top={top}";
    using var req = UnityWebRequest.Get(url);
    yield return req.SendWebRequest();
    if (req.result != UnityWebRequest.Result.Success)
        Debug.LogError(req.error);
    else
        Debug.Log($"Top {top}: " + req.downloadHandler.text);
}
```

> **Limitaciones del prototipo:** sin persistencia por defecto y **no autoritativo** (un cliente podría inventar puntuaciones). Es perfecto para práctica y prototipos, no para producción competitiva.

---

### Guardado persistente en fichero

**Idea:** el servidor calcula una ruta de guardado (carpeta de datos de la app) y usa un `.txt`.

**End-points típicos de prueba**
- `GET /testSave` → escribe un contenido de prueba en disco.  
- `GET /testRead` → lee el contenido del archivo y lo devuelve.

**Pasos**
1) Ejecuta el servidor de **persistencia**.  
2) Navega a `/testSave`.  
3) Navega a `/testRead` y confirma que obtienes lo guardado.

> **Buenas prácticas:** en proyectos mayores usa **SQLite**, **PostgreSQL**, etc. para evitar inconsistencias con múltiples usuarios.

---

### WebSocket (comunicación bidireccional)

**Cuándo usarlo:** tiempo real y bidireccional sin abrir/cerrar conexión en cada mensaje.

**Pasos**
1) Ejecuta el servidor **WebSocket** (verás algo como `ws://localhost:<PUERTO>/ws`).  
2) Configura el cliente Unity con la URL WS y ejecuta la escena del ejemplo.  
3) También puedes probar con `wscat`:
```bash
npx wscat -c ws://localhost:<PUERTO>/ws
# escribe mensajes y verás eco/broadcast según implemente el servidor
```

**Diseño**
- **Servidor autoritativo:** la lógica del juego corre en el servidor; los clientes envían inputs. +Anticheat, +consistencia.  
- **Relay:** el servidor solo reenvía mensajes; más simple, pero no evita trampas.

---

### Alternativas: Node.js y Python

Además del back-end en .NET, hay equivalentes mínimos:

**Node.js**
```js
// server.js
const http = require("http");
const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader("Content-Type", "text/plain");
  res.end("Hello World\n");
});
server.listen(3000, () => console.log("Servidor escuchando en 3000"));
```
```bash
node server.js
# Navegador: http://localhost:3000
```

**Python**
```python
# server.py
from http.server import HTTPServer, BaseHTTPRequestHandler
class SimpleHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        self.send_response(200)
        self.send_header("Content-type", "text/plain; charset=utf-8")
        self.end_headers()
        self.wfile.write(b"Hello World")

if __name__ == "__main__":
    print("Servidor escuchando en 8000")
    HTTPServer(("localhost", 8000), SimpleHandler).serve_forever()
```
```bash
python server.py
# Navegador: http://localhost:8000
```

---

## Conceptos usados (resumen de teoría)

- **Arquitecturas de red**
  - **P2P:** menor coste de servidor; más complejidad (NAT/firewall) y menor “autoría” (más fácil hacer trampas si la lógica va en el cliente).
  - **Cliente-servidor:** mayor coste (hosting), conexiones más sencillas y posibilidad de **servidor autoritativo** (la “fuente de la verdad”).
- **Firewall/NAT**
  - Permiten salir peticiones y entrar **respuestas**; bloquean tráfico entrante no solicitado → complica P2P puro (NAT traversal, *hole punching*).
  - En la práctica, un servidor accesible públicamente simplifica el despliegue.
- **Unity Netcode**
  - Solución integrada *todo-en-uno*; potente pero puede ser excesiva si solo necesitas HTTP/REST/WS simples.
- **Back-end en .NET**
  - Rápido y agnóstico del motor; ideal para rankings, formularios y guardados sencillos.
  - Requiere manejar HTTP/puertos/errores de red con cuidado para evitar bugs.

---

## Problemas comunes y soluciones

- **404 / Connection refused**
  - Verifica **puerto y protocolo** (`http://` vs `https://`).  
  - Comprueba que el servidor está *escuchando* y que no hay otro proceso usando el puerto.

- **CORS (WebGL)**
  - Si consumes desde un *build* WebGL, habilita CORS en el API o sirve todo desde el mismo origen.

- **Firewall/antivirus local**
  - Permite `dotnet`/`node`/`python` en el firewall. Puertos típicos: 3000, 5140, 8000, etc.

- **Unity congelado**
  - No hagas I/O de red en el hilo principal: usa **corrutinas** o `async/await`.

- **Persistencia**
  - La ruta de guardado depende del SO; *loguea* la ruta y comprueba permisos de escritura.

- **WebSocket no conecta**
  - Verifica la **ruta WS** (`/ws` u otra), que el servidor hace *Upgrade: websocket* y que no estás intentando `https://` en vez de `ws://` (o `wss://` si corresponde).

---

## Checklist de evaluación / Rúbrica

- ✅ **Instrucciones de uso incluidas**: cómo arrancar servidores, cómo configurar y ejecutar el cliente Unity, y cómo probar endpoints.  
- ✅ **Referencias a material abierto**: este README y el repositorio sirven como material de consulta para la asignatura.  
- ✅ **Contexto técnico**: explicación breve de P2P vs cliente-servidor, firewall/NAT, servidor autoritativo y cuándo usar HTTP vs WebSocket.  
- ✅ **Limitaciones y buenas prácticas**: prototipo no autoritativo y sin persistencia por defecto; recomendación de pasar a base de datos en proyectos mayores.  
- ✅ **Solución de problemas**: listado de errores típicos con acciones concretas.

---

## Créditos y licencia

- Material y ejemplos de apoyo para *Juegos para Web y Redes Sociales*.  
- **Licencia:** ver archivo `LICENSE` del repositorio (o añade el que corresponda).
