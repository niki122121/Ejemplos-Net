using Microsoft.AspNetCore.Http.Features;
using System.Net.WebSockets;
using System.Net;
using System;
using System.Text;

namespace Ejemplo_websocket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            //ws config
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };
            app.UseWebSockets(webSocketOptions);

            app.MapGet("/", () => "ALL OK");

            //el websocket comienza como una petici�n normal
            app.MapGet("/ws", async (HttpContext context) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {

                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        try
                        {
                            await HandleSocketConneciton(webSocket);
                        }
                        catch (WebSocketException ex2)
                        {
                            Console.WriteLine("WS ex:  " + ex2.Message);
                            HandleConnectionEnd(webSocket);
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("ERROR: Non ws petition");
                }
            });


            app.Run();
        }

        //empieza la conexi�n (el bucle while chequea constantemente si hay mensajes nuevos del usuario)
        static private async Task HandleSocketConneciton(WebSocket webSocket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);

            Console.WriteLine("Connection with user initiated");

            await socketSend(webSocket, new byte[3] { 25, 25, 25 }); //ejemplo de primer mensaje a enviar (25,25,25 es completamente aleatorio)

            while (!webSocket.CloseStatus.HasValue)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    await HandleMessageRecieved(webSocket, ms.ToArray());
                }
            }

            await webSocket.CloseOutputAsync(webSocket.CloseStatus.Value, "Connecion Closed", CancellationToken.None);

            HandleConnectionEnd(webSocket);
        }

        //la conexi�n termina por error o cierre
        static private void HandleConnectionEnd(WebSocket webSocket)
        {
            Console.WriteLine("connection ended");
        }

        //m�todo que recibe mensajes del usuario (el async Task es por si vamos luego a enviar cosas dentro de esta funci�n)
        static private async Task HandleMessageRecieved(WebSocket webSocket, byte[] msgBytes)
        {
            Console.WriteLine("message recieved");
            //Sugerencia: usar el primer byte del mensaje como un ID de accion. Es decir para saber que hacer con le resto del mensaje
            switch (msgBytes[0])
            {

            }
        }

        //con esta funcion podeis enviar info al cliente.
        static public async Task socketSend(WebSocket ws, byte[] msgBytes)
        {
            if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
            {
                await ws.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }



        //recomiendo encapsular cada partida y jugador con sus propias clases
        class Game { }
        class Player { } //aqui en Player hay que meter una referencia a su clase WebSocket para poder enviarle info
    }
}
