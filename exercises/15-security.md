# Exercises 15 — Security

Attack only applications you wrote and run on your own machine.

### 1. Threat model ★★
For your Module 13 to-do API, draw the data flow and fill a STRIDE table: threat, affected component, mitigation.

### 2. SQL injection ★★
In a throwaway app, build a login query by string concatenation. Log in without knowing a password, then fix the query with parameters and show the attack fails.

### 3. Cross-site scripting ★★
In a throwaway page that displays user comments without encoding, make a script run from a stored comment. Fix it with output encoding and add a Content-Security-Policy header.

### 4. Cross-site request forgery ★★
Explain in your own words how CSRF works against cookie authentication. Add antiforgery protection to a form and show a forged request being rejected.

### 5. Password storage ★★
Store passwords with `Rfc2898DeriveBytes.Pbkdf2`, a random per-user salt and a high iteration count. Verify with `CryptographicOperations.FixedTimeEquals`.

### 6. Checksums ★
A tool that prints the SHA-256 of a file and compares it with an expected value.

### 7. Authenticated encryption ★★★
Encrypt and decrypt a file with AES-GCM using a key derived from a passphrase. Show that flipping one byte of the ciphertext makes decryption fail.

### 8. Digital signatures ★★
Generate an ECDSA key pair, sign a file, verify the signature, and show verification failing after the file changes.

### 9. JWT internals ★★★
Decode a JWT by hand (base64url). Change a claim and show your API rejects it. Explain why accepting `alg: none` would be dangerous.

### 10. Broken access control ★★
Write a test proving user A cannot read or modify user B's item by guessing its id. Fix any failure.

### 11. Secrets ★★
Search your repositories' full history for committed secrets. Move configuration secrets to user secrets or environment variables.

### 12. Dependencies ★
Run `dotnet list package --vulnerable --include-transitive` on your projects and enable Dependabot alerts on GitHub.

### 13. Transport and headers ★★
Enforce HTTPS and HSTS and add `X-Content-Type-Options` and frame-protection headers. Check the result with your browser's developer tools.

### 14. Path traversal ★★★
A download endpoint takes a file name. Show that `../` escapes the intended folder, then fix it by resolving the full path and checking it stays inside the folder.
