using PipeProtocolTransport;

namespace CommandsKit
{
    public class ComDeterminant : IDeterminant
    {
        private delegate Command Assigment(byte[] data);

        public Command Define(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));

            TypeCommand typeCommand;
            byte[] data = new byte[buffer.Length - 1];

            if (Enum.IsDefined(typeof(TypeCommand), buffer[0]))
            {
                typeCommand = (TypeCommand)buffer[0];
            }
            else
            {
                typeCommand = TypeCommand.UNKNOW;
            }

            Array.Copy(buffer, 1, data, 0, buffer.Length - 1);

            Assigment assigment = ChooseCommand(typeCommand);

            return assigment(data);
        }

        private Assigment ChooseCommand(TypeCommand typeCommand)
        {
            switch (typeCommand)
            {
                case TypeCommand.UNKNOW:
                    {
                        return UnknowCom.ToCommand;
                    }                
                default:
                    {
                        return UnknowCom.ToCommand;
                    }
            }
        }
    }
}
