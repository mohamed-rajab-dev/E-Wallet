# E-Wallet

A backend **E-Wallet API built with ASP.NET Core** that provides authentication, wallet management, deposits, withdrawals, money transfers, transaction history, and ATM operations.

The project focuses on practical backend engineering problems such as **financial consistency, duplicate requests, concurrent updates, authentication, security, and API reliability**.

---

## Features

* User registration and login
* JWT authentication
* Refresh tokens
* Wallet management
* Deposits and withdrawals
* Money transfers
* Transaction history
* Dynamic OTP for ATM operations
* Idempotency
* Database transactions
* Optimistic concurrency control
* Sliding Window rate limiting
* Input validation

---

## Architecture

The project follows a layered architecture:

```text
E-Wallet
├── E-Wallet.Api
├── E-Wallet.Application
├── E-Wallet.Domain
└── E-Wallet.Infrastructure
```

| Layer              | Responsibility                                                                |
| ------------------ | ----------------------------------------------------------------------------- |
| **Api**            | Controllers, HTTP requests/responses, endpoints, authentication configuration |
| **Application**    | Business logic, services, use cases, validation                               |
| **Domain**         | Entities, enums, and business rules                                           |
| **Infrastructure** | EF Core, SQL Server, repositories, Identity persistence                       |

This separation reduces coupling and keeps business logic independent from HTTP and infrastructure concerns.

---

## Database Design

The project uses **SQL Server** with Entity Framework Core.

Main entities:

| Entity        | Purpose                           |
| ------------- | --------------------------------- |
| Users         | User information                  |
| Wallets       | Wallet and balance information    |
| Banks         | Supported banks                   |
| Transactions  | Financial transaction records     |
| DynamicOtps   | OTP records                       |
| AtmOperations | ATM deposit/withdrawal operations |
| Logs          | Relevant system logs              |

```text
User
 │
 └── Wallet
      ├── Transactions
      ├── DynamicOtps
      └── AtmOperations

Bank
 │
 └── AtmOperations
```

---

## Why REST Instead of SOAP?

REST was selected because the system primarily communicates with web/mobile clients through HTTP.

| Aspect        | REST                | SOAP                   |
| ------------- | ------------------- | ---------------------- |
| Style         | Architectural style | Protocol               |
| Common format | JSON                | XML                    |
| Complexity    | Generally simpler   | More formal/verbose    |
| Communication | HTTP-based APIs     | Commonly XML messaging |
| This project  | Selected            | Not selected           |

The decision is requirement-driven rather than claiming that REST is universally better than SOAP.

---

## Why SQL Server Instead of NoSQL?

A wallet system has strongly related data and requires reliable transactional operations.

SQL Server provides:

* ACID transactions
* Foreign keys
* Referential integrity
* Strong relationships
* Structured data
* Concurrency support
* Complex queries

For example, a transfer needs to update multiple pieces of data atomically:

```text
Sender Balance
      ↓
   Debit
      ↓
Receiver Balance
      ↓
   Credit
      ↓
Transaction Records
```

NoSQL can be appropriate for other workloads, but SQL Server fits the relational and transactional requirements of this project.

---

## Authentication

```text
Register
   ↓
Login
   ↓
Access Token + Refresh Token
   ↓
API Requests
   ↓
Access Token Expires
   ↓
Refresh Token
   ↓
New Access Token
```

### Access Token

A short-lived JWT used to authenticate API requests:

```http
Authorization: Bearer <access-token>
```

### Refresh Token

A longer-lived credential used to obtain a new access token without requiring the user to log in again.

The two tokens have different purposes:

```text
Access Token  → Authenticate API requests
Refresh Token → Obtain a new access token
```

---

## Async/Await and Concurrency

Database operations are asynchronous because they are primarily **I/O-bound**.

```csharp
var wallet = await walletRepository.GetByIdAsync(walletId);
```

`async/await` does not automatically create a new thread for every request.

While waiting for database I/O, the thread can return to the thread pool and handle other work.

```text
Request A → Database I/O
              ↓
         Thread available
              ↓
Request B → Database I/O
```

Important distinctions:

```text
Async ≠ Parallel
Async ≠ Multiple Threads
Concurrency ≠ Parallelism
```

---

## Database Transactions

Financial operations must be atomic.

A transfer is handled conceptually as:

```text
BEGIN TRANSACTION
       ↓
Debit Sender
       ↓
Credit Receiver
       ↓
Create Transaction Records
       ↓
Save Changes
       ↓
     Commit
```

If an operation fails:

```text
Failure
  ↓
Rollback
```

This prevents a situation where the sender is debited but the receiver is not credited.

---

## Optimistic Concurrency

Two requests may attempt to modify the same wallet simultaneously:

```text
Initial Balance = 1000 EGP

Request A → Withdraw 800
Request B → Withdraw 800
```

The project uses optimistic concurrency to detect stale updates.

Conceptually:

```text
Request A
   ↓
Read Version 5
   ↓
Update WHERE Version = 5
   ↓
Success → Version 6


Request B
   ↓
Read Version 5
   ↓
Update WHERE Version = 5
   ↓
No rows affected
   ↓
DbUpdateConcurrencyException
```

This prevents stale data from silently overwriting a newer update.

---

## Idempotency

A network retry or duplicate click can cause the same transfer request to be submitted twice.

The client generates an idempotency key:

```http
Idempotency-Key: ABC-123
```

For the same logical operation:

```text
First Request → ABC-123
Retry         → ABC-123
```

A new logical operation receives a new key.

The backend stores/checks the key, while a unique database constraint provides an additional guarantee against duplicate processing.

### Idempotency vs Concurrency

```text
Idempotency
→ Prevents repeated logical requests

Optimistic Concurrency
→ Detects conflicting simultaneous updates
```

They solve different problems and are used together.

---

## Dynamic OTP

ATM operations require a dynamic OTP.

```text
Generate OTP
     ↓
OTP Validation
     ↓
Validate Wallet
     ↓
Validate Amount
     ↓
Deposit / Withdrawal
     ↓
Create Transaction
     ↓
Complete Operation
```

OTP records have a lifecycle that includes expiration and prevention of reuse.

---

## Rate Limiting

The API uses **Sliding Window rate limiting** to control excessive requests.

Example:

```text
Limit: 10 requests / 60 seconds
```

Unlike a fixed window, the server evaluates requests over a moving time period.

Rate limiting helps with:

* Request bursts
* API abuse
* Brute-force mitigation
* Resource protection

It does not replace authentication, authorization, validation, or other security mechanisms.

---

## Money Transfer Flow

```text
Client
  ↓
JWT Authentication
  ↓
Validate Request
  ↓
Validate Idempotency-Key
  ↓
Load Sender & Receiver
  ↓
Begin Transaction
  ↓
Debit Sender
  ↓
Credit Receiver
  ↓
Create Transaction Records
  ↓
Save Changes
  ↓
Concurrency Check
  ↓
Commit
```

Possible failures include:

* Sender or receiver not found
* Insufficient balance
* Duplicate idempotency key
* Concurrency conflict
* Database failure

---

## API Endpoints

### Authentication

```http
POST /api/user/register
POST /api/user/login
POST /api/user/refresh-token
```

### Transfer

```http
POST /api/transaction/transfer
```

Headers:

```http
Authorization: Bearer <access-token>
Idempotency-Key: <unique-key>
```

Example:

```json
{
  "receiverEmail": "hossam@example.com",
  "amount": "200",
  "senderPassword": "Password123!"
}
```

### Generate Dynamic OTP

```http
POST /transaction/generate/otp
```

```http
Authorization: Bearer <access-token>
```

### ATM Withdrawal

```http
POST /api/transaction/atm/operation/withdrawal
```

```http
Authorization: Bearer <access-token>
atmId: <atm-id>
bankName: <bank-name>
Idempotency-Key: <unique-key>
```

### ATM Deposit

```http
POST /api/transaction/atm/operation/deposit
```

```http
Authorization: Bearer <access-token>
atmId: <atm-id>
bankName: <bank-name>
Idempotency-Key: <unique-key>
```

---

## Security Considerations

The project uses multiple mechanisms, each addressing a different concern:

| Mechanism             | Purpose                                  |
| --------------------- | ---------------------------------------- |
| JWT                   | Authentication                           |
| Refresh Tokens        | Renew expired access tokens              |
| ASP.NET Core Identity | Password hashing and identity management |
| Authorization         | Control access to operations             |
| Dynamic OTP           | Additional operation verification        |
| Rate Limiting         | Control excessive requests               |
| Input Validation      | Reject invalid input                     |
| Idempotency           | Prevent duplicate operations             |
| Transactions          | Atomic financial changes                 |
| Concurrency Control   | Detect conflicting updates               |

---

## Engineering Problems & Solutions

| Problem                      | Solution                     |
| ---------------------------- | ---------------------------- |
| Duplicate financial requests | Idempotency                  |
| Simultaneous wallet updates  | Optimistic concurrency       |
| Partial transfers            | Database transactions        |
| Expired access tokens        | Refresh tokens               |
| Excessive requests           | Sliding Window rate limiting |
| OTP reuse                    | Expiration and OTP state     |
| Coupled business logic       | Layered architecture         |

---

## Technologies

* **C#**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **ASP.NET Core Identity**
* **JWT**
* **FluentValidation**
* **Otp.Net**
* **MailKit / MimeKit**
* **Mapster**

---

## Key Design Decisions

The project intentionally combines several backend patterns:

```text
REST
 ↓
Simple HTTP API communication

SQL Server
 ↓
Relational data + transactions

JWT + Refresh Tokens
 ↓
Stateless authentication lifecycle

Database Transactions
 ↓
Atomic financial operations

Optimistic Concurrency
 ↓
Safe handling of simultaneous updates

Idempotency
 ↓
Safe handling of retries and duplicate requests

Sliding Window
 ↓
Request-rate control

Dynamic OTP
 ↓
Temporary verification for ATM operations
```

The main objective is to demonstrate how a backend can maintain **security, consistency, and reliability when financial operations are exposed through a real-world API**.
