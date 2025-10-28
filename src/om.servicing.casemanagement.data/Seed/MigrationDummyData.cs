using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.data.Seed;

public static class MigrationDummyData
{
    /// <summary>
    /// Returns seed collections for two example cases (with interactions and transactions) you can pass into
    /// modelBuilder.Entity&lt;T&gt;().HasData(...) when creating a migration.
    /// - Case 1: two interactions (first is primary, second points to first), one Policy transaction on first interaction.
    /// - Case 2: one interaction, two transactions: a POCR resolved immediately and a long-running external transaction.
    /// </summary>
    /// <param name="createdDate">Optional created date to use for all seed rows (default = UtcNow). Use a fixed date to keep migrations deterministic.</param>
    public static (IEnumerable<OMCase> Cases, IEnumerable<OMInteraction> Interactions, IEnumerable<OMTransaction> Transactions) GetTwoCaseGraph(DateTime? createdDate = null)
    {
        var now = createdDate ?? DateTime.UtcNow;

        // these are taken from the migration transaction type seed to ensure FK consistency
        string txTypePolicyId = "01JFJ0R4E5SK1Q7HBS9D5RX2CP";
        string txTypePocrId = "01JFJ0R4E4MTHQ4KSNVQ5H1K3W";

        string case1Id = "01K8PEVXGNPEHC9PYZDRDHMFW2";
        string case2Id = "01K8PEVXGNTF2YWGY9BNYWST6N";

        string interaction1Case1Id = "01K8PEVXGN1PYYFTNT8XYAAMCS";
        string interaction2Case1Id = "01K8PEVXGNPBRVW4WBSCRYCFDN";
        string interaction1Case2Id = "01K8PEVXGNAQM83WN75WT8QSYK";

        string transactionCase1PolicyId = "01K8PEVXGNK2PXA93FMVDPPC60";
        string transactionCase2PocrId = "01K8PEVXGPRPP0YDYFHHAH1SXC";
        string transactionCase2ExternalId = "01K8PEVXGPYWWVBEHA37C3NPA4";

        var cases = new List<OMCase>
        {
            new OMCase
            {
                Id = case1Id,
                Channel = "PublicWeb",
                IdentificationNumber = "ID-1001",
                CreatedDate = now,
                ReferenceNumber = "CASE-0001",
                Status = "Open"
            },
            new OMCase
            {
                Id = case2Id,
                Channel = "ContactCentre",
                IdentificationNumber = "ID-2002",
                CreatedDate = now,
                ReferenceNumber = "CASE-0002",
                Status = "Open"
            }
        };

        var interactions = new List<OMInteraction>
        {
            // Case 1 - primary interaction
            new OMInteraction
            {
                Id = interaction1Case1Id,
                CaseId = case1Id,
                Notes = "Initial contact - captured details",
                IsPrimaryInteraction = true,
                PreviousInteractionId = string.Empty,
                CreatedDate = now,
                ReferenceNumber = "INT-0001-1",
                Status = "Initiated"
            },

            // Case 1 - follow-up interaction referencing first interaction
            new OMInteraction
            {
                Id = interaction2Case1Id,
                CaseId = case1Id,
                Notes = "Follow up, references prior interaction",
                IsPrimaryInteraction = false,
                PreviousInteractionId = interaction1Case1Id,
                CreatedDate = now,
                ReferenceNumber = "INT-0001-2",
                Status = "Initiated"
            },

            // Case 2 - single interaction
            new OMInteraction
            {
                Id = interaction1Case2Id,
                CaseId = case2Id,
                Notes = "Customer call - requested beneficiary update",
                IsPrimaryInteraction = true,
                PreviousInteractionId = string.Empty,
                CreatedDate = now,
                ReferenceNumber = "INT-0002-1",
                Status = "Initiated"
            }
        };

        var transactions = new List<OMTransaction>
        {
            // Case1: Policy transaction created on first interaction
            new OMTransaction
            {
                Id = transactionCase1PolicyId,
                CaseId = case1Id,
                InteractionId = interaction1Case1Id,
                TransactionTypeId = txTypePolicyId,
                IsImmediate = false,
                IsFulfilledExternally = false,
                ReceivedDetails = "Policy change request received",
                ProcessedDetails = "Policy transaction processed",
                CreatedDate = now,
                ReferenceNumber = "TX-0001-1",
                Status = "Submitted",
                ExternalSystem = string.Empty,
                ExternalSystemId = string.Empty,
                ExternalSystemStatus = string.Empty,
                ExternalSystemParentId = string.Empty,
                ParentReferenceNumber = string.Empty
            },

            // Case2: POCR resolved immediately on the call
            new OMTransaction
            {
                Id = transactionCase2PocrId,
                CaseId = case2Id,
                InteractionId = interaction1Case2Id,
                TransactionTypeId = txTypePocrId,
                IsImmediate = true,
                IsFulfilledExternally = false,
                ReceivedDetails = "Customer issue resolved on call",
                ProcessedDetails = "POCR completed",
                CreatedDate = now,
                ReferenceNumber = "TX-0002-1",
                Status = "Closed",
                ExternalSystem = string.Empty,
                ExternalSystemId = string.Empty,
                ExternalSystemStatus = string.Empty,
                ExternalSystemParentId = string.Empty,
                ParentReferenceNumber = string.Empty
            },

            // Case2: Long running transaction fulfilled by external system
            new OMTransaction
            {
                Id = transactionCase2ExternalId,
                CaseId = case2Id,
                InteractionId = interaction1Case2Id,
                TransactionTypeId = txTypePolicyId, // reuse policy type for example
                IsImmediate = false,
                IsFulfilledExternally = true,
                ReceivedDetails = "Request to update beneficiaries",
                ProcessedDetails = "Sent to external system for processing",
                CreatedDate = now,
                ReferenceNumber = "TX-0002-2",
                Status = "Submitted",
                ExternalSystem = "ExternalSys",
                ExternalSystemId = "EXT-12345",
                ExternalSystemStatus = "Pending",
                ExternalSystemParentId = string.Empty,
                ParentReferenceNumber = string.Empty
            }
        };

        return (cases, interactions, transactions);
    }
}
