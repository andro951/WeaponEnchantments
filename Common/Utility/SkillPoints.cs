using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Common.Utility
{
	public class SkillPoints {
		public static readonly int[] MilestonePoints = { 5, 10, 25 };
		//Need to convert all the strings in here to enum keys and add localizations and add to LocalizationData
		private static Dictionary<EItemType, List<string>> AllSkillNames = new() {
			{ EItemType.Weapons, new() { "Strength", "Swiftness", "Precision" } },
			{ EItemType.Armor, new() { "Resilience", "Endurance", "Deftness" } },
			{ EItemType.Accessories, new() { "Speed", "Greed", "Luck" } },
			{ EItemType.Tools, new() { "Haste", "Reach", "Violence" } },
			{ EItemType.FishingPoles, new() { "Alluring", "Efficiency", "Fortune" } }
		};
		private int[] _skillPoints = new int[3];
		public int FirstSkill => _skillPoints[0];
		public int SecondSkill => _skillPoints[1];
		public int ThirdSkill => _skillPoints[2];
		public int TotalSkillPoints => AllSkills.Sum();
		public int[] AllSkills => _skillPoints;
		private EItemType _itemType;
		private List<(string, List<EnchantmentEffect>)> _effects = new();
		private List<List<EnchantmentEffect>> _milestoneEffects = new();

		public List<string> SkillNames => AllSkillNames[_itemType];
		public SkillPoints(EnchantedItem enchantedItem) {
			Item item = enchantedItem.Item;
			if (item.pick > 0 || item.axe > 0 || item.hammer > 0) {
				_itemType = EItemType.Tools;
			}
			else {
				_itemType = enchantedItem.ItemType;
			}

			float strength = 0.4f * ConfigValues.GlobalStrengthMultiplier;
			switch (_itemType) {
				case EItemType.Weapons:
					_effects = new() {
						("",
						new List<EnchantmentEffect> { new DamageAfterDefenses(new DifficultyStrength(strength)), new Size(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new AttackSpeed(new DifficultyStrength(strength)), new ProjectileVelocity(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new CriticalStrikeChance(new DifficultyStrength(strength)), new AmmoCost(new DifficultyStrength(strength)), new ManaUsage(new DifficultyStrength(strength)) })
					};
					_milestoneEffects = new() {
						new() {
							new DamageAfterDefenses(new DifficultyStrength(strength / 5f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 2f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 1f))
						},
						new() {
							new AttackSpeed(new DifficultyStrength(strength / 5f)),
							new AttackSpeed(new DifficultyStrength(strength / 2f)),
							new AttackSpeed(new DifficultyStrength(strength / 1f))
						},
						new() {
							new CriticalStrikeChance(new DifficultyStrength(strength / 5f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 2f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 1f))
						}
					};
					break;

				case EItemType.Armor:
					_effects = new() {
						("",
						new List<EnchantmentEffect> { new DamageAfterDefenses(new DifficultyStrength(strength)), new Size(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new AttackSpeed(new DifficultyStrength(strength)), new ProjectileVelocity(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new CriticalStrikeChance(new DifficultyStrength(strength)), new AmmoCost(new DifficultyStrength(strength)), new ManaUsage(new DifficultyStrength(strength)) })
					};
					_milestoneEffects = new() {
						new() {
							new DamageAfterDefenses(new DifficultyStrength(strength / 5f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 2f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 1f))
						},
						new() {
							new AttackSpeed(new DifficultyStrength(strength / 5f)),
							new AttackSpeed(new DifficultyStrength(strength / 2f)),
							new AttackSpeed(new DifficultyStrength(strength / 1f))
						},
						new() {
							new CriticalStrikeChance(new DifficultyStrength(strength / 5f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 2f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 1f))
						}
					};
					break;

				case EItemType.Accessories:
					_effects = new() {
						("",
						new List<EnchantmentEffect> { new DamageAfterDefenses(new DifficultyStrength(strength)), new Size(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new AttackSpeed(new DifficultyStrength(strength)), new ProjectileVelocity(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new CriticalStrikeChance(new DifficultyStrength(strength)), new AmmoCost(new DifficultyStrength(strength)), new ManaUsage(new DifficultyStrength(strength)) })
					};
					_milestoneEffects = new() {
						new() {
							new DamageAfterDefenses(new DifficultyStrength(strength / 5f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 2f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 1f))
						},
						new() {
							new AttackSpeed(new DifficultyStrength(strength / 5f)),
							new AttackSpeed(new DifficultyStrength(strength / 2f)),
							new AttackSpeed(new DifficultyStrength(strength / 1f))
						},
						new() {
							new CriticalStrikeChance(new DifficultyStrength(strength / 5f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 2f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 1f))
						}
					};
					break;

				case EItemType.Tools:
					_effects = new() {
						("",
						new List<EnchantmentEffect> { new DamageAfterDefenses(new DifficultyStrength(strength)), new Size(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new AttackSpeed(new DifficultyStrength(strength)), new ProjectileVelocity(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new CriticalStrikeChance(new DifficultyStrength(strength)), new AmmoCost(new DifficultyStrength(strength)), new ManaUsage(new DifficultyStrength(strength)) })
					};
					_milestoneEffects = new() {
						new() {
							new DamageAfterDefenses(new DifficultyStrength(strength / 5f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 2f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 1f))
						},
						new() {
							new AttackSpeed(new DifficultyStrength(strength / 5f)),
							new AttackSpeed(new DifficultyStrength(strength / 2f)),
							new AttackSpeed(new DifficultyStrength(strength / 1f))
						},
						new() {
							new CriticalStrikeChance(new DifficultyStrength(strength / 5f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 2f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 1f))
						}
					};
					break;

				case EItemType.FishingPoles:
					_effects = new() {
						("",
						new List<EnchantmentEffect> { new DamageAfterDefenses(new DifficultyStrength(strength)), new Size(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new AttackSpeed(new DifficultyStrength(strength)), new ProjectileVelocity(new DifficultyStrength(strength)) }),
						("",
						new List<EnchantmentEffect> { new CriticalStrikeChance(new DifficultyStrength(strength)), new AmmoCost(new DifficultyStrength(strength)), new ManaUsage(new DifficultyStrength(strength)) })
					};
					_milestoneEffects = new() {
						new() {
							new DamageAfterDefenses(new DifficultyStrength(strength / 5f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 2f)),
							new DamageAfterDefenses(new DifficultyStrength(strength / 1f))
						},
						new() {
							new AttackSpeed(new DifficultyStrength(strength / 5f)),
							new AttackSpeed(new DifficultyStrength(strength / 2f)),
							new AttackSpeed(new DifficultyStrength(strength / 1f))
						},
						new() {
							new CriticalStrikeChance(new DifficultyStrength(strength / 5f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 2f)),
							new CriticalStrikeChance(new DifficultyStrength(strength / 1f))
						}
					};
					break;
			}
		}
		public void SetSkillPoints(int first, int second, int third) {
			_skillPoints = new int[] { first, second, third };
		}
		public void SetSkillPoints(int[] skillPoints) {
			int length = skillPoints.Length;
			for (int i = 0; i < _skillPoints.Length; i++) {
				_skillPoints[i] = i < length ? skillPoints[i] : 0;
			}
		}
		public void Reset() {
			for (int i = 0; i < _skillPoints.Length; i++) {
				_skillPoints[i] = 0;
			}

			if (Main.LocalPlayer.TryGetWEPlayer(out WEPlayer wePlayer))
				wePlayer.enchantingTableUI?.UpdateSkills();
		}

		public int AvailableSkillPoints(int totalPoints) =>
			(int)Math.Ceiling((double)(totalPoints - TotalSkillPoints));

		public bool TryUseSkillPoint(int pos, int availablePoints) {
			if (availablePoints <= 0)
				return false;

			if (pos < 0 || pos > _skillPoints.Length)
				return false;

			AllSkills[pos]++;
			return true;
		}

		public string SkillName(int skillNum) => 
			skillNum >= 0 && AllSkillNames[_itemType].Count > skillNum ? AllSkillNames[_itemType][skillNum] : "";

		public string MileStoneTooltip(int skillNum, int milestoneNum) => _milestoneEffects[skillNum][milestoneNum].Tooltip;
		public void UpdateMilestone(ref UIText uIText, int skillNum, int mileStoneNum) {
			int skillPoints = _skillPoints[skillNum];
			int milestonePoints = MilestonePoints[mileStoneNum];
			bool milestone = skillPoints >= milestonePoints;
			uIText.SetText(MileStoneTooltip(skillNum, mileStoneNum));
			uIText.TextColor = milestone ? Color.Yellow : Color.Gray;
		}
		public string PerLevelEffectTooltip(int skillNum) {
			string amalgamation = "";
			if (_effects[skillNum].Item1 != "") return _effects[skillNum].Item1;
			foreach (EnchantmentEffect skillstats in _effects[skillNum].Item2)
            {
				amalgamation = amalgamation + " # " + skillstats.PerLevelTooltip;
			}
			return amalgamation;
		}

		public void GetEffects(List<EnchantmentEffect> effects) {
			if (_effects.Count < 0)
			{
				for (int i = 0; i < _effects.Count; i++)
				{
					int skillPoints = _skillPoints[i];
					if (skillPoints > 0)
					{
                        foreach (EnchantmentEffect skillstats in _effects[i].Item2)
						{
							effects.Add(skillstats);
						}
						foreach (int milestone in MilestonePoints)
						{
							if (skillPoints > milestone)
								effects.Add(_milestoneEffects[i][milestone]);
						}
					}
				}
			}
		}
	}
}
