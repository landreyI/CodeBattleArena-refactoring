# CodeBattle Arena: Architectural Refactoring

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)
![MSSQL](https://img.shields.io/badge/mssql-%23316192.svg?style=for-the-badge&logo=mssql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)

This repository contains the refactored version of the CodeBattle Arena platform. The primary objective of this phase was to transition from a monolithic structure to a modular Clean Architecture, enhancing system reliability, scalability, and real-time performance.

---

## Technical Stack

| Category | Technology | Purpose |
| :--- | :--- | :--- |
| **Backend** | .NET 9 / ASP.NET Core | Core API and business logic |
| **Frontend** | React / TypeScript | User interface and real-time state management |
| **Real-time** | SignalR (WebSockets) | Bi-directional communication |
| **Persistence**| MSSQL (EF Core) | Relational data and notification storage |
| **Caching** | Redis | Distributed presence and atomic counters |
| **Messaging** | RabbitMQ / MassTransit | Asynchronous AI task processing |
| **AI Integration**| Google Gemini API | Automated task and test case generation |

---

## Key Refactoring Details

### 1. Distributed Presence System
The presence logic was migrated from in-memory tracking to a Redis-backed distributed system.
* **Atomic Tracking**: Utilizes Redis atomic operations to track multiple active connections per user.
* **Scalability**: Allows the application to run across multiple server instances while maintaining a consistent "Online" status.
* **Efficiency**: Status lookups are performed against Redis memory instead of the primary database.

### 2. SignalR Hub Optimization
To minimize resource consumption and improve client-side performance, fragmented hubs were consolidated:
* **MainHub**: Acts as a single gateway for global events, including presence updates, system notifications, and AI generation progress.
* **SessionHub**: Dedicated to high-frequency synchronization during active competitive sessions.
* **Echo Suppression**: Implemented `GroupExcept` logic to prevent redundant message broadcasting to the action initiator.

### 3. Persistent Notification Pattern
Developed a reliable "Inbox" mechanism for system-wide notifications:
* **Persistence First**: Notifications are saved to PostgreSQL before delivery to ensure they are never lost.
* **Presence Validation**: The system verifies user status via Redis before attempting a real-time push.
* **State Sync**: Clients fetch unread notifications from the database upon reconnection or initial login.

### 4. Asynchronous AI Task Generation
Heavy computational tasks related to AI generation are handled as an event-driven workflow:
* **Orchestration**: MassTransit manages a multi-stage process (Metadata -> Language Code -> Test Cases).
* **Feedback Loop**: Users receive granular updates via SignalR as each component of the task is successfully generated.

---

## Engineering Outcomes

* **Horizontal Scalability**: Centralized state management in Redis and RabbitMQ allows for seamless cluster deployment.
* **Reliability**: Asynchronous processing and persistence patterns ensure that system messages and AI tasks are resilient to server restarts or connection drops.
* **Maintainability**: Clear separation of layers allows for infrastructure swaps without modifying domain logic.
