using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SimpleChatApp.CommonTypes;
using System;
using System.Threading.Tasks;
using static SimpleChatApp.GrpcService.ChatService;

namespace task3
{
    public enum ServerStatus
    {
        SUCCESS,

        ERROR_LOGIN_BAD_LOGIN,

        ERROR_REG_LOGIN_EXISTS,
        ERROR_REG_BAD_LOGIN,

        ERROR_UNKNOWN,
    }
    public class ChatMessage
    {
        public ChatMessage(string userName, string message)
        {
            Name = userName;
            Text = message;
        }

        public string Name { get; private set; }
        public string Text { get; private set; }
    }

    public delegate void OnMessageReceivedDelegate(ChatMessage message);

    public class ChatClient
    {
        public void Initialize()
        {
            chatService = new ChatServiceClient(new Channel($"{ip}:{port}", ChannelCredentials.Insecure));
        }

        public async Task<ServerStatus> Login(string username, string password)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;

            try
            {
                SimpleChatApp.GrpcService.AuthorizationAnswer loginResult = await chatService.LogInAsync(new SimpleChatApp.GrpcService.AuthorizationData()
                {
                    ClearActiveConnection = true,
                    UserData = new SimpleChatApp.GrpcService.UserData()
                    {
                        Login = username,
                        PasswordHash = SHA256.GetStringHash(password)
                    },
                });

                switch (loginResult.Status)
                {
                    case SimpleChatApp.GrpcService.AuthorizationStatus.AuthorizationSuccessfull:
                        sessionId = loginResult.Sid.Guid_;
                        outStatus = ServerStatus.SUCCESS;
                        break;
                    case SimpleChatApp.GrpcService.AuthorizationStatus.WrongLoginOrPassword:
                        outStatus = ServerStatus.ERROR_LOGIN_BAD_LOGIN;
                        break;
                    default:
                        break;
                }
            }
            catch (RpcException)
            {
            }

            return outStatus;
        }

        public async Task<ServerStatus> Register(string username, string password)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;

            try
            {
                SimpleChatApp.GrpcService.RegistrationAnswer registerResult = await chatService.RegisterNewUserAsync(new SimpleChatApp.GrpcService.UserData()
                {
                    Login = username,
                    PasswordHash = SHA256.GetStringHash(password)
                });

                switch (registerResult.Status)
                {
                    case SimpleChatApp.GrpcService.RegistrationStatus.RegistrationSuccessfull:
                        outStatus = ServerStatus.SUCCESS;
                        break;
                    case SimpleChatApp.GrpcService.RegistrationStatus.LoginAlreadyExist:
                        outStatus = ServerStatus.ERROR_REG_LOGIN_EXISTS;
                        break;
                    case SimpleChatApp.GrpcService.RegistrationStatus.BadInput:
                        outStatus = ServerStatus.ERROR_REG_BAD_LOGIN;
                        break;
                    default:
                        break;
                }
            }
            catch (RpcException)
            {
            }

            return outStatus;
        }

        public async void Subscribe(OnMessageReceivedDelegate OnReceivedCb)
        {
            try
            {
                SimpleChatApp.GrpcService.Messages logsResult =
                    await chatService.GetLogsAsync(new SimpleChatApp.GrpcService.TimeIntervalRequest()
                    {
                        Sid = new SimpleChatApp.GrpcService.Guid() { Guid_ = sessionId },
                        StartTime = Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
                        EndTime = Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime())
                    });

                foreach (SimpleChatApp.GrpcService.MessageData message in logsResult.Logs)
                {
                    var messageData = message.Convert();
                    OnReceivedCb(new ChatMessage(messageData.UserLogin, messageData.Text));
                }

                IAsyncStreamReader<SimpleChatApp.GrpcService.Messages> stream =
                    chatService.Subscribe(new SimpleChatApp.GrpcService.Guid() { Guid_ = sessionId }).ResponseStream;

                while (await stream.MoveNext())
                {
                    if (stream.Current.ActionStatus == SimpleChatApp.GrpcService.ActionStatus.Allowed)
                    {
                        foreach (SimpleChatApp.GrpcService.MessageData message in stream.Current.Logs)
                        {
                            var messageData = message.Convert();
                            OnReceivedCb(new ChatMessage(messageData.UserLogin, messageData.Text));
                        }
                    }
                }
            }
            catch (RpcException)
            {
            }
        }

        public async void Unsubscribe()
        {
            try
            {
                await chatService.UnsubscribeAsync(new SimpleChatApp.GrpcService.Guid() { Guid_ = sessionId });
            }
            catch (RpcException)
            {
            }
        }

        public async Task<ServerStatus> SendMessage(string message)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;

            try
            {
                SimpleChatApp.GrpcService.ActionStatusMessage writeResult = await chatService.WriteAsync(new SimpleChatApp.GrpcService.OutgoingMessage()
                {
                    Sid = new SimpleChatApp.GrpcService.Guid() { Guid_ = sessionId },
                    Text = message
                });

                switch (writeResult.ActionStatus)
                {
                    case SimpleChatApp.GrpcService.ActionStatus.Allowed:
                        outStatus = ServerStatus.SUCCESS;
                        break;
                    default:
                        break;
                }
            }
            catch (RpcException)
            {
            }

            return outStatus;
        }

        public static ChatClient Instance { get { return instance; } }

        private ChatServiceClient chatService;
        private string sessionId = "";

        private static readonly string ip = "109.110.63.211";
        private static readonly string port = "30051";
        private static readonly ChatClient instance = new ChatClient();
    }
}
