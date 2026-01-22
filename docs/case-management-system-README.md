# Case Management System — Conceptual, Behavioural & Architectural Model

## Overview

This document is the **single source of truth** for the Case Management System. It captures the **conceptual model, behavioural flows, state machines, reference standards, event model, and DDD aggregate boundaries**.

The system supports **assisted channels** (Call Center, Branch) and **unassisted digital channels** (Web, App, WhatsApp) using one consistent, channel-agnostic model.

Primary goals:

* Traceability across all customer touchpoints
* Explicit consent-driven submission
* Strong audit and compliance support
* Clear ownership and lifecycle boundaries

---

## Core Domain Concepts

### 1. Interaction

An **Interaction** represents a single customer touchpoint with the organisation.

Examples:

* IVR Interaction (before an agent answers)
* Agent Interaction (call center)
* Branch Interaction (walk-in)
* Digital Interaction (web, app, WhatsApp)

Rules:

* Every customer journey starts with one or more interactions
* Interactions may transfer responsibility to other interactions
* Interactions can initiate zero or more transactions
* Interactions never submit cases

Key attributes:

* `interactionRef`
* `channel`
* `startedAt`, `endedAt`

---

### 2. Transaction

A **Transaction** represents a discrete business request raised by a customer during an interaction.

Rules:

* Every transaction is initiated by **exactly one interaction**
* Transactions cannot exist independently
* Transactions may span multiple interactions via transfers
* Transactions are immutable once included in a submitted case

Key attributes:

* `transactionRef`
* `type`
* `status`
* `createdAt`

---

### 3. Consent

**Consent** is a mandatory, auditable decision that gates case submission.

Rules:

* Consent is requested **after all transactions are captured**
* Exactly one consent decision exists per case
* Consent outcomes:

  * `GRANTED`
  * `DECLINED`
  * `EXPIRED`

Effects:

* Consent granted → case may be submitted
* Consent declined or expired → case is cancelled

Key attributes:

* `consentRef`
* `status`
* `requestedAt`, `respondedAt`

---

### 4. Case

A **Case** represents the formal submission of one or more transactions.

Rules:

* A case is created only at submission time
* A case always contains one or more transactions
* A case cannot be submitted without consent
* Every submitted case produces a case reference number

Key attributes:

* `caseRef`
* `status`
* `submittedAt`

The **case reference number** is the primary customer-facing identifier.

---

## Channel Behaviour

### Call Center Channel

1. Customer call arrives
2. IVR interaction recorded
3. Agent interaction begins
4. Transactions are created
5. Optional agent-to-agent transfer(s)
6. Additional transactions may be created
7. Customer consent is requested
8. Case is submitted or cancelled
9. Customer receives case reference number (if submitted)

Notes:

* IVR and agent interactions are distinct
* Transfers link interactions, not cases
* Transactions remain linked to their initiating interaction

---

### Branch Channel

1. Customer walks into branch
2. Branch interaction recorded
3. Branch agent interaction begins
4. Transactions are created
5. Optional branch agent handover(s)
6. Customer consent is requested
7. Case is submitted or cancelled
8. Customer receives case reference number (if submitted)

---

### Digital Channels (Web / App / WhatsApp)

1. Customer starts a digital session
2. Digital interaction recorded
3. Single transaction is submitted
4. Customer is prompted for consent
5. Case is submitted or cancelled
6. Customer receives case reference number (if submitted)

Constraints:

* One transaction per case
* No agents
* No interaction transfers

---

## Unified Lifecycle Summary

1. One or more interactions occur
2. One or more transactions are created
3. Customer consent is requested
4. If consent granted:

   * Case is created
   * Transactions are bundled
   * Case is submitted
   * Case reference number is provided
5. If consent declined or expired:

   * Case is cancelled
   * Customer is notified

---

## State Machines

### Interaction States

* `STARTED`
* `IN_PROGRESS`
* `TRANSFERRED`
* `ENDED`

---

### Transaction States

* `CREATED`
* `VALIDATED`
* `INCLUDED_IN_CASE`

---

### Consent States

* `REQUESTED`
* `GRANTED`
* `DECLINED`
* `EXPIRED`

---

### Case States

* `DRAFT`
* `AWAITING_CONSENT`
* `SUBMITTED`
* `CANCELLED`

---

## Reference Number Strategy

Reference numbers are immutable, unique, and human-readable.

### Format

```
<ENTITY>-<CHANNEL>-<YYYYMMDD>-<SEQUENCE>
```

### Examples

* Interaction: `INT-CALL-20260122-000123`
* IVR Interaction: `IVR-CALL-20260122-000045`
* Agent Interaction: `AGT-CALL-20260122-000067`
* Branch Interaction: `BRN-BRANCH-20260122-000031`
* Digital Interaction: `DIG-WEB-20260122-000812`
* Transaction: `TXN-GENERIC-20260122-004921`
* Consent: `CNS-20260122-000334`
* Case: `CASE-20260122-000118`

Rules:

* Generated once
* Never reused
* Never mutated
* Case reference is customer-facing

---

## Event Model

The system is designed to support an **event-driven architecture**.

### Core Events

* `InteractionStarted`
* `InteractionTransferred`
* `TransactionCreated`
* `ConsentRequested`
* `ConsentGranted`
* `ConsentDeclined`
* `ConsentExpired`
* `CaseSubmitted`
* `CaseCancelled`

Events are immutable and fully auditable.

---

## DDD Aggregate Boundaries

### Interaction Aggregate

**Aggregate Root:** Interaction

Owns:

* Interaction lifecycle
* Transfers
* Authority to create transactions

Does not own:

* Case
* Consent

---

### Case Aggregate

**Aggregate Root:** Case

Owns:

* Transaction inclusion
* Consent
* Submission and cancellation

Guarantees:

* No submission without consent
* Immutable transaction set after submission

---

## Design Principles

* Channel-agnostic core domain
* Explicit separation of concerns
* Consent-first compliance model
* Strong auditability
* Clear aggregate ownership

---

## Diagram Index

* Conceptual Class Diagram — Interactions, Transactions, Consent, Cases
* Sequence Diagrams — Call Center, Branch, Digital
* Activity Diagram — Unified Lifecycle
* State Diagrams — Interaction, Transaction, Consent, Case
* Aggregate Boundary Diagram — DDD Model

---

**This document defines the authoritative conceptual and architectural model for the Case Management System.**
