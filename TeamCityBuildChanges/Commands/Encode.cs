﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyConsole;
using NDesk.Options;
using TeamCityBuildChanges.ExternalApi;
using TeamCityBuildChanges.ExternalApi.Jira;

namespace TeamCityBuildChanges.Commands
{
    public class Encode : ConsoleCommand
    {
        private string _username;
        private string _password;

        public Encode()
        {
            HasRequiredOption("u|username=", "Username to use for encoding.", s => _username = s);
            HasRequiredOption("p|password=", "Password to use for encoding.", s => _password = s);

        }
        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("Encoded string for use as username/password:  {0}", JiraApi.GetEncodedCredentials(_username, _password));
            return 0;
        }
    }
}
