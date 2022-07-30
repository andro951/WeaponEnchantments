# Weapon Enchantments

# Features
*Item Customization (Enchantments)
*Progression System (Item Experience)
*Item Upgrading (Infusion)

Terraria has you frequently swapping old gear for new. The enchanting system allows you to customize your weapons and armor, and keep your progress as you change or upgrade your gear.

# Wiki

#Config (Most options on the 2nd page!)
Many players will find Enchantments to be too powerful.  For players who enjoy a high difficulty experience, it is recomended to change the **Enchantment Strength Preset** to **Expert** (50%) or **Master** (25%). (2nd page of the config)
> You have an extreme ammount of control over the power level of this mod via the config.

## Item Experience
All weapons, armor and accesories can now gain experience (**XP**). These are **enchantable items**.

These items have a level based on their XP which is for two things:
Increases enchantment capacity of the item by 1 per level
Increases weapon critical strike chance by 1 per level (can be disabled, affected by the Config Recomended Strength multiplier mentioned above.)

You can obtain XP by damaging enemies, doing skilling activities such as mining and cutting trees, and consuming essence.

> Items with no XP will not display experience or levels.

![Tooltip](GithubSprites/MusketTooltip.png)

## Essence
![Basic Essence](/Items/Sprites/EnchantmentEssenceBasic.png)
![Common Essence](/Items/Sprites/EnchantmentEssenceCommon.png)

Essence represent solidified experience and are automatically stored in the enchanting table inventory (like a piggy bank).
They can be used to...
* Upgrade enchantments.
* Infuse it's XP value into items.

There are 5 Essence types, each worth more than the last:
0. Basic
1. Common
2. Rare
3. Super Rare
4. Ultra Rare

## Containments
![Containment](/Items/Sprites/Containment.png)
![Medium Containment](/Items/Sprites/MediumContainment.png)

These contain the power of the enchantments.  More powerful enchantments require larger and stronger containments to hold them.

There are 3 containment levels:
1. Containment (Silver or Tungsten) 
    *Ingredients - 4 Silver/Tungsten Bars, 1 Glass
2. Medium Containment (Gold or Platinum) 
    *Ingredients - 8 Gold/Platinum Bars, 4 Glass
3. Superior Containment (Demonite or Crimtane) 
    *Ingredients - 16 Demonite/Crimtane Bars, 4 common gems (any except diamond/amber)

## Enchantments
![Defense Enchantment](/Items/Sprites/StatDefenseEnchantmentBasic.png)
![Damage Enchantment](/Items/Sprites/DamageEnchantmentCommon.png)

Enchantments allow customization of your items:
    Basic stat upgrades such as:
        *Damage
        *Critical strike chance
        *Armor
        *Life steal
    More unique upgrades such as:
        *Hitting all enemies in an area (One for All)
        *Dealing massive damage but having a long recovery (All for One)
        *Flames that spread between enemies (World Ablaze)
        *Max health true damage (God Slayer)

Each enchantment has a capacity cost.  This cost is subtracted from the item enchantment capacity.

They're obtained by...
* Crafting (Only Damage and Armor)
* Looting chests.
* Defeating enemies.

## Crafting and Upgrading Enchantments

Essence in the enchanting table is available for crafting.  There is no need to take them out of the crafting table.
 > Magic Storage: The environment module lets you access the essence in the enchanting table.  The environment module will also act as the highest tier enchanting table your player has ever ussd.

Recipies can easily be looked up:
* You can give items to the vanilla guide to see what it can be crafted into.
* You can also use the recipe browser mod.

Only Basic Damage and Basic Armor enchantments can be crafted.  The other Basic tier enchantments must be found.  They can all be upgraded as described below.

There are 5 tiers:
0. Basic - 10 Basic Essence, Containment(the smallest one) (tier 0, Wood enchanting table or better)
1. Medium - 10 Medium Essence, Medium Containment, tier 0 Enchantment (tier 1, Dusty enchanting table or better)
2. Rare - 10 Rare Essence, Superior Containment, tier 1 Enchantment (tier 2, Hellish enchanting table or better)
3. Super Rare - 10 Super Rare Essence, tier 2 Enchantmentl, 2 common gems(any except diamond/amber) (tier 3, Soul enchanting table or better)
4. Ultra Rare - 10 Ultra Rare Essence, tier 3 Enchantment, 1 rare gem (diamond/amber) (tier 4, Ultimate enchanting table or better)
> Some enchantments are tier 4 exclusive, as is the case of the Spelunkner Enchantment. (This is temporary until the mechanics are in place to have lower tiers.)

## Power Boosters
These are rare items obtained by defeating bosses, and add 10 levels to your item. They can be used only once per item and are returned upon offering the item.
> The 10 levels do not count towards the lvl 40 cap and do not give critical strike chance.

## Enchanting Table
![WoodEnchantingTable](/Items/Sprites/WoodEnchantingTable.png)
![DustyEnchantingTable](/Items/Sprites/DustyEnchantingTable.png)

All of the essence you pick up are stored here. Right clicking the enchanting table opens the enchanting interface. In an enchanting table you can...
* Create enchantments.
* Upgrade enchantments.
* Apply and remove Enchants from items.
    > All you have to do is place an enchantment in an enchantment slot.  There is no cost or confirm button.
* Convert essence to item experience.
    > The Level up button will consume lower tiers of essence first if possible.  These slots can be favorited (same as vanilla favoriting) to prevent them being consumed unless there is not enough essence to level up without them.
* Offer items (Destroyes the item in exchange for essence and ores - configurable).
    > The value of essence and ores recieved is slightly more than the item's base sell price. (config default is half ore, half essence)
    > Also returns all enchantments and power boosters applied to the weapon and converts experience on the item to essence.
* Syphon excess xp from an item past the max level, 40, converting it to essence.
* Infuse items (Discussed in it's own section)

There are 5 tiers of enchanting tables
0. Wood:
    *Ingredients - workbench, 4 torches
    *60% of item experience converted to Essence when offering items.
1. Dusty:
    *Ingredients - tier 0 Enchanting Table, 10 desert fossil or 1 sturdy fossil. (Used to be Fossil Helmet until I realized that's way too expensive early game)
       > (TIP - Found in the desert, look for amber while you're there)
    *70% of item experience converted to Essence when offering items.
2. Hellish:
    *Ingredients - tier 1 Enchanting Table, obsidian skull
    *80% of item experience converted to Essence when offering items.
3. Soul:
    *Ingredients - tier 2 Enchanting Table, 2 souls of light/night
    *90% of item experience converted to Essence when offering items.
4. Ultimate:
    *Ingredients - tier 3 Enchanting Table, 2 hallowed bars
    *100% of item experience converted to Essence when offering items.

### Leveling items up
If you need to accelerate an item's growth, you can add XP to it from essence.
* Level Up button
    1. Place your item in the *Item* slot.
    2. Click on the "Level Up" button.
    3. Enough essence is used to level your item up *once*. 
        > Nothing happens if not enough.  Uses lower tier essence first.  Favorited essence is ignored if possible.
* Using a specific essence
    1. Place your item in the *Item* slot.
    2. Locate the essence you want to use.
    3. Click on the XP button below it.
    4. One of that essence is consumed to add its XP value to your item.

### Enchanting items
If you have enchantments and an item with enough level that you wish to upgrade, you can enchant it.
1. Place the item you wish to modify on the *Item* slot. 
    > Far left top row, or shift left click from your inventory.
2. Add (or remove) the enchantments you wish to add (or remove) on to the the *Enchantments* slots.
    > All other top row slots, far right is the Utility only slot.  Can also be shift clicked to move.
3. Add (or remove) the utility enchantment you wish to add (or remove) on the *Utility* slot.
    < Shift clicking a utility enchantment will put it in the utility slot first if empty.

![Tooltip](GithubSprites/EnchantingTableUI.png)

### Offering items
If you have an item that is currently useless to you, you offer it.
0. NOTE: you will lose the item in exchange of essence, and a mix of ores up to chlorophyte depending on your game progress. (configurable)
1. Place the item you want to offer on the item slot.
2. Press the offer button.
3. Carefully read the warning.
4. Confirm the offering.
5. Items spawned into your inventory.
    > Config option available to offer all of the same item from your inventory. (Ignores enchanted items)

### Syphoning an item
If you have a level 40 item (maximum level) and you want to extract leftover experience from it, you syphon it.
1. Place the item you wish to syphon on the *Item* slot.
2. Click the syphon button.
3. Essence spawned to the enchanting table. (Or your inventory if the table is full.)

### Infusion:
#### Weapon Infusion - 
Allows you to consume high rarity items to upgrade the damage of low rarity weapons.  
Example, if you like Veilthorn more than your newer/stronger weapon, just infuse the new weapon into Veilthorn to upgrade it's damage instead of switching.  
The damage bonus is based on the difference in rarity and value between the 2 items.  Terraria has 10 rarities of vanilla weapons, so I based the system off of those.  
(modded items can be rarity 11 which will cause their Infusion Power to be the same as the max value rarity 10 items (1100).  

Infusion Power - 
A weapon stat that is determined by an item's rarity and value.  100 Infusion Power per rarity (rarity x100).  
Additionally, the item's rarity will give up to 100 extra infusion power based on the value of the item compared to the average value of items in that rarity.  
(Example: items of rarity 0 have an average value of about 3000 copper (30 silver).  The lowest value item is worth 100 copper.  
This 100 copper item would have an infusion power of 0.  A rarity 0 item worth the average value (~30 silver) would have an infusion power of 50.  
The max value rarity 0 item would have 100 infusion power.  The min, max and average values are calculated based only on vanilla items.
Modded items that are above or below the min/max values will be counted as the min/max value for the infusion power calculation.  
Currently, the highest Infusion Power possible for weapons is from Meowmere (1100) because it is rarity 10 and has the highest item value of rarity 10 weapons.

How to Infuse - 
Place the higher Infusion Power item into the enchanting table (this item will be destroyed)
Click Infusion (If you change your mind, you can get the item back by pressing Cancel - Same button as Infusion)
Place the lower Infusion Power item into the enchanting table
Click Finalize (Same button as Infusion/Cancel)
> The consumed item is Offered just like with the Offer button.  The only difference is you will not get ore, but you will get all of the 
enchantments/experience as essence/power booster back.

Armor Infusion - 
Allows you to consume a piece of armor and replace the set bonus of an item with one from another.  
The piece of armor will act like the consumed one for the purposes of determining set bonuses.
The piece of armor will also look like the consumed one while equiped.

How to Infuse - 
Place the armor with the set bonus you want to transfer into the enchanting tabel (this item will be destroyed)
Click Infusion (If you change your mind, you can get the item back by pressing Cancel - Same button as Infusion)
Place the armor you want to keep into the enchanting table (It will have it's set bonus replaced with the previous item's)
Click Finalize (Same button as Infusion/Cancel)
> The consumed item is Offered just like with the Offer button.  The only difference is you will not get ore, but you will get all of the
enchantments/experience as essence/power booster back.

#### Remember that...
* You cannot add an enchantment to an item if the enchantment's capacity cost is greater than the item's remaining capacity.
* You can always remove enchantments from items, free of cost.

## When you start a game...
* Make an enchanting table right away!
    * The first enchanting table is created with a workbench and 4 torches.
* Gear yourself up (fill in your armor and accesory slots so they start getting XP).
* When upgrading, offer your old armor and weapons for essence.
* Upgrade your new weapons and armor with the obtained essence.
* Keep growing!

### Tips & Tricks
* DONT SELL enchantable items! Offer them instead.
    * The value from ore recieved is slightly higher than an item's sell value and you get Essence equivalent to the item's xp.
    * Offering items returns all Enchantments/Power Booster applied to the consumed item.
* Carrying an Enchanting Table with you to convert unwanted items is a good way to save inventory space.(Especially if you set the config to 0% ore, 100% essence)
* Make a gem tree farm (especially for diamond/amber). They are used to craft high tier Containments.

### Where do I get the good Enchantments?
![WorldAblaze](/Items/Sprites/WorldAblazeEnchantmentUltraRare.png)![Jungles Fury](/Items/Sprites/JunglesFuryEnchantmentUltraRare.png)

Unique Enchantments are obtained from Chests/Bosses/Rare enemies. Pre-Hardmode bosses drop general Enchantments usable on all items and Hardmode bosses drop more specialized Enchantments.

## Please give us your feedback!!!!!
[Weapon Enchantments Discord](https://discord.gg/mPywEhyV9b)

[Weapon Enchantments forum page](https://forums.terraria.org/index.php?threads/weapon-enchantments.112509/) (Not checked as often as Steam and Discord.)

## Contributors

### Art

andro951: 

![CatastrophicReleaseEnchantmentCommon](/Items/Sprites/CatastrophicReleaseEnchantmentCommon.png)
![MaxMinionsEnchantmentCommon](/Items/Sprites/MaxMinionsEnchantmentCommon.png)
![PhaseJumpEnchantmentCommon](/Items/Sprites/PhaseJumpEnchantmentCommon.png)
![PowerBooster](/Items/Sprites/PowerBooster.png)

Kiroto: 

![EnchantmentEssenceBasic](/Items/Sprites/EnchantmentEssenceBasic.png)
![EnchantmentEssenceBasicCB](/Items/Sprites/EnchantmentEssenceBasicCB.png)
![EnchantmentEssenceCommon](/Items/Sprites/EnchantmentEssenceCommon.png)
![EnchantmentEssenceCommonCB](/Items/Sprites/EnchantmentEssenceCommonCB.png)
![EnchantmentEssenceRare](/Items/Sprites/EnchantmentEssenceRare.png)
![EnchantmentEssenceRareCB](/Items/Sprites/EnchantmentEssenceRareCB.png)
![EnchantmentEssenceSuperRare](/Items/Sprites/EnchantmentEssenceSuperRare.png)
![EnchantmentEssenceSuperRareCB](/Items/Sprites/EnchantmentEssenceSuperRareCB.png)
![EnchantmentEssenceUltraRare](/Items/Sprites/EnchantmentEssenceUltraRare.png)
![EnchantmentEssenceUltraRareCB](/Items/Sprites/EnchantmentEssenceUltraRareCB.png)

Sir Bumpleton: 

![ShootSpeedEnchantmentCommon](/Items/Sprites/ShootSpeedEnchantmentCommon.png)

Zorutan: https://twitter.com/ZorutanMesuta

![AllForOneEnchantmentBasic](/Items/Sprites/AllForOneEnchantmentBasic.png)
![AllForOneEnchantmentCommon](/Items/Sprites/AllForOneEnchantmentCommon.png)
![AllForOneEnchantmentRare](/Items/Sprites/AllForOneEnchantmentRare.png)
![AllForOneEnchantmentSuperRare](/Items/Sprites/AllForOneEnchantmentSuperRare.png)
![AllForOneEnchantmentUltraRare](/Items/Sprites/AllForOneEnchantmentUltraRare.png)
![AmmoCostEnchantmentCommon](/Items/Sprites/AmmoCostEnchantmentCommon.png)
![ArmorPenetrationEnchantmentCommon](/Items/Sprites/ArmorPenetrationEnchantmentCommon.png)
![ColdSteelEnchantmentCommon](/Items/Sprites/ColdSteelEnchantmentCommon.png)
![Containment](/Items/Sprites/Containment.png)
![CriticalStrikeChanceEnchantmentCommon](/Items/Sprites/CriticalStrikeChanceEnchantmentCommon.png)
![DamageEnchantmentCommon](/Items/Sprites/DamageEnchantmentCommon.png)
![DustyEnchantingTable](/Items/Sprites/DustyEnchantingTable.png)
![GodSlayerEnchantmentCommon](/Items/Sprites/GodSlayerEnchantmentCommon.png)
![HellishEnchantingTable](/Items/Sprites/HellishEnchantingTable.png)
![HellsWrathEnchantmentCommon](/Items/Sprites/HellsWrathEnchantmentCommon.png)
![JunglesFuryEnchantmentCommon](/Items/Sprites/JunglesFuryEnchantmentCommon.png)
![LifeStealEnchantmentCommon](/Items/Sprites/LifeStealEnchantmentCommon.png)
![ManaEnchantmentCommon](/Items/Sprites/ManaEnchantmentCommon.png)
![MediumContainment](/Items/Sprites/MediumContainment.png)
![MoonlightEnchantmentCommon](/Items/Sprites/MoonlightEnchantmentCommon.png)
![MoveSpeedEnchantmentCommon](/Items/Sprites/MoveSpeedEnchantmentCommon.png)
![MultishotEnchantmentCommon](/Items/Sprites/MultishotEnchantmentCommon.png)
![OneForAllEnchantmentCommon](/Items/Sprites/OneForAllEnchantmentCommon.png)
![PeaceEnchantmentCommon](/Items/Sprites/PeaceEnchantmentCommon.png)
![ScaleEnchantmentCommon](/Items/Sprites/ScaleEnchantmentCommon.png)
![SoulEnchantingTable](/Items/Sprites/SoulEnchantingTable.png)
![SpeedEnchantmentCommon](/Items/Sprites/SpeedEnchantmentCommon.png)
![StatDefenseEnchantmentCommon](/Items/Sprites/StatDefenseEnchantmentCommon.png)
![SuperiorContainment](/Items/Sprites/SuperiorContainment.png)
![UltimateEnchantingTable](/Items/Sprites/UltimateEnchantingTable.png)
![WarEnchantmentCommon](/Items/Sprites/WarEnchantmentCommon.png)
![WoodEnchantingTable](/Items/Sprites/WoodEnchantingTable.png)
![WorldAblazeEnchantmentCommon](/Items/Sprites/WorldAblazeEnchantmentCommon.png)
![DustyEnchantingTable](/Tiles/Sprites/DustyEnchantingTable.png)
![HellishEnchantingTable](/Tiles/Sprites/HellishEnchantingTable.png)
![SoulEnchantingTable](/Tiles/Sprites/SoulEnchantingTable.png)
![UltimateEnchantingTable](/Tiles/Sprites/UltimateEnchantingTable.png)
![WoodEnchantingTable](/Tiles/Sprites/WoodEnchantingTable.png)