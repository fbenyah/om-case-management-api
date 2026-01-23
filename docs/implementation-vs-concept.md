# Implementation vs Concept — Case Management System

This document summarizes a focused comparison between the conceptual model in
`docs/case-management-system-README.md` (the authoritative concept doc you provided)
and the current implementation found under `src/` in this repository. It lists
what matches, what doesn't, evidence (file paths), and prioritized next steps.

Contract
- Inputs: `docs/case-management-system-README.md` (concept) and the code under `src/`.
- Outputs: A concise mapping of Concept → Implementation with evidence and actionable recommendations.
- Success criteria: Every major README concept is marked Present / Partially Present / Missing with file evidence and a recommended next action.

Checklist (what I checked)
- Domain entities and DTO/mapping: Case, Interaction, Transaction
- Enums/state definitions: InteractionStatus, TransactionStatus, CaseStatus
- Consent model and state machine for Consent
- Case submission flow (SubmitCase) and consent enforcement
- Event model (domain events + publishers)
- Reference-number generator and seed data
- API surface: controllers and commands for Case/Interaction/Transaction
- Seed data and example graphs

Executive summary
- Present in code: Interaction, Transaction, Case entities; DTOs and mappers; controllers for create/get; enums for statuses; a ReferenceNumberGenerator utility; database seed demonstrating cases/interactions/transactions.
- Not present / mismatches: Consent aggregate and consent lifecycle (missing); no explicit SubmitCase flow or "case-at-submission-only" behaviour; no domain event classes/publishers; no state-machine enforcement (only enums); reference-number format mismatch between README examples and generator/seed.

High-level mapping (concept → status + evidence)
- Interaction — Present (but coupled to Case)
  - Status: Present (Partially matching)
  - Evidence: `src/om.servicing.casemanagement.domain/Entities/OMInteraction.cs`
  - Notes: Interaction entity includes `CaseId` and a `Case` navigation property (code ties interactions to cases). README expects interactions to never "submit" or own cases.

- Transaction — Present
  - Status: Present (Partially matching)
  - Evidence: `src/om.servicing.casemanagement.domain/Entities/OMTransaction.cs`, Create command: `src/om.servicing.casemanagement.application/Features/OMTransactions/Commands/CreateOMTransactionCommand.cs`
  - Notes: Transactions reference both Interaction and Case. README expects transactions to be initiated by interactions and only bundled into a Case at submission; the code allows transactions associated with a Case earlier and does not enforce immutability after submission.

- Consent — Missing (critical)
  - Status: Missing
  - Evidence: Full-text search returned no `Consent` symbol under `src/`. README and diagrams contain Consent, e.g. `docs/diagrams/Conceptual Class Diagram — Interaction Transaction Case (DOT required).puml`.
  - Impact: Compliance/behaviour in README (consent gates submission) is not implemented.

- Case lifecycle & submission model — Mismatch
  - Status: Mismatch
  - Evidence: Case APIs and handlers exist: `src/om.servicing.casemanagement.api/Controllers/V1/CaseContoller.cs`, `src/om.servicing.casemanagement.application/Features/OMCases/Commands/CreateOMCaseCommand.cs`.
  - Notes: README: "A case is created only at submission time" — code exposes endpoints to create cases (shell/full) before any consent/submission flow. No `SubmitCase` or `ConsentRequested` handlers found.

- State machines & enforcement — Partial
  - Status: Partial
  - Evidence: Enums: `src/om.servicing.casemanagement.domain/Enums/InteractionStatus.cs`, `.../TransactionStatus.cs`, `.../CaseStatus.cs`.
  - Notes: Enums exist but there is no state-machine library or explicit transition enforcement. Consent states are absent.

- Reference number strategy — Present but inconsistent
  - Status: Partial / Mismatch
  - Evidence: Generator: `src/om.servicing.casemanagement.domain/Utilities/ReferenceNumberGenerator.cs`; Seed uses `CASE-0001`, `INT-0001-1`, `TX-0001-1` in `src/om.servicing.casemanagement.data/Seed/MigrationDummyData.cs`.
  - Notes: Generator format (OBS + channel prefix + timestamp + random + ULID suffix) does not match README examples (`<ENTITY>-<CHANNEL>-<YYYYMMDD>-<SEQUENCE>`). Seed data uses simple formatted strings that do not match generator output.

- Event model — Missing
  - Status: Missing
  - Evidence: No domain event classes, IEvent, or publisher usages found in `src/` (search for "Event" returned only build assets). README lists core events (InteractionStarted, TransactionCreated, ConsentRequested, ConsentGranted, CaseSubmitted, ...).

- Seed examples & tests
  - Status: Present (seed) / Tests not covering missing behaviour
  - Evidence: `src/om.servicing.casemanagement.data/Seed/MigrationDummyData.cs` shows two seeded case graphs (cases, interactions, transactions). I did not find tests exercising consent/submission flows.

Concrete mismatches & risks (prioritised)
1. Consent absent (HIGH) — README's compliance model depends on this. Risk: product/QA expectations mismatch, compliance gap.
2. Submit/consent flow absent (HIGH) — No `SubmitCase` or `ConsentRequested` endpoints or handlers.
3. Event-driven architecture missing (MEDIUM) — README expects events; current codebase appears imperative but not event-published.
4. State-machine enforcement absent (MEDIUM) — enums only; no transition guards.
5. Reference number format inconsistent (LOW-MEDIUM) — generator and seed/docs disagree; could cause confusion in integration.
6. Aggregate boundary differences (MEDIUM) — interactions reference case, mixing ownership compared to conceptual model.

Recommended next steps (short-term, low risk)
- Option A (document-first, low-effort): Update the conceptual README to mark Consent and SubmitCase as "Not implemented" and list them as roadmap items. This prevents stakeholders from assuming those behaviours exist.
- Option B (implement-first, higher effort): Implement the missing pieces to match the README:
  1. Add `OMConsent` entity and `ConsentStatus` enum.
  2. Create `RequestConsent` and `SubmitCase` application commands/handlers. `RequestConsent` should create the consent record and emit `ConsentRequested`; `SubmitCase` should validate consent==GRANTED, create/mark case as SUBMITTED and bundle transactions.
  3. Add domain events and a publisher abstraction and raise events from handlers.
  4. Enforce transaction immutability after submission (domain rule + DB considerations).
  5. Add tests for consent and submission flows and event emission.

Appendix — quick reference to inspected files (evidence)
- Domain entities
  - `src/om.servicing.casemanagement.domain/Entities/OMInteraction.cs`
  - `src/om.servicing.casemanagement.domain/Entities/OMTransaction.cs`
  - `src/om.servicing.casemanagement.domain/Entities/OMCase.cs`
  - `src/om.servicing.casemanagement.domain/Entities/BaseEntity.cs`
- DTOs & mappings
  - `src/om.servicing.casemanagement.domain/Mappings/DtoToEntityMapper.cs`
  - `src/om.servicing.casemanagement.domain/Mappings/EntityToDtoMapper.cs`
- Enums
  - `src/om.servicing.casemanagement.domain/Enums/InteractionStatus.cs`
  - `src/om.servicing.casemanagement.domain/Enums/TransactionStatus.cs`
  - `src/om.servicing.casemanagement.domain/Enums/CaseStatus.cs`
- Controllers / API
  - `src/om.servicing.casemanagement.api/Controllers/V1/CaseContoller.cs`
  - `src/om.servicing.casemanagement.api/Controllers/V1/InteractionController.cs`
  - `src/om.servicing.casemanagement.api/Controllers/V1/TransactionController.cs`
- Commands & Handlers
  - `src/om.servicing.casemanagement.application/Features/OMTransactions/Commands/CreateOMTransactionCommand.cs`
  - `src/om.servicing.casemanagement.application/Features/OMCases/Commands/CreateOMCaseCommand.cs`
- Utilities & seed
  - Reference generator: `src/om.servicing.casemanagement.domain/Utilities/ReferenceNumberGenerator.cs`
  - Seed data: `src/om.servicing.casemanagement.data/Seed/MigrationDummyData.cs`
- Diagrams & conceptual model
  - `docs/case-management-system-README.md` (concept)
  - `docs/diagrams/Conceptual Class Diagram — Interaction Transaction Case (DOT required).puml`


