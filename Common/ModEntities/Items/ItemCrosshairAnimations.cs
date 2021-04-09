﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.Systems.Crosshairs;
using TerrariaOverhaul.Common.Systems.Time;

namespace TerrariaOverhaul.Common.ModEntities.Items
{
	public sealed class ItemCrosshairAnimations : GlobalItem
	{
		public override bool? UseItem(Item item, Player player)
		{
			int useTime = PlayerHooks.TotalUseTime(item.useTime, player, item);
			float useTimeInSeconds = useTime * TimeSystem.LogicDeltaTime;

			CrosshairSystem.AddImpulse(7f, useTimeInSeconds);
			CrosshairSystem.AddImpulse(0f, useTimeInSeconds * 0.5f, color: Color.White);

			return base.UseItem(item, player);
		}
		public override void UseAnimation(Item item, Player player)
		{
			const int MinTime = 25;

			if(item.useAnimation > MinTime) {
				int useAnimation = PlayerHooks.TotalMeleeTime(item.useAnimation, player, item);

				if(useAnimation > MinTime) {
					CrosshairSystem.AddImpulse(10f, useAnimation * TimeSystem.LogicDeltaTime, autoRotation: true);
				}
			}
		}
	}
}