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
            string receivedData = String.Empty;

            while (ReceiveData(stream, out receivedData))
            {
                Console.WriteLine($"Receive {receivedData} from client: {info}");

                string msg = doSomething(receivedData);

                SendData(stream, msg);
                Console.WriteLine($"Send {msg} to client: {info}");

                Console.WriteLine("if this message spam too much, it means the code has a bug somewhere.");
            }
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

    static bool ReceiveData(NetworkStream stream, out string data)
    {
        byte[] buffer = new byte[8];
        StringBuilder msg = new StringBuilder();
        data = String.Empty;

        try
        {
            do
            {
                Int32 bytes = stream.Read(buffer, 0, buffer.Length);
                msg.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
            } while (stream.DataAvailable);
        } catch (Exception ex)
        {
            return false;
        }
        

        data = msg.ToString();
        return data != String.Empty;
    }

    static string doSomething(string data)
    {
        return data.ToUpper();
    }

    static void SendData(NetworkStream stream, string msg)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(msg);
        stream.Write(buffer, 0, buffer.Length);
    }
}