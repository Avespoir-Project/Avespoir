﻿using DSharpPlus;

namespace AvespoirTest.Core.Configs {

	class ClientConfig {

		internal static string Token { get; set; }
		
		internal static DiscordConfiguration DiscordConfig() => new DiscordConfiguration {
			Token = Token,
			#if DEBUG
			UseInternalLogHandler = true,
			LogLevel = LogLevel.Debug
			#endif
		};

	}
}