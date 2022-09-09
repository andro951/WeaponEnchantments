namespace WeaponEnchantments.Items
{
	public interface ISoldByWitch
	{
		public virtual SellCondition SellCondition => SellCondition.AnyTime;
		public virtual float SellPriceModifier => Witch.GetSellPriceModifier(SellCondition);
	}
}
