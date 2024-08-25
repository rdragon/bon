using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    internal sealed class MemberTypeGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public MemberTypeGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<ICustomDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                _codeGenerator.StartNewSection();

                foreach (var member in definition.Members)
                {
                    _codeGenerator.AddStatement($"bonFacade.AddMemberType(" +
                        $"{definition.TypeOf}, " +
                        $"{member.Id}, " +
                        $"{member.Definition.TypeOf});");
                }
            }
        }
    }
}
