# 15 — Security

**Duration:** 3 weeks · **Prerequisites:** 09, 13

## Goals
- Recognise and prevent the common classes of vulnerability in your own code
- Use cryptography correctly by relying on vetted libraries

## Topics
1. Threat modelling
2. OWASP Top 10: injection, broken access control, XSS, CSRF and the rest
3. Cryptography concepts: hashing, symmetric and asymmetric encryption, signatures
4. Password storage and authentication flows
5. TLS in practice
6. Secrets management
7. Dependency and supply-chain security
8. Secure coding guidelines for .NET

## Resources
- **Primary:** [OWASP Top Ten](https://owasp.org/www-project-top-ten/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [ASP.NET Core security documentation](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [PortSwigger Web Security Academy (free labs)](https://portswigger.net/web-security)
- [Cryptography I — Dan Boneh (Coursera)](https://www.coursera.org/learn/crypto)

## Practice
- Complete the SQL injection, XSS and access-control lab tracks on Web Security Academy
- Write a deliberately injectable query in a throwaway app, exploit it, then parameterise it
- Hash passwords with a proper password-hashing function and verify them
- Move all secrets in your Module 13 app to user secrets / environment variables

## Checkpoint project
A **security review** of your Module 13 application: a threat model, a findings list against the OWASP Top 10, and the fixes committed.
