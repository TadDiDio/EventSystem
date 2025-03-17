using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace EventSystemAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EventNamespaceAnalyzer : DiagnosticAnalyzer
{
    // Preferred format of DiagnosticId is Your Prefix + Number, e.g. CA1234.
    public const string DiagnosticID = "EventNamespace001";

    // The category of the diagnostic (Design, Naming etc.).
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new
    (
        DiagnosticID,
        "Invalid Event Invocation",
        "Event '{0}' invoked from namespace '{1}' is not allowed. Only allowed from '{2}' or its children.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    // Keep in mind: you have to list your rules here.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // You must call this method to avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        // You must call this method to enable the Concurrent Execution.
        context.EnableConcurrentExecution();

        // Register action for invocation expressions
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax)context.Node;

        // Get symbol for the invoked method.
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpr);
        if (symbolInfo.Symbol is not IMethodSymbol methodSymbol) return;

        // Only process methods named "Invoke"
        if (methodSymbol.Name != "Invoke") return;

        // Ensure we are dealing with a member access expression (e.g., obj.MyEvent.Invoke())
        if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        // Get the receiver of the Invoke call, i.e. "obj.MyEvent"
        var receiverExpression = memberAccess.Expression;

        // Retrieve the symbol for the receiver expression.
        var receiverSymbol = context.SemanticModel.GetSymbolInfo(receiverExpression).Symbol;
        if (receiverSymbol == null) return;

        // Get the type of the receiver.
        var receiverType = context.SemanticModel.GetTypeInfo(receiverExpression).Type;
        if (receiverType == null) return;

        // Only validate if the receiver type implements IGameEvent.
        var iGameEventSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("GameEvents.IGameEvent");
        if (iGameEventSymbol == null || !receiverType.AllInterfaces.Contains(iGameEventSymbol, SymbolEqualityComparer.Default)) return;

        // Determine the "instantiation" namespace by checking the symbol type.
        string eventOwnerNamespace = receiverSymbol switch
        {
            IFieldSymbol fieldSymbol => fieldSymbol.ContainingType.ContainingNamespace.ToString(),
            IPropertySymbol propertySymbol => propertySymbol.ContainingType.ContainingNamespace.ToString(),
            _ => receiverType.ContainingNamespace.ToString()
        };

        // Get the namespace of the caller (where the Invoke call is made).
        var callerNamespaceDeclaration = invocationExpr.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault();
        var callerNamespace = callerNamespaceDeclaration?.Name.ToString();

        // Check if the caller's namespace is allowed.
        if (!IsNamespaceAllowed(eventOwnerNamespace, callerNamespace))
        {
            string receiverName = receiverSymbol.Name;
            var diagnostic = Diagnostic.Create(
                Rule,
                invocationExpr.GetLocation(),
                receiverName,
                callerNamespace,
                eventOwnerNamespace);
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Returns true if callerNamespace equals ownerNamespace or is a subnamespace of ownerNamespace.
    /// </summary>
    private bool IsNamespaceAllowed(string ownerNamespace, string callerNamespace)
    {
        if (callerNamespace == null) return false;

        // Allowed if the caller's namespace is exactly the owner namespace...
        // or if it begins with the owner namespace followed by a dot.
        return callerNamespace.Equals(ownerNamespace) || callerNamespace.StartsWith(ownerNamespace + ".");
    }
}
