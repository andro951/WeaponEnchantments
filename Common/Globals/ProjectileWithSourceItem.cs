using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib;

namespace WeaponEnchantments.Common.Globals
{
	public abstract class ProjectileWithSourceItem : GlobalProjectile
    {
        //Sources
        public Item sourceItem;
        protected bool itemSourceSet;
        protected bool ItemSourceSet {
            get => itemSourceSet;
            set {
                if (sourceItem != null)
                    itemSourceSet = value;
            }
        }

        public Player playerSource;
        public Projectile parent = null;

        //Stat changes
        bool firstScaleCheck = true;
        bool reApplyScale = false;
        float initialScale = 1f;
        float referenceScale = 1f;
        float lastScaleBonus = 1f;

        //Attack speed tracking
        protected float speed;

        //Tracking
        protected bool updated = false;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
            return entity.ValidOwner(out _);
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source) {
            //All other sources
            if (source is EntitySource_ItemUse uSource) {
                if (uSource.Item != null && uSource.Item.TryGetEnchantedItemSearchAll(out EnchantedItem uSourceGlobal)) {
                    sourceItem = uSource.Item;
                    ItemSourceSet = true;
                }
            }
            else if (source is EntitySource_ItemUse_WithAmmo wSource) {
                if (wSource.Item.TryGetEnchantedItem()) {
                    sourceItem = wSource.Item;
                    ItemSourceSet = true;
                }
            }
            else if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile && projectile.type != ProjectileID.FallingStar) {
                parent = parentProjectile;
                TryUpdateFromParent();
            }
            else if (source is EntitySource_Misc eSource && eSource.Context != "FallingStar") {
                if (AndroMod.calamityEnabled && projectile.TryGetWEPlayer(out WEPlayer wePlayer) && wePlayer.CalamityRespawnMinionSourceItems.ContainsKey(projectile.type)) {
                    sourceItem = wePlayer.CalamityRespawnMinionSourceItems[projectile.type];
                    wePlayer.CalamityRespawnMinionSourceItems.Remove(projectile.type);
                }
				else {
                    sourceItem = FindMiscSourceItem(projectile, eSource.Context);
                }
                
                ItemSourceSet = sourceItem.TryGetEnchantedItem();
            }
            else if (source is EntitySource_Parent projectilePlayerSource && projectilePlayerSource.Entity is Player pSource) {
                //Projectiles such as stardust guardian.
                playerSource = pSource;
            }

            if (!ItemSourceSet)
                return;

            ActivateMultishot(projectile, source);
        }

        protected virtual void TryUpdateFromParent() {
            //Player source
            playerSource = parent.GetMyGlobalProjectile().playerSource;

            if (!parent.TryGetProjectileWithSourceItem(out ProjectileWithSourceItem pGlobal))
                return;

            //Source Item
            sourceItem = pGlobal.sourceItem;
            ItemSourceSet = true;

            if (pGlobal is WEProjectile weProjectileParent && this is WEProjectile weProjectile)
				weProjectile.TryUpdateFromWEProjectileParent(weProjectileParent);
        }

        /// <summary>
        /// Usually used for projectiles spawned by ai behavior of itself creating a new projectile. (Example Desert Tiger Staff)
        /// </summary>
        public static Item FindMiscSourceItem(Projectile projectile, string context = "") {
            int matchs = 0;
            int bestMatch = -1;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            List<string> projectileNames = context == "" ? projectile.Name.RemoveProjectileName().SplitString() : context.SplitString();
            for (int i = 0; i < Main.projectile.Length; i++) {
                //Find main projectile i
                Projectile mainProjectile = Main.projectile[i];
                if (mainProjectile.type == ProjectileID.None)
                    continue;

                //Find the best other projectile that matches.
                if (mainProjectile.owner == wePlayer.Player.whoAmI && mainProjectile.type != projectile.type) {
                    if (mainProjectile.TryGetProjectileWithSourceItem(out ProjectileWithSourceItem projectileWSI) && projectileWSI.sourceItem.TryGetEnchantedItem()) {
                        List<string> mainProjectileNames = mainProjectile.Name.RemoveProjectileName().SplitString();
                        int checkMatches = projectileNames.CheckMatches(mainProjectileNames);
                        if (checkMatches > matchs) {
                            matchs = checkMatches;
                            bestMatch = i;
                        }
                    }
                }
            }

            if (bestMatch >= 0 && Main.projectile[bestMatch].TryGetProjectileWithSourceItem(out ProjectileWithSourceItem projectileWithSourceItem))
                return projectileWithSourceItem.sourceItem;

            return null;
        }
        public virtual bool UpdateProjectile(Projectile projectile) {
            if (!ItemSourceSet) {
                if (parent != null)
                    TryUpdateFromParent();

                return false;
            }

            if (!sourceItem.TryGetEnchantedItem())
                return false;

            WEPlayer wePlayer = Main.player[projectile.owner].GetWEPlayer();
            if (wePlayer.BoolEffects.ContainsKey(EnchantmentStat.InfinitePenetration)) {
                if (wePlayer.BoolEffects[EnchantmentStat.InfinitePenetration])
                    projectile.penetrate = -1;
            }
            //Player player = Main.player[projectile.owner];
            //Infinite Penetration
            //if (player.ContainsEStat("InfinitePenetration", sourceItem)) {
            //    projectile.penetrate = -1;
            //}

            //Initial scale
            initialScale = projectile.scale;
			//Fix for Titanium Decimator from Calamity's Titanium Railgun
			if (initialScale <= 0)
                initialScale = 1f;

			GetSharedVanillaModifierStrength(projectile.owner, EnchantmentStat.Size, out float sizeMultiplier);
            bool projectileScaleNotModified = projectile.scale < sizeMultiplier * ContentSamples.ProjectilesByType[projectile.type].scale;
            if (sizeMultiplier >= 1f && projectileScaleNotModified) {
                projectile.scale *= sizeMultiplier;
                lastScaleBonus = sizeMultiplier;
            }

            //Reference scale (after applying sourceItem.scale if needed)
            referenceScale = projectile.scale;
			//Fix for Titanium Decimator from Calamity's Titanium Railgun
			if (referenceScale <= 0)
                referenceScale = 1f;

            return true;
        }
        protected virtual void ActivateMultishot(Projectile projectile, IEntitySource source) {
            if (!projectile.TryGetWEPlayer(out WEPlayer wePlayer))
                return;

            if (!wePlayer.CheckEnchantmentStatsCheckSourceIsHeldItem(EnchantmentStat.Multishot, source, out float multishotChance))
                return;

            //Multishot
            bool notAMultishotProjectile = source is not EntitySource_Parent parentSource || parentSource.Entity is not Projectile parentProjectile || parentProjectile.type != projectile.type;
            if (notAMultishotProjectile) {
                int projectiles = (int)multishotChance;
                float randFloat = Main.rand.NextFloat();
                projectiles += randFloat <= multishotChance - projectiles ? 1 : 0;
                if (projectiles > 0) {
                    float spread = (float)Math.PI / 200f;
                    bool invert = false;
                    int rotationCount = 0;
                    for (int i = 1; i <= projectiles; i++) {
                        if (!invert)
                            rotationCount++;

                        float rotation = (float)rotationCount - ((float)projectiles - 2f) / 2f;
                        if (invert)
                            rotation *= -1f;

                        //Vector2 position = projectile.position.RotatedBy(spread * rotation);
                        Vector2 position = projectile.position;
                        Vector2 velocity = projectile.velocity.RotatedBy(spread * rotation);
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), position, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                        invert = !invert;
                    }
                }
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor) {
            if (!updated)
                UpdateProjectile(projectile);

            if (!sourceItem.TryGetEnchantedItem())
                return true;

            //If source item scale changed, update the projectile scale.
            GetSharedVanillaModifierStrength(projectile.owner, EnchantmentStat.Size, out float sizeMultiplier);
            if (sizeMultiplier == lastScaleBonus)
                return true;

            //Update the prjoectile scale.
            projectile.scale /= lastScaleBonus;
            referenceScale /= lastScaleBonus;
            projectile.scale *= sizeMultiplier;
            referenceScale *= sizeMultiplier;
            lastScaleBonus = sizeMultiplier;

            return true;
        }
		public override bool ShouldUpdatePosition(Projectile projectile) {
            if (!ItemSourceSet)
                return false;

            if (speed == 0f)
                return false;

            return true;
        }

        protected bool GetSharedVanillaModifierStrength(int owner, EnchantmentStat enchantmentStat, out float strength, float baseStrength = 1f) {
            strength = baseStrength;

            WEPlayer wePlayer = Main.player[owner].GetWEPlayer();
            if (wePlayer.VanillaStats.ContainsKey(enchantmentStat))
                wePlayer.VanillaStats[enchantmentStat].ApplyTo(ref strength);

            if (sourceItem.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem)) {
                if (enchantedHeldItem.VanillaStats.ContainsKey(enchantmentStat))
                    enchantedHeldItem.VanillaStats[enchantmentStat].ApplyTo(ref strength);
            }

            return strength != 1f;
        }
        protected bool GetEnchantmentModifierStrength(EnchantmentStat enchantmentStat, out float strength, float baseStrength = 1f) {
            strength = baseStrength;

            if (!sourceItem.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem))
                return false;

            if (enchantedHeldItem.EnchantmentStats.ContainsKey(enchantmentStat))
                enchantedHeldItem.EnchantmentStats[enchantmentStat].ApplyTo(ref strength);

            return strength != 1f;
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox) {
            if (!ItemSourceSet)
                return;

            if (firstScaleCheck) {
                firstScaleCheck = false;
                switch (projectile.type) {
                    case ProjectileID.LastPrismLaser:
                    case ProjectileID.StardustDragon1:
                    case ProjectileID.StardustDragon2:
                    case ProjectileID.StardustDragon3:
                    case ProjectileID.StardustDragon4:
                        //Re-apply scale each draw tick
                        reApplyScale = true;
                        break;
                }

                //Update initialScale if source item scale changed.
                if (Math.Abs(projectile.scale - initialScale) < Math.Abs(projectile.scale - referenceScale))
                    initialScale = projectile.scale;
            }

            GetSharedVanillaModifierStrength(projectile.owner, EnchantmentStat.Size, out float sizeMultiplier);

            //Adjust or re-adjust the projectile to the item scale
            if (reApplyScale || sizeMultiplier > 1f && projectile.scale == initialScale) {
                if (projectile.scale / lastScaleBonus >= 1f) {
                    projectile.scale /= lastScaleBonus;
                }

                projectile.scale *= sizeMultiplier;
            }

            //Excluded Projectiles from hitbox change
            switch (projectile.type) {
                case ProjectileID.LastPrismLaser:
                    return;
            }

            //Modify hitbox
            hitbox.Height = (int)Math.Round(hitbox.Height * referenceScale / initialScale);
            hitbox.Width = (int)Math.Round(hitbox.Width * referenceScale / initialScale);
            float scaleShift = (projectile.scale - 1f) / (2f * projectile.scale);
            hitbox.Y -= (int)(scaleShift * hitbox.Height);
            hitbox.X -= (int)(scaleShift * hitbox.Width);
        }
    }
}
