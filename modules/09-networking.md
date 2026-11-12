# 09 — Computer networking

**Duration:** 4 weeks · **Prerequisites:** 08

**Exercises:** [12 exercises for this module](../exercises/09-networking.md)

## Goals
- Trace what happens between typing a URL and seeing a page
- Write networked programs with sockets and HTTP

## Topics
1. Layered model: link, network, transport, application
2. IP addressing, subnets, routing, NAT
3. UDP and TCP: handshake, reliability, flow and congestion control
4. DNS
5. HTTP/1.1, HTTP/2, HTTP/3; REST
6. TLS and certificates
7. Sockets programming
8. Tools: `ping`, `traceroute`, `curl`, Wireshark

## Resources
- **Primary:** [Computer Networking: A Top-Down Approach (Kurose & Ross) — author site with lectures and labs](https://gaia.cs.umass.edu/kurose_ross/index.php)
- [Beej's Guide to Network Programming](https://beej.us/guide/bgnet/) — sockets (C; port to C#)
- [MDN — HTTP](https://developer.mozilla.org/en-US/docs/Web/HTTP)
- [Networking in .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/overview)

## Practice
- Kurose & Ross Wireshark labs: HTTP, DNS, TCP
- TCP echo server and client with `TcpListener` / `TcpClient`
- UDP ping tool that measures round-trip time
- Call a public REST API with `HttpClient` and deserialise the JSON

## Checkpoint project
A **multi-client chat server** over TCP, or a minimal **HTTP/1.1 server** written on raw sockets that serves static files.
