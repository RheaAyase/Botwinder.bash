using System;
using System.Threading.Tasks;
using Botwinder.core;
using Botwinder.entities;

using guid = System.UInt64;

namespace Botwinder.discord
{
	class Program
	{
		static void Main(string[] args)
		{
			int shardIdOverride = -1;
			if( args != null && args.Length > 0 && !int.TryParse(args[0], out shardIdOverride) )
			{
				Console.WriteLine("Invalid parameter.");
				return;
			}

			(new Client()).RunAndWait(shardIdOverride).GetAwaiter().GetResult();
		}
	}

	class Client
	{
		private BotwinderClient Bot;

		public Client()
		{}

		public async Task RunAndWait(int shardIdOverride = - 1)
		{
			while( true )
			{
				this.Bot = new BotwinderClient(shardIdOverride);
				Init();

				try
				{
					await this.Bot.Connect();
					this.Bot.Events.Initialize += InitCommands;
					await Task.Delay(-1);
				}
				catch(Exception e)
				{
					await this.Bot.LogException(e, "--BotwinderClient crashed.");
					this.Bot.Dispose();
				}
			}
		}

		private void Init()
		{}

		private Task InitCommands()
		{
// !do
			Command newCommand = new Command("do");
			newCommand.Type = CommandType.Standard;
			newCommand.Description = "Run a bash script given by the params.";
			newCommand.RequiredPermissions = PermissionType.OwnerOnly;
			newCommand.OnExecute += async e => {
				string response = "";
				try
				{
					response = Bash.Run(e.TrimmedMessage.Trim('`'));
				}
				catch(Exception ex)
				{
					response = ex.Message;
				}
				await this.Bot.SendMessageToChannel(e.Channel, response);
			};
			this.Bot.Commands.Add(newCommand.Id, newCommand);

			return Task.CompletedTask;
		}
	}
}
