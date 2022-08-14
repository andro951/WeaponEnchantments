using System.IO;
using Terraria;
using Terraria.ID;

namespace KokoLib.Emitters;

class ProjEmitter : ModHandlerEmitter<Projectile>
{
	public override Projectile Read(BinaryReader reader)
	{
		var proj =  Main.projectile[reader.ReadInt32()];
		if(Main.netMode != NetmodeID.SinglePlayer &&  proj.owner == Main.myPlayer)
		{
			proj.netUpdate = true;
		}
		return proj;
	}

	public override void Write(BinaryWriter writer, Projectile ins) => writer.Write(ins.whoAmI);
}