using CommunityToolkit.Mvvm.Messaging.Messages;
using ControYaApp.Models;

namespace ControYaApp.Services.Messages
{
    public class UsuarioLoginMessage : ValueChangedMessage<Usuario>
    {
        public UsuarioLoginMessage(Usuario usuario) : base(usuario)
        {
        }
    }
}
