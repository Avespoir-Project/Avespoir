﻿using Avespoir.Core.Abstructs;
using Avespoir.Core.Attributes;
using Avespoir.Core.Database.Enums;
using Avespoir.Core.Database.Schemas;
using Avespoir.Core.Language;
using Avespoir.Core.Modules.Utils;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avespoir.Core.Modules.Commands.ModeratorCommands {

	[Command("db-useradd", RoleLevel.Moderator)]
	class DBUserAdd : CommandAbstruct {

		internal override LanguageDictionary Description => new LanguageDictionary("Userデータベースにユーザーを追加します") {
			{ Database.Enums.Language.en_US, "Add a user to the User database" }
		};

		internal override LanguageDictionary Usage => new LanguageDictionary("{0}db-useradd [名前] [ユーザーID] [役職登録番号]") {
			{ Database.Enums.Language.en_US, "{0}db-useradd [Name] [UserID] [Role Number]" }
		};

		internal override async Task Execute(CommandObjects CommandObject) {
			try {
				string[] msgs = CommandObject.CommandArgs.Remove(0);
				if (msgs.Length == 0) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.EmptyText);
					return;
				}

				string msgs_Name;

				if (string.IsNullOrWhiteSpace(msgs[0])) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.EmptyName);
					return;
				}
				msgs_Name = msgs[0];

				if (string.IsNullOrWhiteSpace(msgs[1])) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.EmptyId);
					return;
				}
				if (!ulong.TryParse(msgs[1], out ulong msgs_ID)) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.IdCouldntParse);
					return;
				}

				if (string.IsNullOrWhiteSpace(msgs[2])) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.EmptyRoleNumber);
					return;
				}
				if (!uint.TryParse(msgs[2], out uint msgs_RoleNum)) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.RoleNumberNotNumber);
					return;
				}

				if (Database.DatabaseMethods.AllowUsersMethods.AllowUserExist(CommandObject.Guild.Id, msgs_Name)) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.NameRegisted);
					return;
				}

				if (Database.DatabaseMethods.AllowUsersMethods.AllowUserExist(CommandObject.Guild.Id, msgs_ID)) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.IdRegisted);
					return;
				}

				if (!Database.DatabaseMethods.RolesMethods.RoleFind(CommandObject.Guild.Id, msgs_RoleNum, out Roles DBRolesNum)) {
					await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.RoleNumberNotFound);
					return;
				}

				if (!await Authentication.Confirmation(CommandObject)) {
					await CommandObject.Channel.SendMessageAsync(CommandObject.Language.AuthFailure);
					return;
				}

				AllowUsers InsertAllowUserData = Database.DatabaseMethods.AllowUsersMethods.AllowUserInsert(CommandObject.Guild.Id, msgs_ID, msgs_Name, msgs_RoleNum);

				DiscordRole GuildRole = CommandObject.Guild.GetRole(DBRolesNum.Uuid);
				string ResultText = string.Format(CommandObject.Language.DBUserAddSuccess, InsertAllowUserData.Name, InsertAllowUserData.Uuid, InsertAllowUserData.RoleNum, GuildRole.Name);
				await CommandObject.Message.Channel.SendMessageAsync(ResultText);
			}
			catch (IndexOutOfRangeException) {
				await CommandObject.Message.Channel.SendMessageAsync(CommandObject.Language.TypingMissed);
			}
		}
	}
}
