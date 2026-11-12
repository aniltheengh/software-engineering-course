# === Calling the API with curl ===
# Start the example first with: dotnet run
curl http://localhost:5080/
curl http://localhost:5080/hello/Taman
curl "http://localhost:5080/add?a=2&b=3"                  # {"a":2,"b":3,"sum":5}

# Create (-i also prints the status line and headers)
curl -i -X POST http://localhost:5080/notes \
     -H "Content-Type: application/json" \
     -d '{"text":"Check flange rating"}'                  # 201 Created, Location: /notes/1

curl http://localhost:5080/notes                          # list
curl -i http://localhost:5080/notes/99                    # 404 Not Found
curl -i -X POST http://localhost:5080/notes \
     -H "Content-Type: application/json" -d '{"text":""}' # 400 Bad Request
curl -i -X DELETE http://localhost:5080/notes/1           # 204 No Content
