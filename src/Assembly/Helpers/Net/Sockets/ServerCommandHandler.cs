﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assembly.Metro.Controls.PageTemplates.Games;

namespace Assembly.Helpers.Net.Sockets
{
	public class ServerCommandHandler : IPokeCommandHandler
	{
		private NetworkPokeServer _server;
		private List<string> _clientList;
		private string _name = "Server";
	    private HaloMap _map;

	    public ServerCommandHandler(HaloMap map)
		{
		    _map = map;
			_server = new NetworkPokeServer();
			var thread = new Thread(new ThreadStart(delegate
			{
				while (true)
				{
				    _server.ReceiveCommand(this);
				}
			}));
			thread.Start();
		}

		public void HandleFreezeCommand(FreezeCommand freeze)
		{
			HandlerFunctions.FreezeCommand(freeze);
			_server.SendCommandToAll(freeze);
		}

		public void HandleMemoryCommand(MemoryCommand memory)
		{
			HandleFreezeCommand(new FreezeCommand(true));
            HandlerFunctions.MemoryCommand(memory, _map);
			_server.SendCommandToAll(memory);
            HandleFreezeCommand(new FreezeCommand(false));
		}

		public void StartFreezeCommand(FreezeCommand freeze)
		{
			HandleFreezeCommand(freeze);
		}

		public void StartMemoryCommand(MemoryCommand memory)
		{
			HandleMemoryCommand(memory);
		}

		public void StartNameChangeCommand(ChangeNameCommand changeNameCommand)
		{
			_name = changeNameCommand.Name;
		}

		public List<ClientModel> GetClientList()
		{
			return _server.GetClients();
		} 

		public List<string> GetClientIpList()
		{
			_clientList = new List<string>();
			_clientList.Add(_name);
			foreach (var client in GetClientList())
			{
				_clientList.Add(client.Name);
			}
			return _clientList;
		}

		public void HandleChangeNameCommand(ChangeNameCommand changeNameCommand)
		{
			changeNameCommand.Source.Name = changeNameCommand.Name;
		}

		public void HandleClientListCommand(ClientListCommand clientListCommand)
		{
			
		}
	}
}