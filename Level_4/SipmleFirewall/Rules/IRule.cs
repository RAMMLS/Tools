using SimpleFirewall.Models;

namespace SimpleFirewall.Rules
{
    public interface IRule
    {
        string Name { get; }
        int Priority { get; }
        bool Evaluate(NetworkPacket packet);
        RuleAction GetAction();
    }
}
