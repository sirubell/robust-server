using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static public void Main()
    {
        Int32 port = 12345;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        TcpListener server = new TcpListener(localAddr, port);
        server.Start();
        Console.WriteLine("Server Start!");

        int counter = 0;

        try
        {
            while (true)
            {
                counter++;
                TcpClient client = server.AcceptTcpClient();
                StartClientThread(client, Convert.ToString(counter));
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            server.Stop();
        }
    }

    static void StartClientThread(TcpClient client, string name)
    {
        Thread t = new Thread( () => { KeepListening(client, name); });
        t.Start();
    }
    static void KeepListening(TcpClient client, string name)
    {
        string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
        string info = $"IP: {clientIp}, name: {name}";
        Console.WriteLine($"Client connected with {info}");

        try
        {
            NetworkStream stream = client.GetStream();
            byte[] bufferRead = new byte[2048];
            byte[] bufferSend = new byte[2048];
            int bytesRead;

            while ((bytesRead = stream.Read(bufferRead, 0, bufferRead.Length)) > 0)
            {
                string msg = Encoding.ASCII.GetString(bufferRead, 0, bytesRead);
                Console.WriteLine($"Receive {msg} from client: {info}");

                msg = msg.ToUpper();

                bufferSend = Encoding.ASCII.GetBytes(msg);

                stream.Write(bufferSend, 0, bufferSend.Length);
                Console.WriteLine($"Send {msg} to client: {info}");
            }

            Console.WriteLine("hi");
            // stream.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            client.Close();
            Console.WriteLine($"Client disconnected with {info}");
        }
    }
}