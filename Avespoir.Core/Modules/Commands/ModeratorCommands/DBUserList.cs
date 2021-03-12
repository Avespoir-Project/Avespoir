﻿using Avespoir.Core.Attributes;
using Avespoir.Core.Database.Schemas;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tababular;

namespace Avespoir.Core.Modules.Commands {

	partial class ModeratorCommands {

		[Command("db-userlist")]
		public async Task DBUserList(CommandObjects CommandObject) {
			Database.DatabaseMethods.AllowUsersMethods.AllowUsersListFind(CommandObject.Guild.Id, out List<AllowUsers> DBAllowUsersList);
			
			List<object> DBAllowUsersObjects = new List<object> { };
			foreach (AllowUsers DBAllowUser in DBAllowUsersList) {
				Database.DatabaseMethods.RolesMethods.RoleFind(CommandObject.Guild.Id, DBAllowUser.RoleNum, out Roles DBRole);

				DiscordRole GuildRole = CommandObject.Guild.GetRole(DBRole.Uuid);

				DBAllowUsersObjects.Add(new { RegisteredName = DBAllowUser.Name, UserID = DBAllowUser.Uuid, Role = GuildRole.Name });
			}

			object[] DBAllowUsersArray = DBAllowUsersObjects.ToArray();
			string DBAllowUsersTableText = new TableFormatter().FormatObjects(DBAllowUsersArray);

			await CommandObject.Message.Channel.SendMessageAsync(string.Format(CommandObject.Language.DMMention, CommandObject.Message.Author.Mention));
			if (string.IsNullOrWhiteSpace(DBAllowUsersTableText)) await CommandObject.Member.SendMessageAsync(CommandObject.Language.ListNothing);
			else await CommandObject.Member.SendMessageAsync(DBAllowUsersTableText);
		}
	}
}
