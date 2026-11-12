# Exercises 09 — Computer networking

Run scanning and server exercises only against your own machine.

### 1. Subnet calculator ★★
Given an address in CIDR notation, print the network address, broadcast address and number of usable hosts.
`192.168.10.77/26` → network `192.168.10.64`, broadcast `192.168.10.127`, `62` hosts

### 2. DNS lookup ★
Resolve a host name to all its IPv4 and IPv6 addresses with `Dns.GetHostAddressesAsync`.

### 3. TCP echo ★★
A server on `TcpListener` that sends back whatever it receives, and a client that sends lines typed by the user.

### 4. Many clients ★★
Extend the echo server so it serves many clients at once with `async`/`await`. Test with ten simultaneous clients.

### 5. UDP ping ★★
A client that sends ten UDP datagrams to your own UDP echo server, measures each round-trip time, treats no reply within one second as lost, and prints min/avg/max and loss percentage.

### 6. Port scanner ★★
Check which TCP ports from 1 to 1024 are open on `localhost`, with a 200 ms timeout and at most 50 connections in flight.

### 7. HTTP by hand ★★
Open a `TcpClient` to `example.com` on port 80, write a valid `GET / HTTP/1.1` request yourself, and print the status line and response headers.

### 8. REST client ★★
Fetch JSON from a public API with `HttpClient`, deserialise it into records with `System.Text.Json`, and handle timeouts and non-success status codes.

### 9. Static file server ★★★
An HTTP/1.1 server on raw sockets that serves files from a folder with the correct `Content-Type` and `Content-Length`, returns 404 for missing files, and refuses paths that escape the folder.

### 10. Packet capture ★★
With Wireshark, capture the opening of a TCP connection and label the SYN, SYN-ACK and ACK packets. Then capture a TLS handshake and identify the ClientHello and ServerHello.

### 11. Message framing ★★★
Design a chat protocol where each message is prefixed by its length in four bytes. Implement a server and client that stay correct when messages arrive split or merged.

### 12. Parallel downloader ★★★
Download a large file in four parts simultaneously using HTTP `Range` requests, then join the parts and verify the result with a SHA-256 checksum.
