namespace Yahvol.Services.Commands.Test
{
	using System.Collections.Generic;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Newtonsoft.Json;

	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void DeserializeCommand()
		{
			var command = new UserCommand(UserCommand.Actions.Upsert, "Gsp", nameof(UnitTest1));

			var deserializedCommand = JsonConvert.DeserializeObject<UserCommand>(JsonConvert.SerializeObject(command));

			Assert.IsInstanceOfType(deserializedCommand, typeof(UserCommand));
		}
	}
}
