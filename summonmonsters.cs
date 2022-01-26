using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.SHAKE
{

    public class GoodMonsterPlugin : BasePlugin, IInGameWorldPainter
	{

        public WorldDecoratorCollection Decorator { get; set; }
        private Dictionary<string, string> _names = new Dictionary<string, string>();

        public GoodMonsterPlugin()
		{
            Enabled = true;
		}

        public override void Load(IController hud)
        {
            base.Load(hud);

            AddNames("Dark Vessel", "Skeletal Summoner", "Maggot Brood", "Wretched Mother", "Tomb Guardian", "Returned Summoner", "Retching Cadaver", "Deathspitter");

            Decorator = new WorldDecoratorCollection(
                new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(120, 50, 255, 50, 0),
                    ShadowBrush = Hud.Render.CreateBrush(96, 0, 0, 0, 1),
                    ShapePainter = new CircleShapePainter(Hud),
                    Radius = 2,
                },
                new GroundLabelDecorator(Hud)
                {
                    BackgroundBrush = Hud.Render.CreateBrush(120, 0, 169, 28, 0),
                    TextFont = Hud.Render.CreateFont("tahoma", 6.5f, 255, 255, 255, 255, false, false, false),
                }
                );
        }

        public void AddNames(params string[] names)
        {
            foreach (var name in names)
            {
                _names[name] = name;
            }
        }

        public void RemoveName(string name)
        {
            if (_names.ContainsKey(name)) _names.Remove(name);
        }

        public void PaintWorld(WorldLayer layer)
        {
            var monsters = Hud.Game.AliveMonsters;
            foreach (var monster in monsters)
            {
                if (_names.ContainsKey(monster.SnoMonster.NameEnglish) || _names.ContainsKey(monster.SnoMonster.NameLocalized))
                {
                    Decorator.Paint(layer, monster, monster.FloorCoordinate, monster.SnoMonster.NameLocalized);
                }
            }
        }

    }

}
