# Exercises 16 — DevOps

Apply these to your Module 13 application.

### 1. Linux service ★★
On WSL or a Linux VM, create a dedicated user and run your published app as a `systemd` service that restarts on failure.

### 2. Backup script ★★
A bash script that archives a folder with a timestamp in the file name and keeps only the seven newest archives.

### 3. Continuous integration ★
A GitHub Actions workflow that restores, builds and tests on every push and pull request.

### 4. Matrix and cache ★★
Run the workflow on Ubuntu and Windows, and cache NuGet packages. Record the time saved.

### 5. Quality gate ★
Fail the build when `dotnet format --verify-no-changes` reports differences or any warning appears.

### 6. Dockerfile ★★
A multi-stage Dockerfile: build with the SDK image, run on the ASP.NET runtime image as a non-root user. Compare the final image size with a single-stage build.

### 7. Compose ★★
`docker compose up` starts the app and PostgreSQL with a named volume and a health check, and the app waits for the database to be healthy.

### 8. Configuration ★★
Move every environment-specific setting to environment variables so the same image runs unchanged in development and production.

### 9. Publish an image ★★
On a version tag, build the image and push it to GitHub Container Registry, tagged with the version.

### 10. Deploy ★★★
Deploy the container to a cloud provider's free or low-cost tier with a managed or containerised database, over HTTPS.

### 11. Health checks ★
Add liveness and readiness endpoints; readiness must fail when the database is unreachable.

### 12. Structured logging ★★
Emit JSON logs that include the correlation id from Module 13, and find all log lines for one request.

### 13. Metrics ★★★
Export request count and request duration with OpenTelemetry and view them in a local dashboard.

### 14. Rollback drill ★★★
Deploy a deliberately broken version, detect it from health checks or logs, and roll back. Write the steps down as a runbook.
