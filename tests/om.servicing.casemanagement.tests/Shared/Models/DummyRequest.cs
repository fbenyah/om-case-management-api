using MediatR;

namespace om.servicing.casemanagement.tests.Shared.Models;

public class DummyRequest : IRequest<string> { public string Value { get; set; } }

public class DummyResponse { }
