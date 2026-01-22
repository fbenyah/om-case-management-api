# Case Management System — Conceptual & Interaction Model

## Overview

This document describes the **conceptual model, interaction flows, and lifecycle rules** for the Case Management System.

The system is designed to support **assisted channels** (Call Center, Branch) and **unassisted digital channels** (Web, App, WhatsApp) using a **single, consistent model** for:

* Interactions
* Transactions
* Consent
* Case creation and submission

The goal is to ensure **traceability, auditability, and consistency** across all customer touchpoints.

---

## Core Concepts

### 1. Interaction

An **Interaction** represents a customer touchpoint with the organisation. Every customer journey begins with one or more interactions.

Examples:

* IVR Interaction (before an agent answers a call)
* Agent Interaction (call center)
* Branch Interaction (walk-in)
* Digital Interaction (web, app, WhatsApp)

Key properties:

* Has a unique `interactionRef`
* Has a channel and timestamps
* May transfer to another interaction
* Can initiate zero or more transactions

> Interactions **do not submit cases**.

---

### 2. Transaction

A **Transaction** represents a business request initiated by a customer during an interaction.

Rules:

* Every transaction is initiated by **exactly one interaction**
* Transactions cannot exist independently
* Transactions may be created across multiple interactions

Examples:

* Policy update
* Claim submission
* Account change

---

### 3. Consent

**Consent** is a mandatory gate before case submission.

Rules:

* Consent is requested **after all transactions are captured**
* Exactly one consent decision exists per case
* Consent outcomes:

  * `GRANTED`
  * `DECLINED`

Effects:

* If consent is granted → case may be submitted
* If consent is declined → case is cancelled

Consent is auditable and time-bound.

---

### 4. Case

A **Case** represents the formal submission of one or more transactions.

Rules:

* A case is created **only at submission time**
* A case always contains **one or more transactions**
* A case cannot be submitted without consent
* Every submitted case produces a **case reference number**

Case outcomes:

* `SUBMITTED`
* `CANCELLED`

The **case reference number** is the primary customer-facing identifier.

---

## Channel Behaviour

### Call Center Channel

Flow:

1. Customer call arrives
2. IVR interaction recorded
3. Agent interaction begins
4. Transactions are created
5. Optional agent-to-agent transfer(s)
6. Additional transactions may be created
7. Customer consent is requested
8. Case is submitted or cancelled
9. Customer receives case reference number (if submitted)

Key points:

* IVR and Agent interactions are distinct
* Transfers create **interaction-to-interaction links**, not new cases
* Transactions remain linked to the interaction that initiated them

---

### Branch Channel

Flow:

1. Customer walks into branch
2. Branch interaction recorded
3. Branch agent interaction begins
4. Transactions are created
5. Optional branch agent-to-agent handover(s)
6. Customer consent is requested
7. Case is submitted or cancelled
8. Customer receives case reference number (if submitted)

The same rules as the call center apply, without IVR.

---

### Digital Channels (Web / App / WhatsApp)

Flow:

1. Customer starts a digital session
2. Digital interaction recorded
3. Single transaction is submitted
4. Customer is prompted for consent
5. Case is submitted or cancelled
6. Customer receives case reference number (if submitted)

Constraints:

* Only **one transaction per case**
* No agent interactions
* No interaction transfers

---

## Interaction Transfers

Transfers represent a **handover of responsibility**, not a new case.

Examples:

* Agent → Agent (call center)
* Branch Agent → Branch Agent

Rules:

* Transfers link one interaction to another
* Transactions remain linked to the interaction that created them
* The case is shared across all interactions involved

---

## Unified Lifecycle Summary

1. One or more interactions occur
2. One or more transactions are created
3. Customer consent is requested
4. If consent granted:

   * Case is created
   * Transactions are bundled
   * Case is submitted
   * Case reference number is provided to customer
5. If consent declined:

   * Case is cancelled
   * Customer is notified

---

## Design Principles

* Channel-agnostic core model
* Explicit separation of interactions, transactions, and cases
* Consent-first compliance
* Full traceability across transfers
* Single submission point per case

---

## Future Extensions (Not Yet Implemented)

* Consent versioning and wording history
* Case state machine (`DRAFT`, `AWAITING_CONSENT`, etc.)
* Event-driven architecture (`TransactionCreated`, `ConsentGranted`)
* Resume-later / abandoned journey handling
* Reporting and audit views

---

## Diagram Index

The following diagrams accompany this README:

* Conceptual Class Diagram — Interactions, Transactions, Consent, Cases
* Call Center Sequence Diagram — Consent-Gated Case Submission
* Branch Sequence Diagram — Consent-Gated Case Submission
* Digital Channel Sequence Diagram — Consent-Gated Case Submission
* Unified Activity Diagram — End-to-End Lifecycle

---

**This document is the source of truth for the Case Management System conceptual model.**
