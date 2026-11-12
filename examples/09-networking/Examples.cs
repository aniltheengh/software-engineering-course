// Run with: dotnet run
// Everything here talks to this machine only (127.0.0.1), so it works offline.
using System.Net;
using System.Net.Sockets;
using System.Text;

AddressDemo.Run();
await DnsDemo.Run();
await TcpDemo.Run();
await UdpDemo.Run();
await HttpDemo.Run();

// === IP addresses, masks and URLs ===
static class AddressDemo
{
    public static void Run()
    {
        IPAddress address = IPAddress.Parse("192.168.10.77");
        byte[] octets = address.GetAddressBytes();
        Console.WriteLine(string.Join(".", octets.Select(o => Convert.ToString(o, 2).PadLeft(8, '0'))));

        // A /24 mask keeps the first 24 bits: that is the network part.
        uint ip = (uint)(octets[0] << 24 | octets[1] << 16 | octets[2] << 8 | octets[3]);
        uint mask = 0xFFFFFFFF << (32 - 24);
        uint network = ip & mask;
        Console.WriteLine($"network: {network >> 24}.{(network >> 16) & 255}.{(network >> 8) & 255}.{network & 255}");   // 192.168.10.0

        var url = new Uri("https://example.com:8443/api/pipes?size=50#top");
        Console.WriteLine($"{url.Scheme} | {url.Host} | {url.Port} | {url.AbsolutePath} | {url.Query}");
    }
}

// === DNS: from a name to addresses ===
static class DnsDemo
{
    public static async Task Run()
    {
        IPAddress[] addresses = await Dns.GetHostAddressesAsync("localhost");
        foreach (IPAddress a in addresses) Console.WriteLine($"localhost -> {a} ({a.AddressFamily})");
    }
}

// === TCP: a reliable, ordered byte stream ===
static class TcpDemo
{
    public static async Task Run()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);   // port 0: let the OS choose a free port
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;

        // Server: accept one client, read a line, answer in upper case.
        Task server = Task.Run(async () =>
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using var reader = new StreamReader(client.GetStream());
            await using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            string? line = await reader.ReadLineAsync();
            await writer.WriteLineAsync(line?.ToUpperInvariant());
        });

        // Client: connect, send, wait for the reply.
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(IPAddress.Loopback, port);
            using var reader = new StreamReader(client.GetStream());
            await using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            await writer.WriteLineAsync("hello over tcp");
            Console.WriteLine(await reader.ReadLineAsync());   // HELLO OVER TCP
        }

        await server;
        listener.Stop();
    }
}

// === UDP: single datagrams, no connection, no guarantee ===
static class UdpDemo
{
    public static async Task Run()
    {
        using var receiver = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        int port = ((IPEndPoint)receiver.Client.LocalEndPoint!).Port;

        using var sender = new UdpClient();
        byte[] payload = Encoding.UTF8.GetBytes("level=72.5");
        await sender.SendAsync(payload, payload.Length, new IPEndPoint(IPAddress.Loopback, port));

        UdpReceiveResult result = await receiver.ReceiveAsync();
        Console.WriteLine($"{Encoding.UTF8.GetString(result.Buffer)} from {result.RemoteEndPoint}");
    }
}

// === HTTP: request and response on top of TCP ===
static class HttpDemo
{
    public static async Task Run()
    {
        // A tiny local HTTP server so the client has something to call.
        var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        int port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();

        using var server = new HttpListener();
        server.Prefixes.Add($"http://127.0.0.1:{port}/");
        server.Start();

        Task serving = Task.Run(async () =>
        {
            HttpListenerContext context = await server.GetContextAsync();
            Console.WriteLine($"server saw: {context.Request.HttpMethod} {context.Request.RawUrl}");
            byte[] body = Encoding.UTF8.GetBytes("{\"status\":\"ok\"}");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(body);
            context.Response.Close();
        });

        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        using HttpResponseMessage response = await http.GetAsync($"http://127.0.0.1:{port}/health?verbose=1");

        Console.WriteLine($"status: {(int)response.StatusCode} {response.StatusCode}");   // 200 OK
        Console.WriteLine($"content-type: {response.Content.Headers.ContentType}");
        Console.WriteLine($"body: {await response.Content.ReadAsStringAsync()}");

        await serving;
        server.Stop();
    }
}
