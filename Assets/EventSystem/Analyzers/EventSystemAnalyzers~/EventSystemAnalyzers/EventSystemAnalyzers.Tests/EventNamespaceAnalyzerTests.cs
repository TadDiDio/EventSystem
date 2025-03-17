using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace EventSystemAnalyzers.Tests;

public class EventNamespaceAnalyzerTests
{
    // This helper is typically defined in your test project to wrap the analyzer test execution.
    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
    {
        var test = new CSharpAnalyzerTest<EventNamespaceAnalyzer, XUnitVerifier>
        {
            TestCode = source,
        };
        test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Test_InvalidNamespaceInvocation_ShouldTriggerDiagnostic()
    {
        var testCode = @"
namespace GameEvents
{
    public interface IGameEvent
    {
        void Invoke();
    }
    public class GameEvent : IGameEvent
    {
        public void Invoke()
        {
            // Do the invoke
        }
    }
}
namespace Game.Allowed
{
    public class Container
    {
        public GameEvents.GameEvent MyEvent = new();
    }
    public class AllowedInvoker
    {
        void Start()
        {
            Container c = new();
            c.MyEvent.Invoke();
        }
    }
}
namespace DisallowedNamespace
{
    public class SomeClass
    {
        public void Method()
        {
            var c = new Game.Allowed.Container();
            c.MyEvent.Invoke(); // Expected diagnostic: invalid invocation from DisallowedNamespace.
        }
    }
}
namespace Game.Disallowed
{
    public class SomeClass
    {
        public void Method()
        {
            var c = new Game.Allowed.Container();
            c.MyEvent.Invoke(); // Expected diagnostic: invalid invocation from DisallowedNamespace.
        }
    }
}
public class Test
{
    public void Method()
    {
        var c = new Game.Allowed.Container();
        c.MyEvent.Invoke();
    }
}
";


        var expected1 = new DiagnosticResult("EventNamespace001", DiagnosticSeverity.Error)
            .WithMessage("Event 'MyEvent' invoked from namespace 'DisallowedNamespace' is not allowed. Only allowed from 'Game.Allowed' or its children.")
            .WithSpan( 38,  13, 38,  31);

        var expected2 = new DiagnosticResult("EventNamespace001", DiagnosticSeverity.Error)
            .WithMessage("Event 'MyEvent' invoked from namespace 'Game.Disallowed' is not allowed. Only allowed from 'Game.Allowed' or its children.")
            .WithSpan( 49,  13, 49,  31);

        var expected3 = new DiagnosticResult("EventNamespace001", DiagnosticSeverity.Error)
            .WithMessage("Event 'MyEvent' invoked from namespace '' is not allowed. Only allowed from 'Game.Allowed' or its children.")
            .WithSpan( 58,  9, 58,  27);
        
        await VerifyAnalyzerAsync(testCode, expected1, expected2, expected3);
    }

    [Fact]
    public async Task Test_ValidNamespaceInvocations_ShouldNotTriggerDiagnostic()
    {
        var testCode = $@"
namespace GameEvents
{{
    public interface IGameEvent
    {{
        void Invoke();
    }}
    public class GameEvent : IGameEvent
    {{
        public void Invoke()
        {{
            // Do the invoke
        }}
    }}
    public class GameEvent<T> : IGameEvent
    {{
        public void Invoke()
        {{
            // Do the invoke
        }}
        public void Invoke(T value)
        {{
            // Do the invoke
        }}
    }}
    public class Events
    {{
        private static Events _instance;

        // Delete the constructor to prevent duplicates
        private Events() {{ }}

        /// <summary>
        /// A singleton property giving access to all game events.
        /// </summary>
        public static Events Instance => _instance ??= new Events();

        public Game.Character.Player.PlayerEvents PlayerEventsContainer = new();
        public Game.Character.CharacterEvents CharacterEventsContainer = new();
    }}
}}

namespace Game.Character.Player
{{
    public class PlayerEvents
    {{
        public GameEvents.GameEvent PlayerEvent = new();
    }}
    public class Player
    {{
        void Start()
        {{
            GameEvents.Events.Instance.PlayerEventsContainer.PlayerEvent.Invoke();
            GameEvents.Events.Instance.CharacterEventsContainer.CharacterEvent.Invoke();
        }}
    }}
}}

namespace Game.Character
{{
    public class CharacterEvents
    {{
        public GameEvents.GameEvent CharacterEvent = new();
    }}
    public class Character
    {{
        void Start()
        {{
            GameEvents.Events.Instance.CharacterEventsContainer.CharacterEvent.Invoke();
        }}
    }}
}}
";

        await VerifyAnalyzerAsync(testCode);
    }
}
