using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ejemplo_websocket : MonoBehaviour
{
    [SerializeField] int port = 5140;
    string url = "ws://localhost";
    ClientWebSocket socket;

    public void connect()
    {
        wsConnect();
    }

    public void disconnect()
    {
        socket.Abort();
    }
    public void requestDisconnect()
    {
        //IMPORTANTE: para evitar problemas, lo mejor es pedir al servidor que termine nuestra conexión en vez de hacerlo nosotros
        //Sugerencia: usando la funcion sockecSend, enviar un mensaje específico que pida la desconexión
        //socketSend(new byte [1] {1}) por ejemplo
    }


    public void send()
    {
        Debug.Log("message sent to server");
        socketSend(new byte[4] { 13, 2, 15, 241 }); //Ejemplo con bytes random
    }

    async Task wsConnect()
    {
        Debug.Log("WS: Initiating Connection");
        try
        {
            socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(url + ":" + port + "/" + "ws/"), CancellationToken.None);

            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        string socketReturnMsg = result.CloseStatusDescription;
                        Debug.Log(socketReturnMsg);
                        return;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    HandleMessageRecieved(ms.ToArray());
                }
            } while (!socket.CloseStatus.HasValue);

        }
        catch (WebSocketException ex)
        {
            Debug.Log("WS exception: Something went wrong, connection interrupted");
            Debug.LogError(ex);
        }
        finally
        {
            Debug.Log("WS: Connection Ended");
        }
    }

    void HandleMessageRecieved(byte[] msgBytes)
    {
        //Importante: si enviais texto, tener en cuenta que esto lo recibe como un array de bytes y lo tendreis que castear a string con formato UTF8 o ASCI
        //Sugerencia: usar un switch del primer byte del mensaje recibido para indicar que hacer con el resto (como un ID de la accion recibida)
        switch (msgBytes[0])
        {

        }

        Debug.Log(arrToString(msgBytes)); //aqui debería aparecer al inical la conexión el mensaje aleatorio de ejemplo del server
    }

    //Importante: el mensaje se envia como una secuencia de bytes. Si queremos enviar texto hay que castearlo a un array de bytes primero
    public async Task socketSend(byte[] msgBytes)
    {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        else
        {
            Debug.LogError("WS: Not connected to Server (request a session first)");
        }

    }

    string arrToString(byte[] arr)
    {
        string aux = "";
        for (int i = 0; i < arr.Length; i++)
        {
            aux += (arr[i] + ", ");
        }
        return aux;
    }
}
