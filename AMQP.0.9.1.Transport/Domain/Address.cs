using System;

namespace AMQP_0_9_1.Transport.Domain
{
    public sealed class Address
    {
        public const string Amqp = "AMQP";
        public const string Amqps = "AMQPS";
        const int AmqpPort = 5672;
        const int AmqpsPort = 5671;

        /// <summary>
        /// Initializes a new instance of the Address class from a string.
        /// </summary>
        /// <param name="address">The string representation of the _address. User and password in
        /// the string, if any, MUST be URL encoded.</param>
        public Address(string address)
        {
            Port = -1;
            Parse(address);
            SetDefault();
        }

        /// <summary>
        /// Initializes a new instance of the Address class from individual components.
        /// </summary>
        /// <param name="host">The domain of the _address.</param>
        /// <param name="port">The _port number of the _address.</param>
        /// <param name="user">User name for SASL PLAIN profile without URL encoding.</param>
        /// <param name="password">Password for SASL PLAIN profile without URL encoding.</param>
        /// <param name="path">The path of the _address.</param>
        /// <param name="scheme">Protocol scheme, which can be either "amqp" or "amqps".</param>
        public Address(string host, int port, string user = null, string password = null, string path = "/", string scheme = Amqps)
        {
            Host = host;
            Port = port;
            Path = path;
            Scheme = scheme;
            User = user;
            Password = password;
            SetDefault();
        }

        /// <summary>
        /// Gets the protocol scheme.
        /// </summary>
        public string Scheme
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating where TLS is enabled.
        /// </summary>
        public bool UseSsl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the _host of the _address.
        /// </summary>
        public string Host
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the _port number of the _address.
        /// </summary>
        public int Port
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the user name that is used for SASL PLAIN profile.
        /// </summary>
        public string User
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the password that is used for SASL PLAIN profile.
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the path of the _address.
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        enum ParseState
        {
            Scheme,
            Slash1,
            Slash2,
            User,
            Password,
            Host,
            Port,
            Path
        }

        void Parse(string address)
        {
            //  amqp[s]://user:password@a.site.com:_port/foo/bar
            ParseState state = ParseState.Scheme;
            int startIndex = 0;
            for (int i = 0; i < address.Length; ++i)
            {
                switch (address[i])
                {
                    case ':':
                        if (state == ParseState.Scheme)
                        {
                            Scheme = address.Substring(startIndex, i - startIndex);
                            state = ParseState.Slash1;
                        }
                        else if (state == ParseState.User)
                        {
                            User = address.Substring(startIndex, i - startIndex);
                            state = ParseState.Password;
                            startIndex = i + 1;
                        }
                        else if (state == ParseState.Host)
                        {
                            Host = address.Substring(startIndex, i - startIndex);
                            state = ParseState.Port;
                            startIndex = i + 1;
                        }
                        else
                        {
                            throw new Exception("InvalidField");
                        }
                        break;
                    case '/':
                        if (state == ParseState.Slash1)
                        {
                            state = ParseState.Slash2;
                        }
                        else if (state == ParseState.Slash2)
                        {
                            state = ParseState.User;
                            startIndex = i + 1;
                        }
                        else if (state == ParseState.User || state == ParseState.Host)
                        {
                            Host = address.Substring(startIndex, i - startIndex);
                            state = ParseState.Path;
                            startIndex = i;
                        }
                        else if (state == ParseState.Port)
                        {
                            Port = int.Parse(address.Substring(startIndex, i - startIndex));
                            state = ParseState.Path;
                            startIndex = i;
                        }
                        else if (state == ParseState.Password)
                        {
                            Host = User;
                            User = null;
                            Port = int.Parse(address.Substring(startIndex, i - startIndex));
                            state = ParseState.Path;
                            startIndex = i;
                        }
                        break;
                    case '@':
                        if (state == ParseState.Password)
                        {
                            Password = address.Substring(startIndex, i - startIndex);
                            state = ParseState.Host;
                            startIndex = i + 1;
                        }
                        else
                        {
                            throw new Exception("InvalidField");
                        }
                        break;
                    default:
                        break;
                }

                if (state == ParseState.Path)
                {
                    Path = address.Substring(startIndex);
                    break;
                }
            }

            // check state in case of no trailing slash
            if (state == ParseState.User || state == ParseState.Host)
            {
                Host = address.Substring(startIndex);
            }
            else if (state == ParseState.Password)
            {
                Host = User;
                User = null;
                if (startIndex < address.Length - 1)
                {
                    Port = int.Parse(address.Substring(startIndex));
                }
            }
            else if (state == ParseState.Port)
            {
                Port = int.Parse(address.Substring(startIndex));
            }

            if (Password != null && Password.Length > 0)
            {
                Password = Uri.UnescapeDataString(Password);
            }

            if (User != null && User.Length > 0)
            {
                User = Uri.UnescapeDataString(User);
            }

            if (Host != null)
            {
                Host = Uri.UnescapeDataString(Host);
            }
        }

        void SetDefault()
        {
            string schemeUpper = Scheme.ToUpper();
            if (schemeUpper == Amqps)
            {
                UseSsl = true;
            }

            if (Port == -1)
            {
                if (UseSsl)
                {
                    Port = AmqpsPort;
                }
                else if (schemeUpper == Amqp)
                {
                    Port = AmqpPort;
                }
            }

            if (Path == null)
            {
                Path = "/";
            }
        }
    }
}