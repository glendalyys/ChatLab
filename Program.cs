using System;
using System.Net; 
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;


public class Program{
    public static void BecomeClient(string serverIP, int port){
        // a connection socket 
        var client = new TcpClient();
        client.Connect(serverIP, port); 
        // var fromServer = new StreamReader(streams);
        Task.WaitAny(StartTasks(client)); 
        client.Close();

    }

    public async static void BecomeServer(int port){
        var server = new TcpListener(IPAddress.Any, port);
        server.Start();
        var client = server.AcceptTcpClient();
        // var fromClient = new StreamReader(streams);
        
        Task.WaitAny(StartTasks(client)); 
        client.Close();
        server.Stop();
    }

    public static Task[] StartTasks(TcpClient client){
        var streams =  client.GetStream();
        var w = Task.Run(() => {
            var toServer = new StreamWriter(streams){
                AutoFlush = true
            };
            // var fromClient = new StreamReader(client.GetStream());
            while(true){
                Console.Write(">>>");
                Console.Out.Flush();
                var text = Console.ReadLine();
                if(text != null){
                    toServer.WriteLine(text);

                }else{
                    break;
                }
              
            }
        
        
        });
        var r = Task.Run(() => {
            // var stream = client.GetStream();
            // var fromClient = new StreamReader(client.GetStream());
            var fromClient = new StreamReader(streams);
            while(true){
                var text = fromClient.ReadLine();
                if(text == null){
                    break;
                }
                Console.WriteLine(text);   
            }
            // future is complete
        });
       
        return new Task[]{r,w};
        
    }

    public static void Main(string[] args){
        var ServerOrClient = args[0];
        // string s = ServerOrClient.Substring(args[0]);
        if (ServerOrClient == "server"){
            var portNum = Int32.Parse(args[1]);
            BecomeServer(portNum);
        }else{
            var IPAddress = args[1];
            var portNumClient = Int32.Parse(args[2]);
            BecomeClient(IPAddress,portNumClient);

        }
    }
}