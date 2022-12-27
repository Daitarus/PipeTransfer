using PipeProtocolTransport;

namespace CommandsKit
{
    public class ComDeterminant : IDeterminant
    {
        private delegate Command Assigment(byte[] data);

        public Command Define(byte[] buffer)
        {
            TypeCommand typeCommand = TypeCommand.UNKNOW;
            byte[] data = new byte[0];

            if (buffer != null && buffer.Length > 1)
            {
                data = new byte[buffer.Length - 1];

                if (Enum.IsDefined(typeof(TypeCommand), buffer[0]))
                {
                    typeCommand = (TypeCommand)buffer[0];
                }

                Array.Copy(buffer, 1, data, 0, buffer.Length - 1);
            }

            Assigment assigment = ChooseCommand(typeCommand);

            return assigment(data);
        }

        private Assigment ChooseCommand(TypeCommand typeCommand)
        {
            switch (typeCommand)
            {
                case TypeCommand.FILE_REQUEST:
                    {
                        return FileRequest.ToCommand;
                    }                
                default:
                    {
                        return UnknowCom.ToCommand;
                    }
            }
        }
    }
}
