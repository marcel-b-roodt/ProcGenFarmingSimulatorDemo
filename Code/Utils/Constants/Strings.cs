public static class Strings
{
	public static class UI
	{
		public static string CraftingCategory(CraftingCategory craftingCategory) { return $"crafting_{craftingCategory.ToString()}"; }
		public static string CraftingRecipeDescription(ItemMetadataID outputID, CraftingCategory category) { return $"{CraftingCategory(category)}_{outputID.ToString()}"; }
		public static string GenericDialogueTitle(DialoguePurpose id) { return $"dialogue_{id.ToString()}_title"; }
		public static string GenericDialogueText(DialoguePurpose id) { return $"dialogue_{id.ToString()}_text"; }
		public static string ItemName(ItemMetadataID id) { return $"item_{id.ToString()}_name"; }
		public static string ItemDescription(ItemMetadataID id) { return $"item_{id.ToString()}_description"; }
		public static string SkillName(PlayerSkillID id) { return $"skill_{id.ToString()}_name"; }
		public static string SkillDescription(PlayerSkillID id) { return $"skill_{id.ToString()}_description"; }

		public static class Static
		{
			public static string GenericInputDialogueTitle = $"dialogue_generic_input_title";
			public static string GenericInputDialogueText = $"dialogue_generic_input_text";
		}
	}

	public static class Dialogue
	{
		public static string[] HelpDialogueText = StringUtils.Array("Woah there. It looks like you've stopped by kinda early.",
																"I'm here to help you. I'm the handy Help Sign.",
																"So you wanna learn how to farm and mine? Unfortunately...",
																"There's nothing much here just yet. You're gonna have to wait a bit more, I'm afraid.",
																"But thank you so much for stopping by! Be sure to check back later.");
	}
}
