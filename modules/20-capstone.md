# 20 — Capstone project

**Duration:** 6 weeks · **Prerequisites:** Phases 1–3

**Exercises:** [8 exercises for this module](../exercises/20-capstone.md)

## Goal
Design, build, test, deploy and document one substantial piece of software on your own. This is the project you show to other people.

## Requirements
- A written proposal: problem, users, scope, what is deliberately left out
- A design document with an architecture diagram and data model
- At least 3,000 lines of your own C#
- Automated tests and a CI pipeline
- Persistent storage
- A deployed or installable release
- A README that lets a stranger build and run it
- A short retrospective: what went well, what you would change

## Project ideas
- **CAD automation tool:** a plugin or companion service that automates drawing or model generation, with a tested core library separate from the CAD host
- **Engineering calculation service:** web API and frontend for sizing calculations, with saved projects and PDF reports
- **Document pipeline:** ingest files, extract data, store, search and report, processed by background workers
- **Developer tool:** a CLI or Roslyn analyzer published as a NuGet package
- **Distributed system:** extend the Module 17 key-value store with full Raft and a client library

## Suggested schedule
| Week | Work |
|------|------|
| 1 | Proposal, design document, repository and CI set up |
| 2–4 | Core features, tests written alongside |
| 5 | Hardening: error handling, security review, performance |
| 6 | Deployment, documentation, retrospective |

## Resources
- [Make a README](https://www.makeareadme.com/)
- [Keep a Changelog](https://keepachangelog.com/)
- [Semantic Versioning](https://semver.org/)
- [Choose a License](https://choosealicense.com/)
