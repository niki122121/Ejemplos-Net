# Ejemplos-Net

Ejemplos de networking básicos con cliente en Unity y back-end en .NET.  
Software de apoyo para la asignatura **Juegos para Web y Redes Sociales** del **Grado en Diseño y Desarrollo de Videojuegos** (URJC).

Licencia: The MIT License

---

## 1. Requisitos

- **.NET SDK** instalado (la misma versión que se use en la asignatura).
- **Visual Studio** con la carga de trabajo *Desarrollo de ASP.NET y web* (o equivalente).
- **Unity** (misma versión que la utilizada en clase) con soporte para scripts en C#.

---

## 2. Estructura del repositorio

- `BackEnd_servers/`  
  Proyectos .NET que actúan como **servidores** de ejemplo:
  - Servidor HTTP básico (Hello World).
  - APIs REST sencillas (por ejemplo, ranking / scoreboard).
  - Ejemplo de **guardado persistente** en fichero.
  - Ejemplo de **WebSocket** para comunicación bidireccional en tiempo real.

- `EjemplosUnityProj/`  
  Proyecto de Unity que funciona como **cliente**:
  - Escena(s) que hacen peticiones HTTP/REST al servidor.
  - Escena(s) que consumen el API de scoreboard.
  - Escena(s) que se conectan por WebSocket al servidor autoritativo / relay.

---

## 3. Puesta en marcha rápida

### 3.1. Ejecutar un servidor .NET

1. Clona el repositorio:

   ```bash
   git clone https://github.com/niki122121/Ejemplos-Net.git
   cd Ejemplos-Net/BackEnd_servers
